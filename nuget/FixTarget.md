# NDO.Build: Enhancer direkt aus dem MSBuild-Target aufrufen

## Ausgangslage

`NDO.Build.targets` rief bisher den MSBuild-Task `NDO.BuildTask.NDOEnhancer` aus dem
Assembly `NDOEnhancer.BuildTask.dll` auf. Dieses Assembly ist ein .NET-Framework-4.8-Assembly
und braucht zum Bauen MSBuild aus Visual Studio. Es funktioniert also nur unter Windows.

Der BuildTask hat mehr getan, als nur die Applikation aufzurufen:

1. Plattform `x86` abgelehnt.
2. Eine Standard-`.ndoproj` angelegt, wenn keine vorhanden war.
3. `NDOEnhancer.exe "<ndoproj>" <TargetFramework>` gestartet.
4. Zeilen in stderr, die mit `Error:` beginnen, als MSBuild-Fehler gemeldet.
5. Nach einem erfolgreichen Lauf die angereicherte DLL von `obj/.../ndotemp/` nach `bin/...` kopiert.
   Die Applikation selbst hat nur nach `obj` und die PDB nach `bin` kopiert.

## Lösung

### Das Target (`nuget/NDO.Build.targets`)

```xml
<PropertyGroup>
  <NdoProjFullPath Condition="'$(NdoProjFullPath)' == ''">$(MSBuildProjectDirectory)/$(MSBuildProjectName).ndoproj</NdoProjFullPath>
  <NdoEnhancerPath>$(MSBuildThisFileDirectory)NDOEnhancer.dll</NdoEnhancerPath>
</PropertyGroup>

<Target Name="NDOEnhancer" AfterTargets="Build" Condition="'$(TargetFramework)' != ''">
  <Error Condition="'$(Platform)' == 'x86'" Text="Platform x86 is not supported. Use NDO &lt;= 4." />
  <Exec Command="dotnet &quot;$(NdoEnhancerPath)&quot; &quot;$(NdoProjFullPath)&quot; $(TargetFramework)"
        WorkingDirectory="$(MSBuildProjectDirectory)"
        CustomErrorRegularExpression="^Error:" />
</Target>
```

- Der Aufruf `dotnet NDOEnhancer.dll` läuft auf jedem Betriebssystem. Der Apphost (`NDOEnhancer.exe`
  bzw. `NDOEnhancer`) passt dagegen nur zu dem System, auf dem das Paket gebaut wurde.
- Die Applikation liegt durch die nuspec im Ordner `build` des Pakets, also neben der targets-Datei.
- `CustomErrorRegularExpression` ersetzt die Fehlerauswertung des BuildTask. Ein Exit-Code ≠ 0
  lässt den Build ohnehin fehlschlagen.
- `$(MSBuildProjectName)` statt `$(ProjectName)`: `ProjectName` wird erst in
  `Microsoft.Common.CurrentVersion.targets` gesetzt und stammt ursprünglich aus Visual Studio.
- `NdoProjFullPath` lässt sich im Projekt überschreiben.
- Bei Multi-Targeting-Projekten hat der äußere Build kein `TargetFramework`. Die Bedingung
  sorgt dafür, dass der Enhancer nur in den inneren Builds läuft.

### Logik aus dem BuildTask in die Applikation verlagert (`EnhancerMain.cs`)

- Fehlt die `.ndoproj`, legt der Enhancer eine Standarddatei an, statt abzubrechen
  (`CreateNDOProjFile`). Wie im BuildTask sind dort `bin/Debug/<tfm>` und `obj/Debug/<tfm>`
  fest eingetragen, jetzt mit `/` als Trenner.
- Die angereicherte DLL wird von `ndotemp` nach `ProjectDescription.BinFile` kopiert.
  Das Kopieren in `PureBusinessClasses.csproj` ist damit überflüssig, schadet aber nicht.

### Paket und Build-Skript

- `nuget/NDO.Build.nuspec`: `NDOEnhancer.BuildTask.dll` wird nicht mehr ins Paket aufgenommen.
- `Make/packages.json`: Der msbuild-Schritt für `NDOEnhancer.BuildTask` ist aus `buildFirst` entfernt.
  NDO.Build braucht damit kein Visual Studio mehr.
- `Make/HowToBuild.txt`: Der Hinweis auf Visual Studio und MSBuild für den BuildTask ist gelöscht.
- Das Verzeichnis `NDOEnhancer.BuildTask` wird nicht mehr gebraucht, ist aber noch vorhanden.

## Weitere Probleme des Enhancers unter Linux

### Pfade in der `.ndoproj`

`.ndoproj`-Dateien enthalten Windows-Pfade wie `bin\Debug\$(TargetFramework)\X.dll`. Unter Linux
ist `\` kein Pfadtrenner, deshalb fand der Enhancer die Dateien nicht. Die Meldung war
`Can't find file .../obj\Debug\net10.0\/bin\Debug\...`, und der Exit-Code war trotzdem 0.

Lösung: `ProjectDescription.AbsolutePath` ersetzt `\` durch `Path.DirectorySeparatorChar`.
Unter Windows ändert das nichts.

### Optionen von ildasm und ilasm

Unter Linux lesen `ildasm` und `ilasm` alles, was mit `/` beginnt, als Dateipfad:

- `ildasm /LINENUM /UNICODE x.dll /OUT=x.il` meldet `MULTIPLE INPUT FILES SPECIFIED` und erzeugt
  keine `.il`-Datei. Der Enhancer bricht dann mit `FileNotFoundException ... .org.il` ab.
- `ilasm /DLL /QUIET ...` versucht, `/DLL.il`, `/QUIET.il` usw. zu öffnen, und meldet `FAILURE`.

Beide Tools akzeptieren laut ihrer Hilfe `-` oder `/` als Optionszeichen
(„Option key is '-' or '/', options are recognized by first 3 characters“).
Lösung: In `Dasm.cs` und `Asm.cs` verwenden alle Optionen jetzt `-`, ohne Unterscheidung
nach Betriebssystem. Statt `/OUTFILE=` steht jetzt die dokumentierte Option `-OUTPUT=`.

## Nebeneffekt: Warnungen im Build-Log

Der BuildTask hat stdout des Enhancers nur als Nachricht geloggt. Bei `Exec` erkennt MSBuild Zeilen
im Format `warning : ...` als Warnungen. Hinweise wie `can't determine ordinal for field ...`
erscheinen deshalb jetzt als MSBuild-Warnungen, bei `PureBusinessClasses` sind es rund 70.
Sie sind nicht neu, nur jetzt sichtbar. Abschalten ließe sich das mit
`IgnoreStandardErrorWarningFormat="true"` am `Exec`. Dann muss die Fehlererkennung allein über
`CustomErrorRegularExpression` und den Exit-Code laufen.

## Test

Unter Linux mit .NET SDK 11.0.100:

1. `NDOEnhancer` in Release gebaut.
2. `IntegrationTests/PureBusinessClasses` in ein temporäres Verzeichnis kopiert. Das Target
   `CustomAfterBuild` durch `<Import Project=".../nuget/NDO.Build.targets" />` ersetzt und
   `NdoEnhancerPath` per `-p:` auf die gebaute `NDOEnhancer.dll` gesetzt.
3. `dotnet build`: 0 Fehler. Die DLL in `bin` ist identisch mit der in `ndotemp`.
   Disassembliert enthält sie `IPersistenceCapable` und `NDOEnhanced`, und `NDOMapping.xml` wurde erzeugt.

Fehler des Enhancers erscheinen als MSBuild-Fehler und lassen den Build fehlschlagen
(geprüft mit dem ildasm-Fehler vor dessen Korrektur).

## Offen

- Nicht unter Windows getestet, auch nicht die `-`-Optionen für ildasm und ilasm.
- Das von `Make/build.cs` gepackte NuGet-Paket ist nicht end-to-end getestet.
  Getestet wurde nur das direkte Einbinden der targets-Datei.
- Die Integrationstests sind nicht gelaufen, denn sie brauchen eine Datenbank.
- Die Standard-`.ndoproj` trägt immer die Debug-Pfade ein, auch bei einem Release-Build.
  So war es schon im BuildTask.
- Auf dem Rechner, der das Projekt baut, muss die .NET-Runtime installiert sein, für die der
  Enhancer gebaut ist (derzeit `net11.0`).
