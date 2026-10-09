# NDO Build Process – Concept for Independent Package Versioning

## 1. Context

NDO ships as a set of NuGet packages. Other projects inside and outside this
repository consume them. The packages evolve independently. When one package changes:

1. It needs a new version. That means `Version`, `AssemblyVersion` and `FileVersion` in its csproj.
2. Every project that depends on it must reference the new version.
3. Every dependent *package* must get a new version of its own.
4. Steps 2 and 3 repeat for the dependents' dependents.

The repository contains two kinds of projects, and the build handles both:

1. **Package projects (producers).** They produce the NDO packages
   (NDOInterfaces, NDO.dll, the providers, NDO.Build, …).
2. **Test projects (consumers).** They only consume the packages, through
   `PackageReference` or `ProjectReference`. They are never packed. Tools that
   consume packages (TestGenerator, SimpleMappingTool) are treated the same way.

Until now, a complete build was described by the MSBuild script `Make/NDO.proj`,
and versions were patched by `Tools/PatchNdoVersion`. That tool assumed one
version for all packages NDO.dll depends on and never touched a package's own
version. Both are replaced by the script described here.

Out of scope:
- **The VSIX** (`NDOPackage`) and the projects only it needs
  (`UISupport/NDO.UISupport`, the provider `*UISupport` projects) are not built.
  Their `PackageReference`s are still kept up to date.
- **Pre-release versions** (`6.1.0-beta1`) are not supported. All versions are
  `major.minor.patch`.

## 2. Current State

### 2.1 Package projects

| Package id | Project | Version | Assembly/FileVersion |
|---|---|---|---|
| NDOInterfaces | `NDOInterfaces/NDOInterfaces.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.Mapping | `NDO.Mapping/NDO.Mapping/NDO.Mapping.csproj` | 6.0.0 | **6.0.0** (3 parts) |
| NDO.ProviderFactory | `NDO.ProviderFactory/NDO.ProviderFactory/NDO.ProviderFactory.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.SchemaGenerator | `NDO.SchemaGenerator/NDO.SchemaGenerator/NDO.SchemaGenerator.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.dll | `NDODLL/NDO.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.JsonFormatter | `NdoJsonFormatter/NdoJsonFormatter/NDO.JsonFormatter.csproj` | 5.1.1 | 5.1.1.0 |
| ndo.mysql, ndo.mysqlconnector, ndo.oracle, ndo.postgre, ndo.sqlite, ndo.sqlserver | `Provider/*/NDO.*/NDO.*.csproj` | 5.1.0 | 5.1.0.0 |
| NDO.Build | `nuget/NDO.Build.nuspec` (packed with `nuget.exe`) | **6.0.1.0** (4 parts) | – |

Notes:
- **Versions are scattered.** Every version is hard-coded in its project file.
- **NDO.Build and NDOEnhancer are coupled.** NDO.Build packages the binaries of
  `NDOEnhancer/NDOEnhancer` (Version 6.0.1.0) and `NDOEnhancer.BuildTask`. The
  NDOEnhancer version must equal the NDO.Build version.

### 2.2 Test projects

| Project | Kind | NDO references |
|---|---|---|
| `IntegrationTests/IntegrationTests` | test | Package: `NDO.JsonFormatter` 5.1.0 · Project: NDO.dll, ndo.sqlserver, PureBusinessClasses |
| `IntegrationTests/PureBusinessClasses` | library of IntegrationTests | Package: `ndo.dll` 5.1.1 |
| `NdoJsonFormatter/FormatterUnitTests` | test | Package: `ndo.dll` 5.1.1, `ndo.sqlserver` 5.1.0 · Project: NDO.JsonFormatter |
| `NDODLL.Tests/NdoDllUnitTests`, `QueryTests`, `AsyncTests` | test | Project: NDO.dll and providers |
| `NDOEnhancer/Ecma335Tests` | test | Project: Ecma335 (no package) |
| `UnitTestGenerator/UnitTests` (+ `PersistentClasses`) | test | Package: `NDO.dll`, `NDOInterfaces`, `NDO.SqlServer`, `NDO.Build` 5.0.0 |
| `UnitTestGenerator/TestGenerator` | tool | Package: `NDO.Mapping`, `NDO.ProviderFactory` 5.0.0 |
| `SimpleMappingTool/Mapping.csproj` | tool | Package: `NDO.mapping` 5.1.1 |
| `NDOEnhancer/PersistentEnhancerTestClasses` | test classes | Package: `ndo.dll` 5.0.0 – targets net6.0/netstandard2.x, which NDO 6 no longer supports |

### 2.3 How dependencies are expressed

**Package projects use PackageReferences.** A package project references
another NDO package only through a `PackageReference`, never through a
`ProjectReference`. Each package is built against a released version of its
dependencies, which `Make/build.cs` produced before and collected in
`BuiltPackages`. These references form the core graph:

```
NDOInterfaces
 ├── NDO.Mapping ── NDO.SchemaGenerator ─┐
 ├── NDO.ProviderFactory ────────────────┤
 ├───────────────────────────────────────┴── NDO.dll ── NDO.JsonFormatter
 ├── ndo.<provider> (6x)
 └── NDOEnhancer (also Mapping, ProviderFactory, SchemaGenerator) ── NDO.Build
```

Projects that are no packages themselves (e.g. `NDOEnhancer/Ecma335`) are still
referenced with a `ProjectReference`. NDOEnhancer counts as a package project,
because its output is packed into NDO.Build.

Because the version is written in the `PackageReference`, the script must
patch these references whenever a dependency gets a new version (5.3, step 5).
The dependent package also needs its own version bump, because its nuspec
then points to a different dependency version.

**Test projects and tools use ProjectReferences where possible.** The tests
in `NDODLL.Tests` and `IntegrationTests` (with PureBusinessClasses) reference
NDO.dll, NDO.JsonFormatter and the providers through `ProjectReference`s, so
changes to that source take effect without a package build. Some test projects
and tools (2.2) and the provider UISupport projects still use PackageReferences. Package ids are spelled with inconsistent casing
(`ndo.dll`, `NDO.dll`, `NDO.mapping`), so all matching is case-insensitive.

**Mixing both kinds of references.** A test project with a `ProjectReference`
to NDO.dll gets the dependencies of NDO.dll (NDOInterfaces, NDO.Mapping, …)
transitively as *packages* from `BuiltPackages`. If the test project
also references the project of such a dependency (directly or through another
project), NuGet resolves that id to the **project** and ignores the package
with the same id. Two consequences:

- A source change in NDOInterfaces reaches a test only if the test (directly or
  transitively) has a `ProjectReference` to `NDOInterfaces.csproj`. Otherwise the
  test sees the package version until the script has built a new package.
- NDO.dll itself is always compiled against the *package* of NDOInterfaces.
  A change in NDO.dll that needs a new API of NDOInterfaces can only be compiled
  after NDOInterfaces was built as a package (`dotnet run Make/build.cs`).
  The project version and the version in the `PackageReference` must be equal
  (the script guarantees this); otherwise NuGet reports a downgrade (NU1605).

### 2.4 Problems with the old build

- **The patch step was broken.** `Make/NDO.proj` called `PatchNdoVersion -i` on
  projects that no longer have an `NDOInterfaces` PackageReference.
- **PatchNdoVersion was limited:** it only searched the first `ItemGroup` with a
  `PackageReference`, reformatted the file through `XDocument.Save`, and
  couldn't bump a project's own version.
- **Copying to `BuiltPackages` was Windows-only and incomplete.** Per-project
  `PostBuild` targets used `cmd copy`. The providers and NDO.JsonFormatter
  didn't copy at all, and NDO.dll copied no `.snupkg`.
- **Some projects can't be built with `dotnet build`:** `NDOPackage` (VSIX),
  `NDOEnhancer.BuildTask` (old-style, GAC references to
  `Microsoft.Build.Utilities.v4.0`) and `UISupport/NDO.UISupport` (old-style).
- **The NuGet package source pointed to the wrong place.** The user-level source
  `NDO` is `C:\Projekte\NDO\BuiltPackages`, but the repository lives on `D:`.
  That folder exists and contains stale packages, which could be restored
  instead of fresh builds. The repository had no `nuget.config`.

## 3. Requirements

1. **One place to enter versions.** The developer enters the new version of a
   package in one single file.
2. **Producer patching.** The script sets `Version`, `AssemblyVersion` and
   `FileVersion` in that package's csproj, or the version in its nuspec.
3. **Consumer patching.** Every `PackageReference` to an NDO package is set to
   the version being built, in all csproj files: in package projects (like
   NDO.JsonFormatter), test projects and tools.
4. **Propagation.** Every package that depends on a changed package, directly
   or transitively, automatically gets a new version. Then steps 2 and 3 apply
   to it as well.
5. **Building packages.** Package projects are built with `dotnet build <csproj>`,
   in dependency order.
6. **Collection.** All resulting `.nupkg` / `.snupkg` files end up in `BuiltPackages`.
7. **Building and testing consumers.** After the packages, the test projects
   that depend on a rebuilt package are built against the new packages.
   On request they are also run with `dotnet test`.

## 4. Choice of Tool

### 4.1 Is MSBuild the right tool?

**No, not for orchestration.** MSBuild is a declarative item/target engine. It
is excellent for describing *how to build one project*. It is a poor fit for
what this script must do:

- **Graph logic.** The script must read all project files, build a dependency
  graph, compute a transitive closure and sort it topologically. In MSBuild
  this means inline tasks or a custom task assembly, which is effectively C#
  anyway, just harder to debug.
- **Conditional state changes.** Rules like "bump the patch version of X only if
  X itself was not bumped manually" lead to deeply nested conditions.
- **Editing XML in place.** `XmlPoke` rewrites the file and can't easily target
  attributes on specific items without reformatting.
- **Diagnostics.** It's hard to produce a readable dry-run report.

MSBuild stays the right tool *inside* each csproj. It is no longer the orchestrator.

### 4.2 Alternatives

| Option | Pros | Cons |
|---|---|---|
| **MSBuild orchestration file** (old state) | No new runtime | Weak at graph and conditional logic; painful XML patching; hard to debug |
| **PowerShell 7 script** | Good XML support; easy process calls | Different language from the code base; `[xml]` reformats unless handled carefully; PS 5.1 vs. 7 differences |
| **C# file-based app** (`dotnet run build.cs`, .NET 10+) | Same language as NDO; `XDocument`, `System.Text.Json` available; no `.csproj` needed; debuggable; cross-platform | Requires the .NET 10 SDK (already required by the net10.0/net11.0 targets) |
| **Cake / NUKE** | Mature build DSLs in C# | Extra dependency and learning curve; heavyweight for one repository |
| **Central Package Management** (`Directory.Packages.props`) | All `PackageReference` versions in one file | Only covers consumers, not the producers' own versions or propagation. It **complements** a script and doesn't replace it |

### 4.3 Decision

A **C# file-based app**: `Make/build.cs`, run with `dotnet run Make/build.cs`.

- It is the same language as the code base and replaces PatchNdoVersion and `NDO.proj`.
- It needs no build project, bootstrapper or package reference. Since
  pre-release versions are not supported, a small `major.minor.patch` type is
  enough; `NuGet.Versioning` is not needed.
- It has a `--dry-run` mode with a clear report.

## 5. Design

### 5.1 Version manifest – the single source of truth

`Make/packages.json` lists every package that the build produces and every
consumer the build compiles. It is the **only** file the developer edits to
release a new version. Paths are relative to the repository root, except
`packageSource`, which is relative to the manifest.

```json
{
  "packageSource": "../BuiltPackages",
  "configuration": "Release",
  "propagation": "patch",
  "exclude": [ "Tutorial", "ClassGenerator", "UnitTests" ],
  "pinned": [ "NDOEnhancer/PersistentEnhancerTestClasses/PersistentEnhancerTestClasses.csproj" ],
  "packages": [
    { "id": "NDOInterfaces", "project": "NDOInterfaces/NDOInterfaces.csproj", "version": "6.0.0" },
    { "id": "NDO.dll",       "project": "NDODLL/NDO.csproj",                  "version": "6.0.0" },
    …
    { "id": "NDO.Build", "project": "nuget/NDO.Build.nuspec", "builder": "nuget", "version": "6.0.1",
      "versionFollowers": [ "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj" ],
      "buildFirst": [
        { "project": "Tools/MakeEnhancerDate/MakeEnhancerDate.csproj", "configuration": "Release" },
        { "project": "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj", "configuration": "Release" },
        { "project": "NDOEnhancer.BuildTask/NDOEnhancer.BuildTask/NDOEnhancer.BuildTask.csproj",
          "builder": "msbuild", "configuration": "Release" } ] }
  ],
  "enhancer": "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj",
  "consumersBuildFirst": [
    { "project": "Tools/MakeEnhancerDate/MakeEnhancerDate.csproj", "configuration": "Release" },
    { "project": "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj", "configuration": "Debug" }
  ],
  "consumers": [
    { "project": "IntegrationTests/IntegrationTests/IntegrationTests.csproj", "test": true },
    { "project": "SimpleMappingTool/Mapping.csproj" },
    …
  ]
}
```

- **`packages`** – the producers. Dependencies are **not** listed. They are
  derived from the project files, so they can never get out of sync.
  - `builder`: `dotnet` (default), `nuget` (nuspec packed with `nuget/NuGet.exe`)
    or `msbuild` (Visual Studio's MSBuild, found via `vswhere`).
  - `versionFollowers`: projects that get the same version but are not packed
    themselves, like NDOEnhancer.
  - `buildFirst`: projects built before the package. Their dependencies count
    as dependencies of the package, so NDO.Build is bumped whenever a package
    bundled with the enhancer changes.
  - `configuration` (in `buildFirst` entries and consumers): overrides the global configuration.
- **`consumers`** – the test projects and tools the build compiles.
  `"test": true` marks projects that `--test` runs with `dotnet test`.
  Only top-level projects are listed; libraries like PureBusinessClasses are
  built through their ProjectReferences.
- **`enhancer`** – the NDOEnhancer project. Its framework and every path to its
  output are kept up to date (5.5).
- **`consumersBuildFirst`** – projects built once before the first consumer.
  The test projects run the enhancer from its Debug output, so the enhancer is
  built here in Debug, while NDO.Build packs its Release output.
- **`exclude`** – folders (relative to the root) that are not scanned. They
  contain legacy projects with NDO 1.x–4.x GAC or HintPath references.
- **`pinned`** – projects whose PackageReferences are never changed and that
  don't take part in propagation. Used for projects that intentionally stay on
  an old NDO version, like PersistentEnhancerTestClasses (net6.0/netstandard).

### 5.2 Dependency graph

The script scans every `*.csproj` under the repository root, skipping `bin`,
`obj`, `packages`, `.vs` and the `exclude` folders.

- An **edge** "B depends on A" is created when:
  - project B contains `<PackageReference Include="id">`, where `id` matches a
    manifest id case-insensitively. A is that package's project; or
  - project B contains a `<ProjectReference>` that resolves to project A.
- Dependencies are followed transitively, also through projects that are no
  packages (e.g. NDOEnhancer → Ecma335).
- A package's dependencies are the packages reachable from its project, its
  `buildFirst` projects and its `versionFollowers`.
- A consumer's dependencies are the packages reachable from its project.

The package graph is checked for cycles and sorted topologically.

### 5.3 Algorithm

```
1. Load manifest; scan projects; build graph (5.2).

2. Detect changes
   current(p) = version in p's project file / nuspec
   changed    = { p | manifest.version(p) != current(p) }        (manual)

3. Propagate
   for p in topological order:
       if p not in changed and a dependency of p is in changed:
           p.version = bump(current(p), policy)                   // 5.1.0 -> 5.1.1
           changed += p                                           (propagated)
   Write the propagated versions back into packages.json.

4. Patch producers (every package)
   csproj:  <Version>x.y.z</Version>
            <AssemblyVersion>x.y.z.0</AssemblyVersion>
            <FileVersion>x.y.z.0</FileVersion>
   nuspec:  <version>x.y.z</version>
   versionFollowers: same three properties.
   For unchanged packages this only normalizes the format (e.g. NDO.Mapping's
   3-part AssemblyVersion, NDO.Build's 4-part version).

5. Patch consumers (every scanned project except pinned ones)
   for each <PackageReference Include=id Version=v> with id in the manifest:
       if v != manifest version: set Version to the manifest version
   This includes all package projects, because they reference other NDO
   packages only through PackageReferences (2.3), e.g. NDO.dll → NDOInterfaces.
   Every consumer always references the version that is being built.
   ProjectReferences of the test projects are left untouched; they always build
   against the current source.

5b. Patch the enhancer (5.5)
   tfm = highest netX.Y in the TargetFramework(s) of all package projects
   enhancer csproj:  <TargetFramework>tfm</TargetFramework>  (single framework)
   every scanned csproj and every package nuspec:
       NDOEnhancer\bin\<Debug|Release>\<any netX.Y>\…  ->  …\<tfm>\…

6. Print a report – stop here with --dry-run or --no-build.

7. Build packages in topological order (5.4).
   toBuild = changed
           ∪ { p | step 5 updated a PackageReference in p's project }
           ∪ { p | <packageSource>/<id>.<version>.nupkg does not exist }
           ∪ --rebuild ids                       (--all: every package; --only: just these)

8. Build consumers that depend on a package in toBuild (--all: every consumer).
   Before the first consumer, build consumersBuildFirst (enhancer in Debug).
   With --test, run `dotnet test` on those marked "test".
```

The "nupkg does not exist" rule makes the script restartable: if a build fails
after the files were patched, the next run detects no version change, but it
still builds every package that hasn't reached `BuiltPackages` yet.

Rules for patching:
- **Keep formatting.** The text is edited in place with regular expressions
  limited to the found element. Encoding, BOM and line endings are preserved.
  The diff of a project file shows only the changed version lines.
- **Only properties are patched.** `<Version>` is only replaced inside a
  `PropertyGroup`, never the `<Version>` child element of a `PackageReference`.
  All occurrences are patched (also in conditional `PropertyGroup`s). Missing
  `AssemblyVersion`/`FileVersion` properties are not added; the SDK derives them.
- **Report unknown references.** If a reference uses a version range or a
  property (`Version="$(...)"`), the script reports it instead of guessing.

### 5.4 Building

**Packages** are built in topological order:

```
dotnet restore <csproj> --force
dotnet build   <csproj> -c Release --no-restore
```

`GeneratePackageOnBuild=true` is set in all package projects, so the build also
produces the `.nupkg` / `.snupkg`. The script then copies
`bin/<Configuration>/<id>.<version>.nupkg` and `.snupkg` to `BuiltPackages`.
The `cmd copy` `PostBuild` targets have been removed from the project files.

**Restore.** The repository's `nuget.config` adds `BuiltPackages` as a package
source. Because producers are built and collected first (topological order),
the packages are available when a dependent package or a consumer restores:
NDOInterfaces is in `BuiltPackages` before NDO.Mapping restores, NDO.Mapping
before NDO.SchemaGenerator, and so on. `--force` makes restore re-evaluate
even if the version in the project file didn't change.

**Fresh clone.** `BuiltPackages` is not under version control. In a fresh
clone, a package project like NDO.dll can't be restored until its dependencies
have been built. Run `dotnet run Make/build.cs` once (or
`dotnet run Make/build.cs --all --no-consumers`) before building a solution in
Visual Studio.

**NuGet cache.** Before a package is built, its entry
`~/.nuget/packages/<id>/<version>` (or `%NUGET_PACKAGES%`) is deleted.
Otherwise consumers would keep the old content if a package is rebuilt without
a version change.

**Consumers** are built after all packages. First the `consumersBuildFirst`
projects are built once (MakeEnhancerDate in Release, NDOEnhancer in Debug),
then every consumer with

```
dotnet restore <csproj> --force
dotnet build   <csproj> -c <configuration> --no-restore
dotnet test    <csproj> -c <configuration> --no-build      (only with --test)
```

A failing package build stops the script, because its dependents can't be built.
A failing consumer is reported, the remaining consumers are still built, and the
exit code is non-zero.

**Exceptions that can't use `dotnet build`:**

| Project | Handling |
|---|---|
| `NDOEnhancer.BuildTask` | `"builder": "msbuild"`: Visual Studio's `MSBuild.exe`, found via `vswhere -latest -find MSBuild\**\Bin\MSBuild.exe`. Long term: convert it to an SDK-style project with `Microsoft.Build.Utilities.Core` |
| `NDO.Build` (nuspec) | `"builder": "nuget"`: first the `buildFirst` projects, then `nuget/NuGet.exe pack NDO.Build.nuspec -Version <version> -OutputDirectory <packageSource>` |
| `Tools/MakeEnhancerDate` | Prerequisite of NDOEnhancer, listed in `buildFirst` of NDO.Build |

### 5.5 Enhancer

Test projects enhance their persistent classes with an `<Exec>` task that calls
the enhancer directly from its build output, e.g. in PureBusinessClasses:

```xml
<Exec WorkingDirectory="bin\Debug\net8.0"
      Command="..\..\..\..\..\NDOEnhancer\NDOEnhancer\bin\debug\net11.0\NDOEnhancer ..\..\..\PureBusinessClasses.ndoproj $(TargetFramework)" />
```

Rules:
- **Only the highest .NET version.** The enhancer targets exclusively the highest
  `netX.Y` that any package project targets (currently net11.0). The script sets
  `<TargetFramework>` in the enhancer project accordingly; a `TargetFrameworks`
  list is replaced by that single framework.
- **Paths follow the framework.** In every scanned csproj and every package
  nuspec, the framework folder in a path `NDOEnhancer\bin\<Debug|Release>\<netX.Y>\`
  is set to that version. This covers the `<Exec>` tasks of the test projects and
  the `<file src=…>` of `NDO.Build.nuspec`.
- **Release for the package, Debug for the tests.** NDO.Build builds the enhancer
  in Release (`buildFirst`). Before the first consumer, the enhancer is built in
  Debug (`consumersBuildFirst`), so the `<Exec>` tasks find a current Debug build.

When a package project gets a new framework (e.g. net12.0), the next run moves
the enhancer and all paths to it without further manual edits.

**Assembly resolution.** When the enhancer reflects the assembly of a test
project, it resolves referenced assemblies (NDO.dll, …) first in the bin
directory of that assembly, then in the NuGet package folder (via
`project.assets.json`). The bin directory covers ProjectReferences, the package
folder covers PackageReferences.

### 5.6 Command line

```
dotnet run Make/build.cs [options]

  --dry-run            Show detected changes, propagated bumps, all file edits
                       and the build plan; change nothing.
  --no-build           Patch versions and references only.
  --all                Build all packages and all consumers.
  --only <id>          Build only this package (repeatable); consumers that
                       depend on it are built as well.
  --rebuild <id>       Rebuild a package without a version change (repeatable).
  --no-consumers       Don't build consumers.
  --test               Run `dotnet test` on the built test projects.
  --bump patch|minor   Propagation policy (default: "propagation" in the manifest).
  -c <config>          Configuration of packages (default: manifest).
```

Example session:

```
> # edit Make/packages.json: NDOInterfaces 6.0.0 -> 6.0.1
> dotnet run Make/build.cs --dry-run
Version changes:
  NDOInterfaces        6.0.0 -> 6.0.1   (manual)
  NDO.Mapping          6.0.0 -> 6.0.1   (propagated)
  NDO.ProviderFactory  6.0.0 -> 6.0.1   (propagated)
  ndo.sqlserver        5.1.0 -> 5.1.1   (propagated)   … other providers
  NDO.dll              6.0.0 -> 6.0.1   (propagated)
  NDO.JsonFormatter    5.1.1 -> 5.1.2   (propagated)
  NDO.Build            6.0.1 -> 6.0.2   (propagated)
File edits:
  NDOInterfaces/NDOInterfaces.csproj   Version 6.0.0 -> 6.0.1 …
  SimpleMappingTool/Mapping.csproj     PackageReference NDO.mapping 5.1.1 -> 6.0.1
  …
Packages to build:  NDOInterfaces, NDO.Mapping, …
Consumers to build: IntegrationTests/IntegrationTests/IntegrationTests.csproj, …
> dotnet run Make/build.cs --test
```

### 5.7 Propagation policy

By default a dependent package gets a **patch** bump. If the developer has
already raised a dependent's version in the manifest, for example to a new
minor version for an API change, that manual version wins and the package is
not bumped again.

This applies to **all** dependents, including the **providers**: they depend on
NDOInterfaces through a PackageReference, so a change of NDOInterfaces bumps
every provider.

The dependency in the produced nuspec is a *minimum* version (`>= x`).
Bumping the dependent is still necessary. It guarantees that
the published dependent package is a new artifact that points to the new
dependency, and that old consumers don't receive a changed package under an
unchanged version.

## 6. Migration Steps

1. **Create the manifest** `Make/packages.json` from the current project versions (2.1, 2.2). ✔
2. **Implement the script** `Make/build.cs`. A first `--dry-run` without
   manual changes reports no version changes, only format normalizations. ✔
3. **Unify version formats.** 4-part Assembly/FileVersion, 3-part Version;
   done automatically on the first patch run. ✔
4. **Remove the copy targets.** The `cmd copy` `PostBuild` targets are gone;
   the script collects the packages. ✔
5. **Add a `nuget.config`** at the repository root with `BuiltPackages` as a
   relative package source. The stale user-level source `NDO` has been removed
   from the user's `NuGet.Config`. ✔
6. **Retire the old build.** `Make/NDO.proj` and `Tools/PatchNdoVersion` are deleted. ✔
6b. **PackageReferences between package projects.** The ProjectReferences
   between the package projects (and from NDOEnhancer to the core packages) are
   replaced by PackageReferences (2.3). ✔
7. **Optional: Central Package Management.** Introduce `Directory.Packages.props`
   for the NDO package versions used by consumers.
8. **Optional: modernize legacy projects.** Convert `NDOEnhancer.BuildTask` to
   SDK-style so that the build needs no Visual Studio installation.

## 7. Decisions

- **Package projects reference each other through PackageReferences, test
  projects preferably through ProjectReferences** (2.3). A package is built
  against released versions of its dependencies; tests see source changes
  immediately where they reference the projects.
- **Consumers always reference the version being built.** This applies to test
  projects, tools and package projects that consume other NDO packages (e.g.
  NDO.dll → NDOInterfaces, NDO.JsonFormatter → ndo.dll, and therefore
  FormatterUnitTests). Their
  PackageReferences are set to the manifest version on every run. A package
  whose references were updated is rebuilt. Existing ProjectReferences, as in
  the NDODLL tests, stay; they always build against the current source. After
  the packages, the build compiles the affected test projects and, with
  `--test`, runs them. Projects that must stay on an old version are `pinned`.
- **Provider propagation** takes place. Providers are bumped when NDOInterfaces changes (5.7).
- **VSIX** is not part of the build (1).
- **Pre-release versions** are not supported (1).

## 8. Known Issues

These consumers fail for reasons outside the build script:

- `NDOEnhancer/PersistentEnhancerTestClasses` is multi-targeted. Its `<Exec>`
  task runs `AfterTargets="Build"` in the outer build, where `$(TargetFramework)`
  is empty, and the enhancer exits with code 3.
- `UnitTestGenerator/UnitTests` uses the `NDO.Build` package. Its
  `NDOEnhancer.BuildTask.dll` references `Microsoft.Build.Utilities.v4.0`, which
  can't be loaded by `dotnet build` (.NET MSBuild). Projects using NDO.Build can
  currently only be built with Visual Studio. Converting `NDOEnhancer.BuildTask`
  to an SDK-style project with `Microsoft.Build.Utilities.Core` (migration step 8)
  solves this for package consumers as well.
- `IntegrationTests/IntegrationTests` calls `Logger.ClearTestLogs()`, which the
  only published version of `Formfakten.TestLogger` (1.0.0) doesn't contain.
  A newer TestLogger version is needed.
