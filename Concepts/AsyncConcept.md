# Konzept: Asynchrone Datenbankzugriffe in NDO

## 1. Ziel

* Alle Operationen, die mit der Datenbank kommunizieren, laufen intern asynchron
  (`OpenAsync`, `ExecuteReaderAsync`, `ReadAsync`, `NextResultAsync`, ...).
* `SqlPersistenceHandler` und `NDOMappingTableHandler` verwenden keinen `DbDataAdapter` mehr,
  sondern ausschließlich `DbCommand`/`DbDataReader`.
* Die öffentlichen Eintrittspunkte gibt es in einer synchronen und einer asynchronen
  Variante mit dem Postfix `Async`:
  * Abfragen: `NDOQuery<T>`, `IQuery`, `VirtualTable<T>`
  * Änderungen: `PersistenceManager.Save` (plus `IPersistenceManager`, `OfflinePersistenceManager`)
  * Laden: `PersistenceManager.LoadData`, `LoadRelation`, `Refresh`
  * Direkter SQL-Zugriff: `ISqlPassThroughHandler`
* Die synchronen Methoden rufen **so früh wie möglich** die asynchrone Variante auf:
  `XxxAsync(...).ConfigureAwait(false).GetAwaiter().GetResult()`.
* Jedes `await` in der gesamten Aufrufkette bis einschließlich `SqlPersistenceHandler`
  verwendet `.ConfigureAwait(false)`.
* Gleichzeitige (parallele) Nutzung eines `PersistenceManager` wird erkannt und mit einer Exception abgewiesen (4.14).
* `PersistenceManager.GetClassExtent` entfällt.
* `IProvider.NewConnection`/`NewSqlCommand` arbeiten mit `DbConnection`/`DbCommand`; NDO.dll verwendet intern
  durchgängig `DbConnection`/`DbCommand`/`DbTransaction` und damit direkt deren Async-Methoden (4.1, 4.12.1).
* `IProvider.NewDataAdapter` und `IProvider.NewCommandBuilder` werden vollständig entfernt (Interface, abstrakte
  Basisklasse, alle Provider). Der letzte Nutzer, `NDOAbstractProvider.GetDatabaseStructure`, liest das Schema künftig
  über `ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo)` (4.12).
* `VirtualTable<T>` unterstützt `await foreach` (4.10).
* Typen, deren `Dispose` Datenbankoperationen ausführt (Rollback, Schließen von Connections), implementieren zusätzlich
  `IAsyncDisposable` (4.15).
* Unterstützte Target Frameworks: nur noch `net8.0`, `net10.0`, `net11.0` (kein netstandard mehr, siehe 2.4).

Die Umstellung gehört in den Branch `main/v6.0`; Breaking Changes an den Erweiterungs-Interfaces
(`IPersistenceHandler`, `IMappingTableHandler`, `INDOTransactionScope`, `IQuery`, `ISqlPassThroughHandler`,
`IProvider`)
sind damit vertretbar.

---

## 2. Ist-Analyse

### 2.1 Aufrufketten mit Datenbankzugriff

| Eintrittspunkt | Kette | DB-Operation im Handler |
|---|---|---|
| `NDOQuery<T>.Execute()` | `GetResultList` → `ExecuteSubQuery` / `QueryOrderedPolymorphicList` → `ExecuteOrderedSubQuery` → `pm.CheckTransaction` | `IPersistenceHandler.PerformQuery` → `dataAdapter.Fill` |
| `NDOQuery<T>.ExecuteSingle()` | → `Execute()` | wie oben |
| `NDOQuery<T>.ExecuteAggregate(...)` | `ExecuteAggregateQuery` → `pm.CheckTransaction` | `ExecuteBatch` (`ExecuteReader`) |
| `NDOQuery<T>.DeleteDirectly()` | `pm.CheckTransaction` → `CheckEndTransaction(true)` | `ExecuteBatch` |
| `NDOQuery<T>` (SQL-Query) | `ExecuteSqlQuery` (aktuell ungenutzt) | `PerformQuery` |
| `VirtualTable<T>` | `ResultTable`, `Count`, `Max/Min/Sum/...`, `First/Single/...`, `DeleteDirectly`, `GetEnumerator`, impliziter Cast nach `List<T>` → `NDOQuery<T>` | wie oben |
| `PersistenceManager.Save()` | `UpdateDeletedMappingTableEntries` → `UpdateTypes(delete)` → `UpdateTypes` → (ggf. 2. Durchlauf) → `UpdateCreatedMappingTableEntries` → `EndSave` → `CheckEndTransaction` | `Update`, `UpdateDeletedObjects` → `dataAdapter.Update(rows)`; `IMappingTableHandler.Update(ds)` → `dataAdapter.Update(dt)`; `TransactionScope.Complete` → `tx.Commit()` |
| Lazy Loading (`StateManager.LoadData/LoadField/LoadRelation`), `pm.LoadData`, `pm.LoadRelation`, `pm.Refresh`/`RefreshAll`, `GetClassExtent` (entfällt) | `LoadData` → `NDOQuery.Execute`; `LoadRelation` → `IMappingTableHandler.FindRelatedObjects` bzw. `QueryRelatedObjects`; `Refresh` → `MakeHollow` + `LoadData` | `PerformQuery`, `FindRelatedObjects` → `dataAdapter.Fill` |
| `FindObject` | Cache-Lookup bzw. Erzeugen eines Hollow-Objekts | **keine** – das Laden geschieht erst später per Lazy Loading |
| Transaktionen | `PersistenceManager.CheckTransaction` → `INDOTransactionScope.CheckTransaction/GetConnection` | `conn.Open()`, `conn.BeginTransaction()`, `tx.Commit()`, `tx.Rollback()` |
| `ISqlPassThroughHandler.Execute/BeginTransaction/CommitTransaction` | `TransactionScope.CheckTransaction` → `GetConnection` bzw. `TransactionScope.Complete` | `conn.Open()`, `ExecuteReader`/`ExecuteNonQuery`, `tx.Commit()` |

Nutzer des `ISqlPassThroughHandler` innerhalb von NDO: `PersistenceManager.BuildDatabase` (zwei Überladungen) und
`GetSchemaIds` (Schema-Transitions). Außerhalb: `IntegrationTests`, `UnitTestGenerator` (generierter Testcode).

### 2.2 Was der `DbDataAdapter` heute implizit leistet

Diese Semantik muss beim Wegfall des Adapters explizit nachgebaut werden:

**Fill (`PerformQuery`, `FindRelatedObjects`)**
1. Zuordnung der Reader-Spalten zu `DataColumn`s per Name, **case-insensitiv**
   (wichtig z. B. für Oracle, das Spaltennamen in Großbuchstaben liefert).
2. `MissingSchemaAction.Add`: Spalten, die nicht im Template existieren, werden ergänzt.
3. Bei vorhandenem Primärschlüssel im Template werden Zeilen mit gleichem Schlüssel überschrieben (Merge).
4. `AcceptChangesDuringFill = true` → alle Zeilen sind `Unchanged`.
5. Typkonvertierung über den `DataColumn`-Storage (z. B. `Int64` → `Int32` bei Sqlite, `decimal` → `int` bei Oracle).

**Update (`Update`, `UpdateDeletedObjects`, `IMappingTableHandler.Update`)**
1. Auswahl des Commands nach `RowState` (Added → Insert, Modified → Update, Deleted → Delete).
2. Befüllen der Parameter aus `IDataParameter.SourceColumn` und `SourceVersion`
   (die Parameter werden in `GenerateInsert/Update/DeleteCommand` bereits mit diesen Angaben angelegt).
3. `UpdatedRowSource.FirstReturnedRecord` beim Insert-Batch (`INSERT ...; SELECT ... WHERE id = LAST_ID`):
   Die erste zurückgelieferte Zeile wird in die `DataRow` zurückgeschrieben (Autoincrement-Ids).
4. `RecordsAffected == 0` bei Update/Delete → `DBConcurrencyException` mit gesetzter `Row`.
5. Nach Erfolg `row.AcceptChanges()` (`AcceptChangesDuringUpdate = true`); gelöschte Zeilen werden dadurch `Detached`
   (`OnConcurrencyError` prüft auf `Detached`).
6. `RowUpdated`-Event: wird über `IProvider.RegisterRowUpdateHandler(IRowUpdateListener)` genutzt, um bei Providern
   ohne Insert-Batch die Autoincrement-Id nachzulesen (`SqlPersistenceHandler.OnRowUpdate`).
   **Befund:** Kein Provider im Repository überschreibt `RegisterRowUpdateHandler`, auch nicht der Postgres-Provider
   (`SupportsInsertBatch == false`, `SupportsLastInsertedId == true`). Dieser Pfad ist also derzeit wirkungslos.
   Die Neuimplementierung behebt das nebenbei (siehe 4.3).
7. Zeilenreihenfolge: Rows werden in der Reihenfolge des übergebenen Arrays bzw. der Tabelle verarbeitet, `UpdateBatchSize = 1`.

### 2.3 Synchrone Pflicht-Pfade

Lazy Loading wird vom Enhancer in Property-Zugriffe eingewoben (`IStateManager.LoadData`, `LoadField`, `LoadRelation`).
Diese Pfade **müssen synchron bleiben**; sie werden zu internen synchronen Eintrittspunkten, die ebenfalls
nach dem Muster `...Async().ConfigureAwait(false).GetAwaiter().GetResult()` arbeiten.

### 2.4 Target Frameworks

Ist-Stand:

| Projekt | heute |
|---|---|
| `NDO.dll`, `NDO.Mapping`, `NDO.ProviderFactory`, Provider (`NDO.Sqlite`, `NDO.SqlServer`, `NDO.Oracle`, `NDO.MySql`, `NDO.Postgre`) | `netstandard2.0; netstandard2.1; net6.0; net8.0; net9.0` |
| `NDO.MySqlConnector` | `netstandard2.0; net6.0; net8.0; net9.0` |
| `NDOInterfaces` | `netstandard2.0; netstandard2.1; net6.0; net8.0; net48` |
| `*UISupport` der Provider | `net4.8` (referenzieren `NDOInterfaces`) |

**Entscheidung:** netstandard wird nicht mehr unterstützt. Die Laufzeit-Bibliotheken (`NDO.dll`, `NDO.Mapping`,
`NDO.ProviderFactory`, `NDOInterfaces`, alle Provider) erhalten `net8.0; net10.0; net11.0`.

Folgen:
* Alle benötigten Async-APIs stehen auf allen Targets zur Verfügung
  (`OpenAsync`, `ExecuteReaderAsync`, `ReadAsync`, `NextResultAsync`, `BeginTransactionAsync`, `CommitAsync`,
  `RollbackAsync`, `CloseAsync`, `IAsyncDisposable`/`await using`, `IAsyncEnumerable<T>`).
  **Es gibt keine `#if`-Weichen für Framework-Unterschiede.**
* Die netstandard2.0-spezifischen Paketreferenzen (`PatchProductVersion` mit Bedingung `netstandard2.0`) müssen
  auf ein verbleibendes Target umgestellt werden.
* Die .NET-Analyzer (CA2007) sind im SDK enthalten; ein zusätzliches Analyzer-Paket ist nicht nötig.
* `net11.0` erfordert das .NET-11-SDK (Release voraussichtlich November 2026) auf Build-Servern und in `NDO.Build`.
* Ausnahme `NDOInterfaces`: wird weiterhin **zusätzlich für `net48`** gebaut
  (`net48; net8.0; net10.0; net11.0`), weil die `*UISupport`-Projekte (`net4.8`, Tooling für Visual Studio) es
  referenzieren. `NDOInterfaces` darf deshalb keine APIs verwenden, die es in .NET Framework 4.8 nicht gibt
  (z. B. `DbDataReader.GetColumnSchema()`, `IAsyncDisposable`, `DbConnection.BeginTransactionAsync`).
  Das ist unkritisch, weil die Async-Umstellung ausschließlich NDO.dll betrifft (siehe auch 4.12).
* Die Provider referenzieren `NDOInterfaces` heute als NuGet-Paket (`Version="5.1.0"`), nicht als Projekt. Änderungen an
  `IProvider` werden für die Provider erst wirksam, wenn sie auf ein `NDOInterfaces`-6.0-Paket (ggf. Preview) umgestellt sind (siehe 8).

---

## 3. Grundprinzipien

1. **Async all the way down, sync nur an der Oberfläche.** Die Implementierung existiert genau einmal (asynchron).
   Synchrone Methoden sind dünne Wrapper ohne eigene Logik:
   ```csharp
   public List<T> Execute()
   {
       return ExecuteAsync().ConfigureAwait( false ).GetAwaiter().GetResult();
   }
   ```
2. **`ConfigureAwait(false)` bei jedem `await`** in NDO.dll, auch in Lambdas und `using`-Blöcken.
   Fehlt es nur an einer Stelle, kann der synchrone Wrapper unter einem `SynchronizationContext`
   (WinForms, WPF, klassisches ASP.NET) **deadlocken**.
   Absicherung über den Analyzer CA2007 als Fehler (siehe 4.13).
3. **`Task`/`Task<T>`** als Rückgabetyp, kein `ValueTask`. `ValueTask` wäre zwar auf allen Targets verfügbar, bringt aber
   bei Methoden, die praktisch immer IO machen, keinen Vorteil und hat Nutzungsbeschränkungen (nur einmal awaiten).
4. **`CancellationToken cancellationToken = default`** als letzter optionaler Parameter aller öffentlichen
   Async-Methoden; wird bis zu den ADO.NET-Aufrufen durchgereicht. (Ausnahme: `params`-Überladungen, siehe 4.11.)
5. **Keine Nebenläufigkeit innerhalb eines PersistenceManagers.** Async bedeutet hier nicht „parallel“. Ein
   `PersistenceManager` (Cache, DataSet, Transaktionen) ist nicht threadsicher; Subqueries werden weiterhin
   sequenziell ausgeführt (kein `Task.WhenAll`). Parallele Aufrufe werden durch einen Guard erkannt (4.14).
6. **Exception-Verhalten bleibt gleich:** `NDOException` mit denselben Nummern, `DBConcurrencyException` für Kollisionen.
   Durch `GetAwaiter().GetResult()` (statt `.Result`) werden Exceptions nicht in `AggregateException` verpackt.

---

## 4. Zielarchitektur je Schicht

### 4.1 ADO.NET-Typen: `DbConnection`/`DbCommand`/`DbTransaction` statt `IDb*`

`IProvider.NewConnection` liefert künftig `DbConnection`, `IProvider.NewSqlCommand` erwartet eine `DbConnection` und
liefert ein `DbCommand` (Details der Provider-Schnittstelle in 4.12). Damit stehen alle Async-Methoden ohne Cast
zur Verfügung: `OpenAsync`, `CloseAsync`, `DisposeAsync`, `BeginTransactionAsync`, `ExecuteReaderAsync`,
`ExecuteNonQueryAsync`, `ExecuteScalarAsync`, `DbTransaction.CommitAsync`/`RollbackAsync`, `DbDataReader.ReadAsync`/`NextResultAsync`.

Folgen:
* Eine Hilfsschicht (`DbAsyncExtensions`) mit Casts und synchronem Fallback ist **nicht nötig**. Der mögliche
  Laufzeitfehler „Provider liefert kein `DbCommand`“ entfällt, weil der Compiler die Typen sicherstellt.
* NDO.dll verwendet intern und in seinen Erweiterungs-Interfaces durchgängig die `Db*`-Typen:

| Stelle | bisher | neu |
|---|---|---|
| `IPersistenceHandlerBase.Connection` | `IDbConnection` | `DbConnection` |
| `IPersistenceHandlerBase.Transaction` | `IDbTransaction` | `DbTransaction` |
| `INDOTransactionScope.GetConnection(Async)` | `Func<IDbConnection>` → `IDbConnection` | `Func<DbConnection>` → `Task<DbConnection>` |
| `INDOTransactionScope.GetTransaction` | `IDbTransaction` | `DbTransaction` |
| `SqlPersistenceHandler`, `NDOMappingTableHandler` (Commands, Connection, Transaction) | `IDbCommand`, `IDbConnection`, `IDbTransaction` | `DbCommand`, `DbConnection`, `DbTransaction` |
| `NDOTransactionScope` (`UsedConnectionsInfo.Connection`, `usedTransactions`), `NDODistributedTransactionScope` (`usedConnections`) | `IDbConnection`, `IDbTransaction` | `DbConnection`, `DbTransaction` |
| `SqlPassThroughHandler`, `SqlDumper` | `IDbConnection`, `IDbCommand` | `DbConnection`, `DbCommand` |

* `DbConnection.BeginTransactionAsync` liefert `ValueTask<DbTransaction>`; das Ergebnis wird direkt awaited
  (`await conn.BeginTransactionAsync( il, ct ).ConfigureAwait( false )`), nicht gespeichert.
* Parameter-Objekte bleiben vom Typ `IDataParameter`/`IDbDataParameter` (`AddParameter`, `GetDbTypeString`). Sie haben
  keine IO-Methoden, eine Umstellung brächte nichts.
* Mocks: Die bestehenden Tests mocken `IPersistenceHandler`/`IPersistenceHandlerManager`, nicht Connections oder
  Commands. Wird künftig eine Connection gebraucht, nimmt man eine echte (Sqlite in-memory) statt eines Mocks des
  abstrakten `DbConnection`.
* `DbConnection`/`DbCommand` gibt es auch in .NET Framework 4.8. Das `net48`-Target von `NDOInterfaces` und die
  `*UISupport`-Projekte sind deshalb nicht betroffen.

### 4.2 Lesen ohne Adapter: `DbDataTableFiller` (neu, internal)

Ersetzt `dataAdapter.Fill(table)` mit der unter 2.2 beschriebenen Semantik:

```csharp
internal static class DbDataTableFiller
{
    public static async Task FillAsync( DbCommand command, DataTable table, CancellationToken ct )
    {
        await using (var reader = await command.ExecuteReaderAsync( ct ).ConfigureAwait( false ))
        {
            // 1. Spaltenzuordnung einmalig aufbauen (case-insensitiv, fehlende Spalten ergänzen)
            int fieldCount = reader.FieldCount;
            var targetColumns = new DataColumn[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                string name = reader.GetName( i );
                DataColumn col = table.Columns[name];          // DataColumnCollection sucht case-insensitiv
                if (col == null)
                    col = table.Columns.Add( UniqueName( table, name ), reader.GetFieldType( i ) );
                targetColumns[i] = col;
            }

            // 2. Zeilen lesen
            table.BeginLoadData();
            try
            {
                var values = new object[fieldCount];
                while (await reader.ReadAsync( ct ).ConfigureAwait( false ))
                {
                    reader.GetValues( values );
                    object[] rowValues = MapToTableOrder( table, targetColumns, values );
                    table.LoadDataRow( rowValues, LoadOption.OverwriteChanges ); // Unchanged, Merge über PK
                }
            }
            finally
            {
                table.EndLoadData();
            }
        }
    }
}
```

Hinweise:
* `LoadDataRow` mit `LoadOption.OverwriteChanges` entspricht `Fill` mit `AcceptChangesDuringFill = true`
  inkl. Merge über einen Primärschlüssel.
* Doppelte Spaltennamen im Resultset (z. B. bei Joins) werden wie bei `Fill` durch Suffixe eindeutig gemacht (`UniqueName`).
* `GetValues` ist synchron, das ist korrekt: Nach `ReadAsync` liegt die Zeile im Puffer (kein `CommandBehavior.SequentialAccess`).
* `await using` erfordert `.ConfigureAwait(false)` auch für das `DisposeAsync`
  (`await using (var reader = (...).ConfigureAwait(false))` bzw. getrennte Variable); CA2007 meldet fehlende Stellen.

### 4.3 Schreiben ohne Adapter: `DbRowUpdater` (neu, internal)

Ersetzt `dataAdapter.Update(rows)` bzw. `dataAdapter.Update(dt)`:

```csharp
internal class DbRowUpdater
{
    // insert/update/delete dürfen null sein (MappingTableHandler hat kein Update-Command)
    public DbRowUpdater( DbCommand insert, DbCommand update, DbCommand delete,
                         Func<DataRow, CancellationToken, Task> afterInsert = null ) { ... }

    public async Task UpdateAsync( IEnumerable<DataRow> rows, CancellationToken ct )
    {
        foreach (DataRow row in rows)
        {
            DbCommand cmd;
            StatementType st;
            switch (row.RowState)
            {
                case DataRowState.Added:    cmd = insert; st = StatementType.Insert; break;
                case DataRowState.Modified: cmd = update; st = StatementType.Update; break;
                case DataRowState.Deleted:  cmd = delete; st = StatementType.Delete; break;
                default: continue;
            }

            SetParameterValues( cmd, row );    // aus SourceColumn/SourceVersion, DBNull bei null,
                                               // Fallback auf Current, wenn !row.HasVersion(Original)
            int recordsAffected;
            if (st == StatementType.Insert && cmd.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord)
                recordsAffected = await ExecuteAndReadBackAsync( cmd, row, ct ).ConfigureAwait( false );
            else
                recordsAffected = await cmd.ExecuteNonQueryAsync( ct ).ConfigureAwait( false );

            if (recordsAffected == 0)
            {
                if (st == StatementType.Insert)
                    throw new NDOException( ..., "Insert affected 0 rows" );
                throw new DBConcurrencyException( $"Concurrency violation: the {st}Command affected 0 of the expected 1 records.", null, new[] { row } );
            }

            if (st == StatementType.Insert && afterInsert != null)
                await afterInsert( row, ct ).ConfigureAwait( false );

            row.AcceptChanges();   // Deleted-Rows werden Detached
        }
    }
}
```

* `ExecuteAndReadBackAsync`: `ExecuteReaderAsync`, erste Zeile per `ReadAsync` lesen, Werte per Name
  (case-insensitiv) in die `DataRow` schreiben, Rest-Resultsets mit `NextResultAsync` konsumieren,
  `RecordsAffected` erst **nach** dem Schließen des Readers auswerten.
  Prüfen: Autoincrement-Spalten im Schema dürfen nicht `ReadOnly` sein (ggf. temporär aufheben).
* `afterInsert` ersetzt den `RowUpdated`-Mechanismus: Wenn `hasAutoincrementedColumn && !provider.SupportsInsertBatch
  && provider.SupportsLastInsertedId`, übergibt der `SqlPersistenceHandler` eine Methode `ReadLastInsertedIdAsync`
  (der bisherige Inhalt von `OnRowUpdate`, asynchron, **mit gesetzter Transaction** am Command – das fehlt heute).
  Damit funktioniert der Pfad für Postgres künftig tatsächlich.
* Die `DBConcurrencyException` enthält die Row; der bisherige Code im Handler, der `dbex.Row` sucht, entfällt.

### 4.4 Neue Interfaces

**`IPersistenceHandlerBase`** – unverändert (`Connection`, `Transaction` bleiben Properties ohne IO).

**`IPersistenceHandler`** – nur noch asynchrone DB-Methoden:

```csharp
public interface IPersistenceHandler : IPersistenceHandlerBase
{
    event ConcurrencyErrorHandler ConcurrencyError;

    Task UpdateAsync( DataTable dt, CancellationToken cancellationToken = default );
    Task UpdateDeletedObjectsAsync( DataTable dt, CancellationToken cancellationToken = default );
    Task<IList<Dictionary<string, object>>> ExecuteBatchAsync( string[] statements, IList parameters, CancellationToken cancellationToken = default );
    Task<DataTable> PerformQueryAsync( string expression, IList parameters, DataSet templateDataset, CancellationToken cancellationToken = default );

    IMappingTableHandler GetMappingTableHandler( Relation r );
    void Initialize( NDOMapping ndoMapping, Type t, Action<Type, IPersistenceHandler> disposeCallback );
}
```

* Die Ableitung von `IRowUpdateListener` entfällt.
* `ConcurrencyErrorHandler` bleibt synchron (Benutzer-Callback `CollisionEvent`).

**`IMappingTableHandler`**:

```csharp
Task UpdateAsync( DataSet ds, CancellationToken cancellationToken = default );
Task<DataTable> FindRelatedObjectsAsync( ObjectId id, DataSet templateDataSet, CancellationToken cancellationToken = default );
```

**`INDOTransactionScope`**:

```csharp
Task CheckTransactionAsync( CancellationToken cancellationToken = default );
Task CompleteAsync( CancellationToken cancellationToken = default );
Task<DbConnection> GetConnectionAsync( Connection ndoConnection, Func<DbConnection> factory, CancellationToken cancellationToken = default );
DbTransaction GetTransaction( string id );    // bleibt synchron (kein IO)
void Dispose();                               // synchroner Wrapper (4.15)
ValueTask DisposeAsync();                     // aus IAsyncDisposable: Rollback + Schließen der Connections (4.15)
```

`INDOTransactionScope` leitet künftig von `IDisposable` **und** `IAsyncDisposable` ab.

Die synchronen Varianten `CheckTransaction`, `Complete`, `GetConnection` entfallen. Der einzige externe Nutzer,
`SqlPassThroughHandler`, wird selbst asynchron (4.11) und braucht sie nicht mehr.

### 4.5 `SqlPersistenceHandler`

* Felder `dataAdapter` und Property `DataAdapter` entfernen, `provider.NewDataAdapter(...)` in `Initialize` entfernen,
  stattdessen ein `DbRowUpdater` mit `insertCommand`, `updateCommand`, `deleteCommand` und ggf. `afterInsert`.
* `provider.RegisterRowUpdateHandler(this)` in `GenerateInsertCommand` entfällt; die Fehlerprüfung (NDOException 32) bleibt.
* `OnRowUpdate(DataRow)` → `private async Task ReadLastInsertedIdAsync(DataRow row, CancellationToken ct)`;
  Command bekommt `this.transaction`.
* `Update` → `UpdateAsync`: Logik (Timestamp setzen, `Dump`, Fehlerbehandlung 37) bleibt; `dataAdapter.Update(rows)` →
  `await rowUpdater.UpdateAsync(rows, ct).ConfigureAwait(false)`. Der `catch (DBConcurrencyException)`-Zweig ruft
  weiterhin `ConcurrencyError` auf bzw. wirft neu.
* `UpdateDeletedObjects` → `UpdateDeletedObjectsAsync` analog (Fehler 38).
* `PerformQuery` → `PerformQueryAsync`: `dataAdapter.Fill(table)` → `await DbDataTableFiller.FillAsync(selectCommand, table, ct).ConfigureAwait(false)` (Fehler 40).
* `ExecuteBatch` → `ExecuteBatchAsync`: `conn.Open()` → `OpenAsync`, `ExecuteReader` → `ExecuteReaderAsync`,
  `Read` → `ReadAsync`, `NextResult` → `NextResultAsync`. Das `finally` mit `closeIt` bleibt.
* Thread-Sicherheit: Der Handler nutzt die Instanz-Commands (`selectCommand` usw.). Das bleibt korrekt, solange
  ein Handler-Objekt während eines `await` nicht von einem zweiten Aufrufer verwendet wird. Der Pool
  (`NDOPersistenceHandlerManager`, Rückgabe über `Dispose` → `disposeCallback`) garantiert das, da
  `using (handler) { await ... }` den Handler erst nach Abschluss zurückgibt. Muss beim Review explizit geprüft werden.

### 4.6 `NDOMappingTableHandler`

* `dataAdapter` entfernen, `DbRowUpdater(insertCommand, null, deleteCommand)` verwenden.
* `FindRelatedObjects` → `FindRelatedObjectsAsync` mit `DbDataTableFiller` (Fehler 25).
* `Update(DataSet)` → `UpdateAsync`: Zeilen der Tabelle in Tabellenreihenfolge mit Status Added/Modified/Deleted (Fehler 26).
  Ein `Modified`-Status kommt bei Mapping-Tabellen nicht vor; falls doch, wie bisher Fehler (kein Update-Command).

### 4.7 Transaktionen: `NDOTransactionScope` / `NDODistributedTransactionScope`

* `OpenConnAndStartTransaction` → `OpenConnAndStartTransactionAsync` (`OpenAsync`, `BeginTransactionAsync`).
* `CheckTransactionAsync`, `GetConnectionAsync` (öffnet ggf. Connection + Transaktion, wenn bereits in Transaktion),
  `CompleteAsync` → `CommitTransactionsAsync` (`CommitAsync`).
* `Dispose()` (Rollback + Close) wird zu `DisposeAsync()` mit `RollbackAsync`/`DisposeAsync` der Connections;
  `Dispose()` bleibt als synchroner Wrapper. Aufrufer: `AbortTransaction`, `ISqlPassThroughHandler.Dispose`,
  `PersistenceManager.Close` – jeweils mit Async-Gegenstück (Details in 4.15).
* `NDODistributedTransactionScope`: `System.Transactions.TransactionScope` **muss** mit
  `TransactionScopeAsyncFlowOption.Enabled` erzeugt werden, sonst fließt die ambient Transaction nicht über `await`
  hinweg bzw. es kommt zu `InvalidOperationException` beim Dispose auf einem anderen Thread.
  Achtung: Der Scope muss auf demselben logischen Ausführungsfluss erzeugt und beendet werden.

### 4.8 `PersistenceManager`

Interne Methoden werden asynchron, sichtbare Eintrittspunkte bekommen beide Varianten.

| bisher | neu |
|---|---|
| `CheckTransaction(handler, Type/Connection)` | `CheckTransactionAsync(handler, ..., ct)`: `TransactionScope.CheckTransactionAsync`, `GetConnectionAsync`, `handler.Connection.OpenAsync` |
| `CheckEndTransaction(bool)` | `CheckEndTransactionAsync(bool, ct)` → `TransactionScope.CompleteAsync` |
| `UpdateTypes(types, delete)` | `UpdateTypesAsync(...)` → `handler.UpdateAsync / UpdateDeletedObjectsAsync` |
| `UpdateCreatedMappingTableEntries()` / `UpdateDeletedMappingTableEntries()` | `...Async(ct)` |
| `EndSave(bool)` | `EndSaveAsync(bool, ct)` |
| `Save(bool deferCommit = false)` | `public virtual Task SaveAsync(bool deferCommit = false, CancellationToken ct = default)` – enthält die komplette bisherige Logik |
| | `public void Save(bool deferCommit = false) => SaveAsync(deferCommit).ConfigureAwait(false).GetAwaiter().GetResult();` |
| `LoadData(object)` | `public virtual Task LoadDataAsync(object o, CancellationToken ct = default)`; `public void LoadData(object o)` als Wrapper |
| `LoadRelation(object, string, bool)` | `public virtual Task LoadRelationAsync(object o, string fieldName, bool hollow, CancellationToken ct = default)`; `LoadRelation` als Wrapper |
| `LoadRelation(pc, Relation, hollow)`, `LoadRelationInternal`, `QueryRelatedObjects` (internal) | `...Async`; `handler.FindRelatedObjectsAsync` bzw. `q.ExecuteAsync` |
| `Refresh(object)`, `Refresh(IList)`, `RefreshAll()` | `public virtual Task RefreshAsync(object o, ct)`, `RefreshAsync(IList list, ct)`, `RefreshAllAsync(ct)` (sequenziell); synchrone Wrapper |
| `AbortTransaction()` | `public virtual Task AbortTransactionAsync()` (ohne Token, 4.15) → `TransactionScope.DisposeAsync()`; `AbortTransaction` als Wrapper. `OfflinePersistenceManager` überschreibt künftig `AbortTransactionAsync`. |
| `Abort()` | `public virtual Task AbortAsync()` (ohne Token; Objektzustand zurücksetzen wie bisher, danach `AbortTransactionAsync`); `Abort` als Wrapper |
| `Close()`, `Dispose()` | `CloseAsync()` und `DisposeAsync()` (4.15); `Close`/`Dispose` als Wrapper |
| `LoadField`, `LoadAndMarkDirty` (internal, Lazy Loading) | rufen die synchronen Wrapper (sync-over-async) |
| `FindObject(...)` (alle Überladungen) | **unverändert synchron**, keine Async-Variante: `FindObject` greift nicht auf die Datenbank zu, sondern liefert ein Objekt aus dem Cache bzw. ein neues Hollow-Objekt. Geladen wird erst beim ersten Zugriff (Lazy Loading) oder explizit über `LoadDataAsync`. |
| `GetClassExtent(Type)`, `GetClassExtent(Type, bool)` | **entfällt** (auch in `IPersistenceManager` und `OfflinePersistenceManager`). Ersatz: `pm.Objects<T>().ResultTable` / `await pm.Objects<T>().ToListAsync()` bzw. `new NDOQuery<T>( pm, null, hollow ).Execute()` / `ExecuteAsync()`; untypisiert über `pm.NewQuery( t, null, hollow ).Execute()` / `ExecuteAsync()`. |

* `IPersistenceManager` erhält
  `Task SaveAsync(bool deferCommit = false, CancellationToken cancellationToken = default)`,
  `Task LoadDataAsync(object pc, CancellationToken cancellationToken = default)`,
  `Task LoadRelationAsync(object pc, string fieldName, bool hollow, CancellationToken cancellationToken = default)`,
  `Task RefreshAsync(object pc, CancellationToken cancellationToken = default)`,
  `Task RefreshAsync(IList list, CancellationToken cancellationToken = default)`,
  `Task AbortAsync()`,
  `Task AbortTransactionAsync()`,
  `Task CloseAsync()` und leitet zusätzlich von `IAsyncDisposable` ab; `GetClassExtent` wird entfernt.
* `OfflinePersistenceManager` überschreibt `SaveAsync`, `LoadDataAsync`, `LoadRelationAsync`, `RefreshAsync`,
  `AbortTransactionAsync` und – statt `Dispose()` – `CloseAsync` (nicht mehr die synchronen Methoden). Die synchronen Methoden sind künftig **nicht mehr `virtual`**, sonst gäbe es zwei
  Überschreibpunkte mit unterschiedlichem Verhalten. Die Überschreibungen von `GetClassExtent` entfallen.
* `StateManager` (Lazy Loading) ruft unverändert die synchronen Methoden → intern sync-over-async.
* Benutzer-Callbacks (`OnSavingEvent`, `IPersistenceNotifiable.OnSaving`, `CollisionEvent`, `OnSavedEvent`,
  `OpenConnectionListener`, `ObjectNotPresentEvent`) bleiben synchron. **Wichtig für die Doku:** Callbacks, die nach dem
  ersten `await` aufgerufen werden (`CollisionEvent`, `OnSavedEvent`), laufen auf einem ThreadPool-Thread,
  auch beim synchronen `Save()`. Ein `Control.Invoke` darin führt zu einem Deadlock, da der UI-Thread im `GetResult()` blockiert.
* Migration der Tests: `GetClassExtent` wird in 25 Dateien von `IntegrationTests/IntegrationTests` und in
  `IntegrationTests/NdoUnitTests` verwendet (rund 120 Aufrufe); die Aufrufe werden auf `pm.Objects<T>().ResultTable`
  bzw. `NDOQuery<T>` umgestellt.

### 4.9 `NDOQuery<T>` und `IQuery`

Neue öffentliche Methoden (die bisherige Logik wandert in die Async-Varianten):

```csharp
public Task<List<T>> ExecuteAsync( CancellationToken cancellationToken = default );
public Task<T> ExecuteSingleAsync( bool throwIfResultCountIsWrong = false, CancellationToken cancellationToken = default );
public Task<object> ExecuteAggregateAsync( string field, AggregateType aggregateType, CancellationToken cancellationToken = default );
public Task<object> ExecuteAggregateAsync( AggregateType aggregateType, CancellationToken cancellationToken = default );
public Task<object> ExecuteAggregateAsync<K>( Expression<Func<T, K>> keySelector, AggregateType aggregateType, CancellationToken cancellationToken = default );
public Task DeleteDirectlyAsync( CancellationToken cancellationToken = default );

public List<T> Execute() => ExecuteAsync().ConfigureAwait( false ).GetAwaiter().GetResult();
// usw. für ExecuteSingle, ExecuteAggregate, DeleteDirectly
```

Interne Umstellung:
* `GetResultList` → `GetResultListAsync`.
* `ExecuteSubQuery(Type, QueryContextsEntry)` → `ExecuteSubQueryAsync` (`await pm.CheckTransactionAsync`, `await handler.PerformQueryAsync`).
* Der Iterator `IEnumerable<T> ExecuteSubQuery(QueryContextsEntry)` (`yield return`) entfällt; das Ergebnis der
  Async-Methode wird direkt gecastet und an `result` angehängt.
* `QueryOrderedPolymorphicList` / `ExecuteOrderedSubQuery` / `ExecuteAggregateQuery` / `ExecuteSqlQuery` → `...Async`.
* Query-Cache (`UseQueryCache`) unverändert in `ExecuteAsync`.
* Der per `#if MaskOutPrefetches` ausgeblendete Prefetch-Code wird beim späteren Reaktivieren direkt asynchron geschrieben.
* Nebenbefund: In `GetResultList` wird `fetchResult` berechnet, aber nicht zurückgegeben (und die Schleifenbedingung
  `i + this.take` ist fehlerhaft). Bei der Umstellung korrigieren oder separat als Bug erfassen.

`IQuery` erhält entsprechend `ExecuteAsync` (→ `Task<IList>`), `ExecuteSingleAsync`, `ExecuteAggregateAsync`.
`PersistenceManager.NewQuery` bleibt unverändert.

### 4.10 `VirtualTable<T>`

| synchron (bleibt) | neu asynchron |
|---|---|
| `ResultTable`, impliziter Cast nach `List<T>`, `GetEnumerator()` | `ToListAsync(ct)` |
| `Select<S>(selector)` | `SelectAsync<S>(selector, ct)` |
| `Count` (Property) | `CountAsync(ct)` |
| `Max`, `Min`, `Sum`, `Average`, `StandardDeviation`, `Variance` | `MaxAsync`, `MinAsync`, `SumAsync`, `AverageAsync`, `StandardDeviationAsync`, `VarianceAsync` |
| `First`, `FirstOrDefault`, `Single`, `SingleOrDefault` | `FirstAsync`, `FirstOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync` |
| `DeleteDirectly` | `DeleteDirectlyAsync` |
| – | `GetAsyncEnumerator(ct)` für `await foreach`, `AsAsyncEnumerable()` → `IAsyncEnumerable<T>` |

Die synchronen Member delegieren direkt an `Ndoquery.XxxAsync().ConfigureAwait(false).GetAwaiter().GetResult()`
(Vorgabe „so früh wie möglich“).

**`await foreach` / `IAsyncEnumerable<T>`**

```csharp
public class VirtualTable<T> : IEnumerable<T>
{
    // Für APIs, die ein IAsyncEnumerable<T> erwarten (z. B. ASP.NET Core Streaming-Responses, System.Linq.AsyncEnumerable)
    public async IAsyncEnumerable<T> AsAsyncEnumerable( [EnumeratorCancellation] CancellationToken cancellationToken = default )
    {
        List<T> result = await Ndoquery.ExecuteAsync( cancellationToken ).ConfigureAwait( false );
        foreach (T item in result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    // Musterbasiert: await foreach (var x in pm.Objects<T>().Where(...)) funktioniert ohne Interface-Implementierung
    public IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken cancellationToken = default )
        => AsAsyncEnumerable( cancellationToken ).GetAsyncEnumerator( cancellationToken );
}
```

* `VirtualTable<T>` implementiert `IAsyncEnumerable<T>` **nicht direkt**, sondern stellt `GetAsyncEnumerator`
  musterbasiert bereit (der C#-Compiler akzeptiert für `await foreach` jede Instanzmethode `GetAsyncEnumerator`).
  Grund: `VirtualTable<T>` implementiert bereits `IEnumerable<T>`. Ab .NET 10 sind die LINQ-Operatoren für
  `IAsyncEnumerable<T>` (`System.Linq.AsyncEnumerable`) Teil der BCL und liegen im Namespace `System.Linq`; unter `net8.0`
  gilt das Gleiche mit dem Paket `System.Linq.Async`/`System.Linq.AsyncEnumerable`. Ein Typ, der beide Interfaces
  implementiert, macht jeden LINQ-Aufruf, der keine Instanzmethode von `VirtualTable<T>` ist (z. B. `vt.Any()`,
  `vt.ToList()`, `vt.First(x => ...)`, `vt.Count()`), mehrdeutig (CS0121). EF Core hat `DbSet<T>` aus demselben Grund
  auf `AsAsyncEnumerable()` umgestellt.
* `ConfigureAwait(false)`: Innerhalb des Iterators bekommt das `await` auf `ExecuteAsync` wie überall
  `ConfigureAwait(false)`. Wie die Schleife des Aufrufers fortgesetzt wird, entscheidet dagegen der Aufrufer; er schreibt bei Bedarf
  `await foreach (var x in vt.AsAsyncEnumerable().ConfigureAwait(false))`.
* Die Abfrage wird beim ersten `MoveNextAsync` vollständig ausgeführt und materialisiert (NDO baut aus dem Resultset
  Objekte über das `DataSet`); es gibt kein zeilenweises Streaming vom Server. Das entspricht dem Verhalten von
  `GetEnumerator()` und wird so dokumentiert.
* Der Guard (4.14) ist nur während `ExecuteAsync` aktiv. Im Schleifenrumpf sind PM-Aufrufe (z. B. Lazy Loading) erlaubt.

Hinweis: Namen wie `ToListAsync`/`FirstAsync` kollidieren nicht mit EF Core, da es Instanzmethoden von `VirtualTable<T>` sind
(Instanzmethoden haben Vorrang vor Extension-Methoden).

### 4.11 `ISqlPassThroughHandler` / `SqlPassThroughHandler`

Wird in diesem Schritt mit umgestellt.

```csharp
public interface ISqlPassThroughHandler : IDisposable, IAsyncDisposable
{
    // Kernmethode
    Task<DbDataReader> ExecuteAsync( string command, bool returnReader, object[] parameters, CancellationToken cancellationToken = default );
    // Komfort-Überladung mit params, ohne CancellationToken
    Task<DbDataReader> ExecuteAsync( string command, bool returnReader = false, params object[] parameters );
    IDataReader Execute( string command, bool returnReader = false, params object[] parameters );   // Wrapper

    Task BeginTransactionAsync( CancellationToken cancellationToken = default );
    void BeginTransaction();                                                                        // Wrapper
    Task CommitTransactionAsync( CancellationToken cancellationToken = default );
    void CommitTransaction();                                                                       // Wrapper

    IProvider Provider { get; }
}
```

* `params` muss der letzte Parameter sein; ein `CancellationToken` davor würde bei `ExecuteAsync(sql, true, 1, 2)`
  das erste Argument binden wollen. Daher die Kernmethode mit `object[]` plus `CancellationToken` und eine
  `params`-Überladung ohne Token.
* Rückgabe `DbDataReader` statt `IDataReader`, damit der Aufrufer `ReadAsync`/`await using` nutzen kann.
* `ExecuteAsync`: `await TransactionScope.CheckTransactionAsync`, `await TransactionScope.GetConnectionAsync`,
  `OpenAsync`, `ExecuteReaderAsync` bzw. `ExecuteNonQueryAsync`.
* `BeginTransactionAsync` → `TransactionScope.CheckTransactionAsync`; `CommitTransactionAsync` → `TransactionScope.CompleteAsync`.
* `DisposeAsync` → `await pm.TransactionScope.DisposeAsync()` (Rollback einer offenen Transaktion, Schließen der
  Connections) und Zurücksetzen des `TransactionMode`; `Dispose` ist der synchrone Wrapper (4.15).
  Empfohlene Nutzung: `await using (var handler = pm.GetSqlPassThroughHandler()) { ... }`.
* NDO-interne Nutzer (`PersistenceManager.BuildDatabase`, `GetSchemaIds`) bleiben synchron und rufen die Wrapper.
  Ein `BuildDatabaseAsync` ist nicht vorgesehen: `BuildDatabase` wird nur in kleinen Beispielprogrammen ohne Tasks genutzt.
* Anpassen: `IntegrationTests` (`TransactionTests`, `TransactionScopeTests`, `AuditTests`, ...) und der
  `UnitTestGenerator` (`TestGenerator.cs` erzeugt `pm.GetSqlPassThroughHandler(...)`-Code) – für die synchronen
  Aufrufe ändert sich nichts (Signatur und Rückgabetyp von `Execute` bleiben gleich), sie bleiben quellkompatibel.

Weitere Bereiche, die synchron bleiben: `ObjectContainer`/`SerializationIterator` rufen `LoadData`/`LoadRelation` synchron.

### 4.12 Provider-Schnittstelle (`NDOInterfaces`)

#### 4.12.1 `DbConnection`/`DbCommand` in `IProvider`

```csharp
public interface IProvider
{
    DbConnection NewConnection( string parameters );                      // bisher IDbConnection
    DbCommand NewSqlCommand( DbConnection connection );                   // bisher IDbCommand NewSqlCommand(IDbConnection)
    object GetConnectionId( DbConnection connection );                    // bisher IDbConnection
    IDataParameter AddParameter( DbCommand command, string parameterName, object dbType, int size, string columnName );
    IDataParameter AddParameter( DbCommand command, string parameterName, object dbType, int size, ParameterDirection dir,
                                 bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value );
    string[] GetTableNames( DbConnection conn );
    string[] GetTableNames( DbConnection conn, string owner );
    DataSet GetDatabaseStructure( DbConnection conn, string owner );
    ...
}
```

* Gefordert sind `NewConnection` und `NewSqlCommand`. Die übrigen Member mit `IDbConnection`/`IDbCommand`-Parametern
  (`GetConnectionId`, `AddParameter`, `GetTableNames`, `GetDatabaseStructure`) werden **mit umgestellt**. Grund:
  `NewSqlCommand(DbConnection)` lässt sich mit einer `IDbConnection` nicht mehr ohne Cast aufrufen (z. B. in
  `GetDatabaseStructure`). Die Provider müssen ihre Überschreibungen ohnehin anpassen; so bricht die Schnittstelle
  einmal und vollständig statt in zwei Schritten.
* `NDOAbstractProvider` wird entsprechend angepasst (abstrakte Member, `GetTableNames(conn)`, `GetDatabaseStructure`,
  `CreateDatabase` (nutzt `NewConnection`/`NewSqlCommand`, ab Zeile 377), `GetConnectionId`).
* Die Provider liefern bereits heute konkrete `DbConnection`/`DbCommand`-Ableitungen (`SQLiteConnection`,
  `SqlConnection`, `OracleConnection`, `MySqlConnection` (2×), `NpgsqlConnection`). Die Änderung betrifft also nur die
  Signaturen; die Implementierungen bleiben inhaltlich gleich (die vorhandenen Casts auf die konkreten Typen
  wie `(SQLiteConnection) connection` bleiben nötig, da `DbConnection` abstrakt ist).
  Betroffen je Provider: `NewConnection`, `NewSqlCommand`, `GetConnectionId` (außer Sqlite und Oracle, die es nicht
  überschreiben), beide `AddParameter`, `GetTableNames`.
* Aufrufer außerhalb von NDO.dll, die das Ergebnis heute in `IDbConnection`-Variablen speichern und an `NewSqlCommand`
  weitergeben, müssen angepasst werden (Variable als `DbConnection` bzw. `var`):
  * `ClassGenerator/ApplicationController.cs:480-481`
  * `UnitTests/ExecuteSqlBatch/Class1.cs:74-76`
  * Unkritisch, weil sie nur das Ergebnis speichern oder an `GetDatabaseStructure`/`GetTableNames` übergeben:
    `ClassGenerator/NodeEntities/Database.cs:81` (Variable auf `DbConnection` ändern),
    `ClassGenerator/Nodes/DatabaseNode.cs:114` und `TableNode.cs:234` (toter `#if DontUseDataSets`-Code, wird entfernt).
  * `NDOPackage/ConfigurationDialog.cs:746` ruft `NDOMapping.NewConnection` auf – eine andere Methode, nicht betroffen.

#### 4.12.2 Vollständige Entfernung von `NewDataAdapter`

**Ist-Stand der Verwendung von `IProvider.NewDataAdapter` (Stand `main/v6.0`) und Maßnahme**

| Teilprojekt | Datei | Art der Verwendung | Maßnahme |
|---|---|---|---|
| `NDOInterfaces` | `IProvider.cs:64` | Deklaration im Interface | **entfernen** |
| `NDOInterfaces` | `NDOAbstractProvider.cs:57` | abstrakte Deklaration | **entfernen** |
| `NDOInterfaces` | `NDOAbstractProvider.cs:310` (`GetDatabaseStructure`) | `NewDataAdapter(cmd, null, null, null).FillSchema(ds, SchemaType.Source)` für jede Tabelle; liest die Tabellenstruktur der Datenbank in ein `DataSet` | durch `ExecuteReader(SchemaOnly \| KeyInfo)` + `DataTable.Load` ersetzen (s. u.) |
| `NDO` (`NDODLL`) | `SqlPersistenceHandling/SqlPersistenceHandler.cs:456` (`Initialize`) | Adapter für Fill/Update | entfällt durch `DbDataTableFiller`/`DbRowUpdater` (4.5) |
| `NDO` (`NDODLL`) | `NDOMappingTableHandler.cs:71` | Adapter für Fill/Update der Mapping-Tabellen | entfällt (4.6) |
| `ClassGenerator` | `Nodes/TableNode.cs:236` | `FillSchema` in `#if DontUseDataSets` (Symbol nirgends definiert, toter Code) | toten `#if DontUseDataSets`-Block entfernen (auch `Nodes/DatabaseNode.cs:107`) |
| `ClassGenerator` | `NodeEntities/Database.cs:85` | indirekt über `provider.GetDatabaseStructure(conn, null)` | unverändert (nutzt die neue Implementierung) |
| Provider `NDO.Sqlite` | `Provider.cs:77` | Implementierung (`SQLiteDataAdapter`) | **entfernen** |
| Provider `NDO.SqlServer` | `Provider.cs:77` | Implementierung (`SqlDataAdapter`) | **entfernen** |
| Provider `NDO.Oracle` | `OracleProvider.cs:59` | Implementierung (`OracleDataAdapter`) | **entfernen** |
| Provider `NDO.MySql` | `MySqlProvider.cs:61` | Implementierung (`MySqlDataAdapter`) | **entfernen** |
| Provider `NDO.MySqlConnector` | `MySqlProvider.cs:61` | Implementierung (`MySqlDataAdapter`) | **entfernen** |
| Provider `NDO.Postgre` | `Provider.cs:62` | Implementierung (`NpgsqlDataAdapter`) | **entfernen** |

Ebenfalls entfernt werden, weil sie an den `DbDataAdapter` gebunden sind:
* `IProvider.NewCommandBuilder(DbDataAdapter)` – in `IProvider.cs:72`, `NDOAbstractProvider.cs:62` und allen sechs Providern
  implementiert, aber **nirgends aufgerufen**.
* `IRowUpdateListener` (`NDOInterfaces/IRowUpdateListener.cs`, enthält die Property `DbDataAdapter DataAdapter`) und
  `IProvider.RegisterRowUpdateHandler` (`IProvider.cs:261`, leere Default-Implementierung in `NDOAbstractProvider.cs:338`,
  von keinem Provider überschrieben). Statt sie nur `[Obsolete]` zu markieren, werden sie in v6 gleich entfernt, da die
  Provider-Schnittstelle ohnehin bricht und der Mechanismus durch `afterInsert` (4.3) ersetzt ist.

Keine Verwendung in: `SimpleMappingTool`, `NDOEnhancer`, `NDOEnhancer.BuildTask`, `NDO.SchemaGenerator`, `NDOPackage`,
`UISupport` und den `*UISupport`-Projekten der Provider, `NDO.Mapping`, `NDO.ProviderFactory`, `NDO.DataBinding`,
`NdoJsonFormatter`, Testprojekte.

**Neue Implementierung von `GetDatabaseStructure`**

Was `DbDataAdapter.FillSchema(ds, SchemaType.Source)` heute intern tut (Basisklasse in `System.Data.Common`):
1. `SelectCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo)` ausführen.
2. Aus `reader.GetSchemaTable()` über die interne Klasse `SchemaMapping` eine `DataTable` aufbauen:
   Spaltennamen (inkl. Eindeutigmachen doppelter Namen), `DataType`, `AllowDBNull`, `AutoIncrement`, `MaxLength`,
   `ReadOnly`, `Unique`, `PrimaryKey` (aus `IsKey`), Behandlung versteckter Schlüsselspalten (`IsHidden`).

Genau diesen Weg geht man ohne Adapter direkt:

```csharp
public virtual DataSet GetDatabaseStructure( DbConnection conn, string ownerName )
{
    bool wasOpen = conn.State == ConnectionState.Open;
    if (!wasOpen)
        conn.Open();

    DataSet ds = new DataSet();
    try
    {
        foreach (string tableName in this.GetTableNames( conn, ownerName ))
        {
            DbCommand cmd = this.NewSqlCommand( conn );
            cmd.CommandText = ...;   // SELECT * FROM [owner.]table, wie bisher

            DataTable dt = new DataTable( tableName );
            using (DbDataReader reader = cmd.ExecuteReader( CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo ))
            {
                // Load baut das Schema aus reader.GetSchemaTable() auf (MissingSchemaAction.AddWithKey)
                // und liest 0 Zeilen, da SchemaOnly.
                dt.Load( reader );
            }
            ds.Tables.Add( dt );
        }
    }
    finally
    {
        if (!wasOpen)
            conn.Close();
    }
    return ds;
}
```

Warum das für alle Provider funktioniert:
* `DataTable.Load(IDataReader)` verwendet intern dieselbe Schema-Logik wie `DbDataAdapter.FillSchema`
  (`System.Data`: interner `LoadAdapter : DataAdapter` mit `MissingSchemaAction.AddWithKey` → `SchemaMapping` →
  `GetSchemaTable()`). Provider-spezifisch ist in beiden Fällen **nur** der Reader mit seiner `GetSchemaTable()`-Implementierung,
  aufgerufen mit denselben `CommandBehavior`-Flags. Der Provider-Adapter steuert nichts bei, solange er
  `FillSchema` nicht selbst überschreibt.
* Der ClassGenerator verwendet aus dem Schema nur `ColumnName`, `DataType`, `DataTable.PrimaryKey` und
  `DataColumn.AutoIncrement` (`TableNode.cs:245-264`). Genau diese Informationen kommen aus `GetSchemaTable()`
  (`ColumnName`, `DataType`, `IsKey`, `IsAutoIncrement`).
* Es werden nur APIs genutzt, die auch in .NET Framework 4.8 existieren (`ExecuteReader(CommandBehavior)`,
  `DataTable.Load`). `NDOInterfaces` bleibt damit `net48`-fähig. `DbDataReader.GetColumnSchema()` wird bewusst
  **nicht** verwendet: Es fehlt in `net48`, und ohne `IDbColumnSchemaGenerator` fällt es ohnehin auf `GetSchemaTable()` zurück.
* `GetDatabaseStructure` bleibt synchron: Der einzige Nutzer ist das Tooling (`ClassGenerator`), und `NDOInterfaces` soll
  keine Async-Abhängigkeiten bekommen.

Prüfung je Provider:

| Provider | ADO.NET-Bibliothek (laut `csproj`) | Überschreibt der Adapter `FillSchema`? | Besonderheit bei `SchemaOnly \| KeyInfo` |
|---|---|---|---|
| `NDO.Sqlite` | System.Data.SQLite.Core 1.0.119 | nein (`SQLiteDataAdapter` ergänzt nur Update-Events) | Bei Tabellen ohne explizite PK kann eine versteckte `rowid`-Spalte (`IsHidden`) gemeldet werden – gleicher `SchemaMapping`-Pfad wie bisher, also identisches Ergebnis |
| `NDO.SqlServer` | Microsoft.Data.SqlClient 5.2.3 / 6.0.2 | nein | `IsKey`/`IsAutoIncrement` (Identity) zuverlässig |
| `NDO.Oracle` | Oracle.ManagedDataAccess.Core 2.19.290 / 23.26.0 | **nicht anhand der Quellen prüfbar** (Closed Source); `OracleDataAdapter` hat eigene Optionen (`SafeMapping`, `ReturnProviderSpecificTypes`), die aber beim heutigen Aufruf auf Default stehen | Äquivalenztest ist hier zwingend; `NUMBER` → `decimal` wie bisher |
| `NDO.MySql` | MySql.Data 8.4.0 / 9.3.0 | nein (nur Batch-/Update-Erweiterungen und `FillAsync`/`FillSchemaAsync`-Wrapper) | – |
| `NDO.MySqlConnector` | MySqlConnector 2.4.0 | nein | – |
| `NDO.Postgre` | Npgsql 8.0.8 / 9.0.4 | nein | `KeyInfo` löst eine zusätzliche Metadatenabfrage aus – wie bisher bei `FillSchema` |

Die Aussagen „überschreibt nicht“ beruhen auf den öffentlichen Quellen der Bibliotheken und wurden in dieser
Analyse **nicht** durch einen Build oder Test bestätigt (in der Analyseumgebung war kein .NET-SDK verfügbar). Deshalb
gilt verbindlich: Bevor `NewDataAdapter` entfernt wird, läuft für **jeden** Provider ein Äquivalenztest (siehe 7, Punkt 9),
der alte und neue Implementierung gegen dieselbe Datenbank vergleicht.

**Folgen für Provider-Autoren (auch Dritt-Provider)**
* Die Überschreibungen `NewDataAdapter` und `NewCommandBuilder` müssen gelöscht werden; sonst Compilerfehler CS0115
  („kein passender Member zum Überschreiben gefunden“), sobald gegen `NDOInterfaces` 6.x gebaut wird.
* Wer `GetDatabaseStructure` selbst überschrieben und dort einen Adapter benutzt hat, stellt auf das obige Muster um.
  (Im Repository überschreibt kein Provider `GetDatabaseStructure`.)
* Zusätzlich die Umstellung der Target Frameworks (2.4) und der Paketreferenz auf `NDOInterfaces` 6.x.

### 4.13 Absicherung von `ConfigureAwait(false)`

In `NDODLL/.editorconfig` (neu):

```ini
[*.cs]
dotnet_diagnostic.CA2007.severity = error
```

und im `NDO.csproj` `<EnableNETAnalyzers>true</EnableNETAnalyzers>` (bei `net8.0`+ ohnehin Standard; die Analyzer
sind Teil des SDK). Damit schlägt der Build fehl, wenn irgendwo ein `await` ohne `ConfigureAwait` steht.

### 4.14 Guard gegen parallele Nutzung eines `PersistenceManager`

Ziel: `Task.WhenAll(q1.ExecuteAsync(), q2.ExecuteAsync())` auf demselben PM, oder ein zweiter Thread, der während
eines laufenden `SaveAsync` eine Abfrage startet, führt zu einer klaren Exception statt zu zerstörtem Cache/DataSet.

Anforderungen:
* Verschachtelte Aufrufe **im selben logischen Ausführungsfluss** müssen erlaubt bleiben, z. B.
  Lazy Loading in `OnSavingEvent`/`IPersistenceNotifiable.OnSaving`, `Refresh` → `LoadData` → `NDOQuery.Execute`,
  `LoadRelation` → `QueryRelatedObjects` → `NDOQuery.ExecuteAsync`.
* Der Guard muss auch über `await` hinweg wirken, wenn die Fortsetzung auf einem anderen Thread läuft – eine
  Thread-Id oder `[ThreadStatic]` reicht daher nicht.

Umsetzung: Zähler mit `Interlocked` plus `AsyncLocal<bool>`, das den Besitz für den aktuellen logischen Fluss markiert.

```csharp
// PersistenceManager
private int activeOperation;                                   // 0 = frei, 1 = belegt
private readonly AsyncLocal<bool> ownsOperation = new AsyncLocal<bool>();

internal OperationGuard EnterOperation( [CallerMemberName] string caller = null )
{
    if (this.ownsOperation.Value)                              // verschachtelter Aufruf im selben Fluss
        return default;
    if (Interlocked.CompareExchange( ref this.activeOperation, 1, 0 ) != 0)
        throw new NDOException( /* neue Nummer */, $"{caller}: The PersistenceManager is already in use by another operation. "
            + "A PersistenceManager must not be used concurrently. Await each call before starting the next one." );
    this.ownsOperation.Value = true;
    return new OperationGuard( this );
}

internal readonly struct OperationGuard : IDisposable
{
    private readonly PersistenceManager pm;
    public OperationGuard( PersistenceManager pm ) { this.pm = pm; }
    public void Dispose()
    {
        if (this.pm == null) return;                           // verschachtelter Aufruf: nichts freigeben
        this.pm.ownsOperation.Value = false;
        Volatile.Write( ref this.pm.activeOperation, 0 );
    }
}
```

Verwendung am Anfang jedes asynchronen öffentlichen Eintrittspunkts:

```csharp
public virtual async Task SaveAsync( bool deferCommit = false, CancellationToken ct = default )
{
    using (EnterOperation())
    {
        ...
    }
}
```

Eintrittspunkte mit Guard: `SaveAsync`, `LoadDataAsync`, `LoadRelationAsync`, `RefreshAsync`/`RefreshAllAsync`,
`NDOQuery<T>.ExecuteAsync`/`ExecuteSingleAsync`/`ExecuteAggregateAsync`/`DeleteDirectlyAsync` (und damit
`VirtualTable<T>` und `IQuery`), `SqlPassThroughHandler.ExecuteAsync`/`BeginTransactionAsync`/`CommitTransactionAsync`.
Die synchronen Wrapper sind automatisch abgedeckt.

Warum das funktioniert:
* `EnterOperation` ist eine synchrone Methode; die Änderung an `ownsOperation` wird deshalb im aufrufenden
  `async`-Eintrittspunkt sichtbar und fließt mit dem `ExecutionContext` in alle darin awaiteten Aufrufe – auch auf
  ThreadPool-Threads und in Callbacks, die aus diesem Fluss aufgerufen werden.
* Beim Verlassen einer `async`-Methode stellt die Runtime den `ExecutionContext` des Aufrufers wieder her. Bei
  `Task.WhenAll(q1.ExecuteAsync(), q2.ExecuteAsync())` sieht `q2` daher `ownsOperation == false`, findet aber den Zähler
  belegt → Exception.
* `Interlocked.CompareExchange` macht die Prüfung für echte Parallelität (mehrere Threads) atomar.

Abgrenzung:
* Der Guard schützt nur Operationen mit DB-Zugriff. Reine Cache-Operationen (`MakePersistent`, `Delete`, `FindObject`,
  `MakeHollow`, ...) bleiben ungeschützt; dass der PM nicht threadsicher ist, wird weiterhin dokumentiert.
* Ein Callback, der selbst `Task.Run(...)` mit PM-Zugriff startet, erbt den `ExecutionContext` und gilt als
  verschachtelt. Das ist akzeptabel, solange er vor dem Ende der äußeren Operation abgewartet wird; die Doku weist darauf hin.
* Ein von `ISqlPassThroughHandler.ExecuteAsync` zurückgegebener offener Reader wird vom Guard nicht erfasst
  (der Guard endet mit der Rückgabe).
* Kosten: ein `Interlocked`-Aufruf und ein `AsyncLocal`-Zugriff pro äußerer Operation – vernachlässigbar gegenüber dem DB-Roundtrip.

### 4.15 `IAsyncDisposable`

Prüfung aller `Dispose`-/`Close`-Implementierungen in NDO.dll auf Operationen, für die es Async-Methoden gibt:

| Typ | Was passiert in `Dispose`/`Close` | Async-fähige Operation? | Entscheidung |
|---|---|---|---|
| `NDOTransactionScope` | `RollbackTransactions()` → `tx.Rollback()` je offener Transaktion; `CloseConnections()` → `conn.Dispose()` je offener Connection | **ja**: `DbTransaction.RollbackAsync`, `DbConnection.DisposeAsync` | `IAsyncDisposable` |
| `NDODistributedTransactionScope` | `innerScope.Dispose()` (`System.Transactions.TransactionScope`, Rollback der ambient Transaction); `CloseConnections()` → `conn.Close()` | **teilweise**: `DbConnection.CloseAsync` ja; `TransactionScope` hat kein `DisposeAsync` und wird weiterhin synchron disposed | `IAsyncDisposable` (ohnehin über `INDOTransactionScope` vorgegeben) |
| `SqlPassThroughHandler` | `pm.TransactionScope.Dispose()` (Rollback + Close), `TransactionMode` zurücksetzen | **ja** (über den TransactionScope) | `IAsyncDisposable` |
| `PersistenceManager` (`Dispose` → `Close`) | `TransactionScope.Dispose()` (Rollback + Close), `UnloadCache()`, dann `base.Close()` | **ja** (über den TransactionScope) | `IAsyncDisposable` + `CloseAsync` |
| `PersistenceManagerBase` (`Close`) | `ds.Dispose()`, `scope.Dispose()` (der `IServiceScope` der DI), `queryCache.Clear()` | **ja**: Ist der DI-Scope `IAsyncDisposable` (z. B. `AsyncServiceScope` von Microsoft.Extensions.DependencyInjection), muss `DisposeAsync` verwendet werden; ein synchrones `Dispose` wirft dort `InvalidOperationException`, wenn ein Scoped Service nur `IAsyncDisposable` implementiert | Teil von `CloseAsync` |
| `OfflinePersistenceManager` | überschreibt `Dispose()` → `Close()` | wie `PersistenceManager` | Überschreibung entfällt (erbt `DisposeAsync`/`Dispose`) |
| `SqlPersistenceHandler` | `disposeCallback(type, this)` – Rückgabe an den Handler-Pool | nein | `IDisposable` bleibt |
| `NDOMappingTableHandler` | leer | nein | `IDisposable` bleibt |
| `NDOServiceProvider` | leer | nein | `IDisposable` bleibt |
| `NDOConsoleLogger` (Logger und Scope) | keine DB-Operationen | nein | `IDisposable` bleibt |

Umsetzung:

```csharp
// NDOTransactionScope
public async ValueTask DisposeAsync()
{
    await RollbackTransactionsAsync().ConfigureAwait( false );   // tx.RollbackAsync(CancellationToken.None), Fehler wie bisher ignoriert
    await CloseConnectionsAsync().ConfigureAwait( false );       // conn.DisposeAsync()
    this.isInTransaction = false;
}

public void Dispose() => DisposeAsync().ConfigureAwait( false ).GetAwaiter().GetResult();
```

```csharp
// PersistenceManagerBase
public virtual async Task CloseAsync()
{
    if (isClosing) return;
    isClosing = true;
    this.ds.Dispose();
    this.ds = null;
    if (this.scope is IAsyncDisposable asyncScope)
        await asyncScope.DisposeAsync().ConfigureAwait( false );
    else
        this.scope?.Dispose();
    this.queryCache.Clear();
}

public void Close() => CloseAsync().ConfigureAwait( false ).GetAwaiter().GetResult();

public async ValueTask DisposeAsync()
{
    await CloseAsync().ConfigureAwait( false );
    GC.SuppressFinalize( this );
}

public void Dispose() { Close(); GC.SuppressFinalize( this ); }

// PersistenceManager
public override async Task CloseAsync()
{
    if (this.isClosing) return;
    this.isClosing = true;
    await TransactionScope.DisposeAsync().ConfigureAwait( false );
    UnloadCache();
    await base.CloseAsync().ConfigureAwait( false );
}
```

Regeln:
* Es gilt Grundprinzip 1: Die Logik existiert einmal (asynchron), `Dispose()`/`Close()` sind synchrone Wrapper.
  `DisposeAsync` gibt `ValueTask` zurück, weil `IAsyncDisposable` das vorgibt (Ausnahme zu Grundprinzip 3).
* Überschreibpunkt ist `CloseAsync`. `Close`, `Dispose` und `DisposeAsync` sind nicht mehr `virtual`;
  `Dispose(bool)` und der Finalizer bleiben unverändert (der Finalizer schließt nicht).
* Rollback und Schließen erhalten **kein** `CancellationToken` (bzw. `CancellationToken.None`): Ein abgebrochener
  Rollback würde Transaktionen und Connections offen lassen. Deshalb haben auch `AbortTransactionAsync()` und
  `AbortAsync()` keinen Token-Parameter (Ausnahme zu Grundprinzip 4).
* `DisposeAsync`/`CloseAsync` laufen **nicht** durch den Guard (4.14), da `Dispose` nicht werfen soll. Den
  PersistenceManager zu schließen, während noch eine Operation läuft, ist ein Bedienfehler und wird so dokumentiert.
* `NDODistributedTransactionScope.DisposeAsync`: `innerScope.Dispose()` muss im selben logischen Ausführungsfluss wie
  das Erzeugen laufen; mit `TransactionScopeAsyncFlowOption.Enabled` (4.7) ist das bei `await using` gegeben.
* Empfohlene Nutzung in der Doku: `await using var pm = new PersistenceManager();` bzw.
  `await using (var handler = pm.GetSqlPassThroughHandler()) { ... }`.
* Interne synchrone Aufrufer (`AbortTransaction` in `BuildDatabase`) bleiben synchron; in Async-Pfaden wird
  ausschließlich `DisposeAsync`/`AbortTransactionAsync` verwendet.
* `INDOTransactionScope`, `ISqlPassThroughHandler` und `IPersistenceManager` leiten zusätzlich von `IAsyncDisposable` ab.

---

## 5. Breaking Changes (für Release Notes v6)

* Target Frameworks: netstandard2.0/2.1, `net6.0` und `net9.0` entfallen; unterstützt werden `net8.0`, `net10.0`, `net11.0`
  (`NDOInterfaces` zusätzlich `net48`).
* `IPersistenceHandler`: `Update`, `UpdateDeletedObjects`, `ExecuteBatch`, `PerformQuery` → nur noch `...Async`; keine Ableitung von `IRowUpdateListener`.
* `IMappingTableHandler`: `Update`, `FindRelatedObjects` → `...Async`.
* `INDOTransactionScope`: `CheckTransaction`, `Complete`, `GetConnection` → `...Async`; zusätzlich `IAsyncDisposable`.
* `ISqlPassThroughHandler`: zusätzliche Async-Member (`ExecuteAsync`, `BeginTransactionAsync`, `CommitTransactionAsync`) und `IAsyncDisposable`.
* `IQuery`, `IPersistenceManager`: zusätzliche Async-Member (Implementierer außerhalb von NDO müssen sie ergänzen);
  `IPersistenceManager` zusätzlich `IAsyncDisposable`.
* `PersistenceManager.GetClassExtent` und `IPersistenceManager.GetClassExtent` entfallen
  (Ersatz: `pm.Objects<T>()`, `NDOQuery<T>`, `pm.NewQuery`).
* `PersistenceManager.Save`, `LoadData`, `LoadRelation`, `Refresh`, `Abort`, `AbortTransaction`, `Close`, `Dispose`: nicht mehr
  `virtual`; Überschreibpunkt ist die Async-Variante (`CloseAsync` für `Close`/`Dispose`).
* Gleichzeitige Nutzung eines `PersistenceManager` aus mehreren Tasks/Threads löst eine `NDOException` aus (bisher undefiniertes Verhalten).
* `SqlPersistenceHandler.DataAdapter` entfällt.
* `IProvider.NewDataAdapter`, `IProvider.NewCommandBuilder`, `IProvider.RegisterRowUpdateHandler` und `IRowUpdateListener`
  entfallen, ebenso die entsprechenden Member von `NDOAbstractProvider`. Provider (auch von Dritten) müssen ihre
  Überschreibungen von `NewDataAdapter` und `NewCommandBuilder` löschen (4.12).
* `NDOAbstractProvider.GetDatabaseStructure` liest das Schema ohne Adapter (gleiches Ergebnis, 4.12).
* `IProvider`/`NDOAbstractProvider`: `NewConnection` liefert `DbConnection`; `NewSqlCommand`, `GetConnectionId`,
  `AddParameter`, `GetTableNames`, `GetDatabaseStructure` erwarten `DbConnection` bzw. `DbCommand` (4.12.1).
* `IPersistenceHandlerBase.Connection`/`Transaction` und `INDOTransactionScope.GetConnectionAsync`/`GetTransaction`
  verwenden `DbConnection`/`DbTransaction` (4.1).
* Eigene `IPersistenceHandler`-Implementierungen und Mocks in Tests müssen angepasst werden
  (z. B. `NDODLL.Tests/QueryTests/QueryTests.cs`: `Setup(h => h.PerformQueryAsync(...)).ReturnsAsync(new DataTable())`).

---

## 6. Risiken und Fallstricke

| Risiko | Gegenmaßnahme |
|---|---|
| Deadlock beim sync-over-async unter `SynchronizationContext` | Lückenlos `ConfigureAwait(false)`, CA2007 als Fehler; Test mit eigenem Single-Thread-`SynchronizationContext` (siehe 7) |
| Abweichende Semantik zu `Fill` (Spaltennamen, Typen, Merge, fehlende Spalten) | Spezifische Unit-Tests für `DbDataTableFiller`; Integrationstests gegen alle Provider |
| Abweichende Semantik zu `Update` (Parameter-Versionen, Read-Back, Concurrency, AcceptChanges) | Spezifische Unit-Tests für `DbRowUpdater`; bestehende Concurrency-/Collision-Tests |
| Provider implementieren Async nur pseudo-asynchron (z. B. System.Data.SQLite) | Funktional unkritisch, nur kein Skalierungsgewinn |
| Thread-Pool-Starvation durch sync-over-async in Lazy Loading unter Last | Doku: in Server-Anwendungen Async-API verwenden und Relationen vorab laden |
| Callbacks laufen auf ThreadPool-Thread | Doku (siehe 4.8) |
| `System.Transactions` ohne AsyncFlow | `TransactionScopeAsyncFlowOption.Enabled` (4.7) |
| Handler-Pool und Instanz-Commands während `await` | Review des `NDOPersistenceHandlerManager`; Test mit mehreren PMs parallel |
| Gleichzeitige Aufrufe auf demselben PM | Guard (4.14) mit `NDOException` |
| Guard meldet fälschlich Parallelität bei legitimer Verschachtelung | `AsyncLocal`-Besitzmarke (4.14); Tests für Lazy Loading in `OnSaving`, `Refresh` und `LoadRelation` |
| `net11.0`-SDK noch nicht auf allen Build-Umgebungen | `NDO.Build` und CI vorab auf .NET 11 SDK aktualisieren |
| `GetDatabaseStructure` ohne `FillSchema` liefert bei einem Provider ein anderes Schema (v. a. Oracle, Closed Source) | Äquivalenztest je Provider (7, Punkt 9) **vor** dem Entfernen von `NewDataAdapter` |
| Provider bauen gegen das alte `NDOInterfaces`-5.1.0-Paket | Phasenreihenfolge (8): erst `NDOInterfaces` 6.0 (Preview) veröffentlichen, dann Provider umstellen |
| `Dispose()` als sync-over-async-Wrapper unter `SynchronizationContext` | wie alle Wrapper: lückenlos `ConfigureAwait(false)`; Deadlock-Test schließt `Dispose`/`Close` ein |
| Mehrdeutige LINQ-Aufrufe, falls `VirtualTable<T>` `IAsyncEnumerable<T>` direkt implementiert | musterbasiertes `GetAsyncEnumerator` + `AsAsyncEnumerable()` (4.10) |

---

## 7. Teststrategie

1. **Bestehende Tests** (`NDODLL.Tests`, `IntegrationTests`) müssen grün bleiben – sie decken über die
   synchronen Wrapper automatisch die neuen Async-Pfade ab. Einzige inhaltliche Anpassung: `GetClassExtent`-Aufrufe ersetzen (4.8).
2. **Mocks anpassen** auf die Async-Signaturen.
3. **Neue Unit-Tests** für `DbDataTableFiller` und `DbRowUpdater` (z. B. mit Sqlite in-memory):
   case-insensitive Spalten, fehlende Spalten, PK-Merge, Typkonvertierung, Insert mit Read-Back, Concurrency (0 Rows),
   Deleted → Detached, Original-Werte in WHERE-Parametern.
4. **Async-Varianten der Kern-Tests**: für Query, Aggregate, DeleteDirectly, Save (Insert/Update/Delete/Mapping-Tabellen,
   Autoincrement, Kollisionen), `LoadDataAsync`, `LoadRelationAsync`, `RefreshAsync` und `ISqlPassThroughHandler.ExecuteAsync`
   (mit und ohne Reader, mit Transaktion) je ein `async Task`-Test.
5. **Deadlock-Test**: synchrone `Execute()`/`Save()` innerhalb eines Single-Thread-`SynchronizationContext`
   (z. B. `AsyncContext` aus Nito.AsyncEx oder einer kleinen eigenen Implementierung) mit Timeout.
6. **Postgres-Autoincrement**: Test, dass nach `Save` die Id gesetzt ist (deckt den bisher toten `RowUpdated`-Pfad ab).
7. **Distributed Transactions**: `NDODistributedTransactionScope` mit `SaveAsync` über mehrere `await`s.
8. **Guard**:
   * `Task.WhenAll(q1.ExecuteAsync(), q2.ExecuteAsync())` auf demselben PM → `NDOException`.
   * Zwei Threads, die gleichzeitig `Save()` bzw. `Execute()` aufrufen → genau einer erhält die Exception.
   * Lazy Loading in `OnSavingEvent`, `Refresh`, `LoadRelation` → **keine** Exception.
   * Nach einer Exception innerhalb einer Operation ist der PM wieder benutzbar (Guard freigegeben).
   * Zwei verschiedene PMs parallel → keine Exception.
9. **Schema-Äquivalenz `GetDatabaseStructure`** (neues Testprojekt, z. B. `NDOInterfaces.Tests`), parametrisiert über alle
   sechs Provider (Sqlite immer, die übrigen über Connection-Strings aus Umgebungsvariablen, in der CI z. B. per
   Container für SQL Server, Oracle Free, MySQL und PostgreSQL). Testdatenbank mit Tabellen mit Autoincrement-PK,
   GUID-PK, zusammengesetztem PK, ohne PK, Nullable-/Not-Null-Spalten, Strings mit Länge, Dezimal-, Datums- und Binärspalten.
   Verglichen werden je Tabelle Spaltennamen und -reihenfolge, `DataType`, `AllowDBNull`, `AutoIncrement`, `MaxLength`,
   `ReadOnly`, `Unique` und `PrimaryKey` zwischen alter (`FillSchema`) und neuer Implementierung (`DataTable.Load`).
   Der Test wird geschrieben und muss grün sein, **solange `NewDataAdapter` noch existiert**; danach vergleicht er gegen
   die festgehaltenen Erwartungswerte weiter (Regressionstest).
10. **Dispose**: `await using` von PM, PassThroughHandler und TransactionScope mit offener Transaktion → Rollback
    ausgeführt, Connections geschlossen; synchrones `Dispose()` im Single-Thread-`SynchronizationContext` ohne Deadlock;
    PM mit einem `AsyncServiceScope`, dessen Scoped Service nur `IAsyncDisposable` implementiert → `DisposeAsync` wirft nicht.
11. **`await foreach`** über `VirtualTable<T>` inkl. Abbruch über `CancellationToken` und `AsAsyncEnumerable()`;
    Kompiliertest, dass `vt.ToList()`/`vt.Any()` mit `using System.Linq;` unter `net10.0` eindeutig bleiben.

---

## 8. Umsetzungsreihenfolge

Jede Phase ist für sich kompilier- und testbar.

0. **Target Frameworks**: alle Laufzeitprojekte auf `net8.0; net10.0; net11.0`, netstandard-Bedingungen in den
   `csproj`-Dateien entfernen, `NDO.Build`/CI auf .NET 11 SDK; `GetClassExtent` entfernen und Tests migrieren.
1. **Infrastruktur**: `DbDataTableFiller`, `DbRowUpdater` inkl. Unit-Tests; `.editorconfig` mit CA2007.
   Neue Implementierung von `NDOAbstractProvider.GetDatabaseStructure` (4.12) und Schema-Äquivalenztest (7, Punkt 9)
   gegen alle sechs Provider – zu diesem Zeitpunkt existiert `NewDataAdapter` noch als Vergleichsbasis.
2. **Handler-Ebene**: `IPersistenceHandler`, `IMappingTableHandler` auf Async umstellen; `SqlPersistenceHandler` und
   `NDOMappingTableHandler` ohne `DbDataAdapter`. Übergangsweise rufen `PersistenceManager`/`NDOQuery` die Handler per
   `GetAwaiter().GetResult()` auf → alle bestehenden Tests laufen gegen die neue Handler-Implementierung.
2a. **Provider-Schnittstelle** (Voraussetzung: Äquivalenztest aus Phase 1 für alle Provider grün):
   `IProvider`/`NDOAbstractProvider` auf `DbConnection`/`DbCommand` umstellen (4.12.1) und die Aufrufer im
   `ClassGenerator` und in `UnitTests/ExecuteSqlBatch` anpassen; NDO.dll intern auf `Db*`-Typen umstellen (4.1);
   `NewDataAdapter`, `NewCommandBuilder`, `RegisterRowUpdateHandler` und `IRowUpdateListener` aus `NDOInterfaces` entfernen;
   toten `#if DontUseDataSets`-Code im `ClassGenerator` entfernen; `NDOInterfaces` 6.0 (Preview) als Paket veröffentlichen;
   alle sechs Provider auf diese Paketversion und die neuen Target Frameworks umstellen, die Signaturen
   anpassen und ihre Überschreibungen von `NewDataAdapter`/`NewCommandBuilder` löschen – alles in einem Schritt, damit
   jeder Provider nur einmal angefasst wird. Kontrolle: `grep -rn "DataAdapter\|CommandBuilder" --include=*.cs`
   liefert keine Treffer mehr (außer ggf. im `ClassGenerator`-XSD-Pfad, der keinen Adapter verwendet).
3. **Transaktionen**: `INDOTransactionScope` + beide Implementierungen asynchron; `PersistenceManager.CheckTransactionAsync`,
   `CheckEndTransactionAsync`; `ISqlPassThroughHandler`/`SqlPassThroughHandler` asynchron; `DisposeAsync` für beide
   TransactionScopes und den PassThroughHandler (4.15).
4. **Abfragen und Laden**: `NDOQuery<T>` vollständig async, synchrone Wrapper; `IQuery`; `VirtualTable<T>`.
   `LoadDataAsync`, `LoadRelationAsync`, `RefreshAsync` (öffentlich), `QueryRelatedObjectsAsync` (intern).
5. **Speichern**: `SaveAsync`, `UpdateTypesAsync`, Mapping-Tabellen, `EndSaveAsync`; `Save` als Wrapper;
   `IPersistenceManager`, `OfflinePersistenceManager`; `AbortAsync`, `AbortTransactionAsync`, `CloseAsync`, `DisposeAsync`.
6. **Guard** (4.14) an allen Eintrittspunkten inkl. Tests.
7. **Aufräumen**: Übergangs-`GetResult()`-Aufrufe aus Phase 2 entfernen (es darf im Inneren keine mehr geben – nur in
   den synchronen Eintrittspunkten), XML-Doku und Release Notes.

Kontrolle nach Phase 7:
`grep -rn "GetAwaiter().GetResult()" NDODLL` darf nur Treffer in den synchronen Eintrittspunkten liefern
(`NDOQuery`, `VirtualTable`, `PersistenceManager.Save/LoadData/LoadRelation/Refresh/Close/...`, `SqlPassThroughHandler`,
`Dispose`-Wrapper). Da `PersistenceManager.cs` ISO-8859-kodiert ist, muss `grep -a` verwendet werden, sonst wird die Datei
ohne Meldung übersprungen.

---

## 9. Entscheidungen

1. `LoadData`, `LoadRelation`, `Refresh` erhalten öffentliche `Async`-Varianten (4.8). `GetClassExtent` wird entfernt.
   `FindObject` bleibt synchron ohne `await` (kein DB-Zugriff).
2. `ISqlPassThroughHandler` wird in diesem Schritt mit umgestellt (4.11).
3. netstandard wird nicht mehr unterstützt; Targets sind `net8.0`, `net10.0`, `net11.0` (2.4).
   `NDOInterfaces` wird zusätzlich weiterhin für `net48` gebaut.
4. Ein Guard gegen parallele Nutzung eines `PersistenceManager` wird eingebaut (4.14).
5. `NewDataAdapter` (sowie `NewCommandBuilder`, `RegisterRowUpdateHandler`, `IRowUpdateListener`) wird vollständig
   entfernt; `GetDatabaseStructure` nutzt `ExecuteReader(SchemaOnly | KeyInfo)` + `DataTable.Load`, abgesichert durch
   einen Äquivalenztest je Provider (4.12, 7).
6. `VirtualTable<T>` unterstützt `await foreach` über ein musterbasiertes `GetAsyncEnumerator` und `AsAsyncEnumerable()` (4.10).
7. `IAsyncDisposable` für alle Typen, deren `Dispose` DB-Operationen ausführt: `INDOTransactionScope` (beide
   Implementierungen), `ISqlPassThroughHandler`, `IPersistenceManager`/`PersistenceManager`. Handler ohne DB-Operationen
   im `Dispose` behalten nur `IDisposable` (4.15).

8. `IProvider.NewConnection`/`NewSqlCommand` liefern bzw. erwarten `DbConnection`/`DbCommand`; die übrigen
   `IProvider`-Member mit Connection-/Command-Parametern werden mit umgestellt. NDO.dll arbeitet intern durchgängig
   mit `DbConnection`/`DbCommand`/`DbTransaction` (4.1, 4.12.1).
9. Kein `BuildDatabaseAsync`: `BuildDatabase` wird nur in kleinen Beispielprogrammen ohne Tasks genutzt und bleibt synchron.
