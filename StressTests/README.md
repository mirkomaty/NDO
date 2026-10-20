# NDO Stress Test

Checks whether NDO handles many concurrent requests without errors. Many workers create, change, verify and
delete objects in parallel. They use only the async API of `PersistenceManager` and `VirtualTable<T>`.
Each worker creates its own `PersistenceManager` instance for every step.

## Projects

| Project | Contents |
|---|---|
| `StressTestClasses` | Persistent classes `Customer`, `SalesOrder`, `OrderLine`, `Tag`. They are enhanced by the NDOEnhancer during the build, which also creates `NDOMapping.xml`. |
| `StressTest` | Console program that runs the test. References all providers. |

The relations cover the most important mapping variants:

* `SalesOrder.customer`: n:1, foreign key in `SalesOrder`
* `SalesOrder.lines` ↔ `OrderLine.order`: bidirectional 1:n composition, foreign key in `OrderLine`
* `SalesOrder.tags`: n:m via the mapping table `relSalesOrderTag`. The tags are created once and shared
  by all workers.

## Running the test

```
cd StressTests/StressTest
dotnet run                                    # Sqlite with the values from appsettings.json
dotnet run -- --provider Postgre --threads 100 --iterations 20
dotnet run -- -p SqlServer -t 200 -d 120      # 200 workers for 2 minutes
dotnet run -- -p MySql -c "Server=db;Database=ndostresstest;Uid=ndo;Pwd=secret"
dotnet run -- --help
```

Supported providers: `Sqlite`, `SqlServer`, `MySql`, `MySqlConnector`, `Postgre`.

The connection strings are in the `ConnectionStrings` section of `appsettings.json`, one per provider.
So `--provider <name>` is all it takes to switch the provider. Local connection strings with passwords belong
in an `appsettings.local.json` next to `appsettings.json` (ignored by git) or in environment variables:

```
export NDOSTRESS_ConnectionStrings__Postgre="Host=db;Database=ndostresstest;Username=ndo;Password=secret"
```

Precedence: `appsettings.json` < `appsettings.local.json` < environment variables `NDOSTRESS_*` < command line.

The database must exist. The tables are dropped and recreated before the test (`--recreateSchema false`
turns this off). The DDL is created by the `ISqlGenerator` of the respective provider, just like the NDOEnhancer does.

Exit codes: 0 = success, 1 = errors, 2 = cancelled (Ctrl+C), 3 = configuration or setup error.
This makes the test usable in a script or a CI pipeline.

## Steps of an iteration

Each worker runs its iterations one after another, each step with a new `PersistenceManager`:

1. **Create:** Create a customer and a sales order with lines and two tags (`FindObject`), `SaveAsync`.
2. **Read and change:** Load the order with `VirtualTable.SingleAsync`, `LoadRelationAsync`, `LoadDataAsync`;
   change the status, change, remove and add a line, replace a tag, change the customer, `SaveAsync`.
3. **Verify:** Check with `ToListAsync`, `CountAsync`, `SumAsync`, `FirstOrDefaultAsync`, `await foreach`
   (condition on a relation) and `RefreshAsync` that all changes have been saved.
4. **Query:** Queries over the data of all workers (`CountAsync`, `OrderByDescending`/`Take`).
5. **Delete:** Delete the order with `Delete` + `SaveAsync` (the composition deletes the lines and the
   mapping table entries), delete the customer with `DeleteDirectlyAsync`.
6. **Verify deletion:** Check that nothing is left.

Any deviation causes a `VerificationException`. After the run, the program compares the row counts of all
tables (including the mapping table) with the counts before the run. At the end it prints statistics of the
operations (count, average and maximum duration) and, if errors occurred, a grouping by step and exception type
as well as the first errors with stack traces.

## Options

| Option | Default | Meaning |
|---|---|---|
| `--threads`, `-t` | 32 | Number of concurrent workers |
| `--iterations`, `-i` | 25 | Iterations per worker |
| `--duration`, `-d` | 0 | Run time in seconds instead of a fixed number of iterations |
| `--transactionMode` | Optimistic | `Optimistic` or `Pessimistic` |
| `--isolationLevel` | NDO default | e.g. `ReadCommitted`, `Serializable`. For Sqlite `Serializable` by default, see below |
| `--sharedMapping` | true | All PersistenceManagers share one `NDOMapping` object |
| `--lines`, `--tags` | 5, 8 | Lines per order, number of shared tags |
| `--recreateSchema` | true | Recreate the tables before the test |
| `--dropSchema` | false | Drop the tables after a successful test |
| `--stopOnFirstError` | false | Stop all workers at the first error |

## Notes on the databases

* **Sqlite** allows only one writer at a time. A transaction that starts with a read
  (`BEGIN DEFERRED`, corresponds to `ReadCommitted`) can't become a write transaction while another
  connection is writing; Sqlite then reports `database is locked` immediately instead of waiting. Therefore the
  test uses `Serializable` (`BEGIN IMMEDIATE`) for Sqlite by default. Use `--isolationLevel ReadCommitted` to see
  the effect. The database file is created in the current directory.
* **Connection pools:** With many workers the maximum pool size must be large enough (500 in `appsettings.json`).
  Otherwise there are timeouts when acquiring a connection, which have nothing to do with NDO.
* So far the test has only been run with Sqlite. For Postgre, the DDL shows that the provider's generator creates
  the type `date` for `DateTime` and no primary key for identity columns. This doesn't matter for the test,
  because it doesn't check times of day.
