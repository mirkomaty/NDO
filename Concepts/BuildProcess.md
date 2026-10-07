# NDO Build Process – Concept for Independent Package Versioning

## 1. Context

NDO ships as a set of NuGet packages. Other projects inside and outside this
repository consume them. A complete build is currently described by the MSBuild
script `Make/NDO.proj`. The resulting packages land in `BuiltPackages`.

The packages evolve independently. When one package changes:

1. It needs a new version. That means `Version`, `AssemblyVersion` and `FileVersion` in its csproj.
2. Every project that depends on it must reference the new version.
3. Every dependent *package* must get a new version of its own.
4. Steps 2 and 3 repeat for the dependents' dependents.

The current tool, `Tools/PatchNdoVersion`, can't do this. It assumes one version
for all packages NDO.dll depends on (`-i`, `-n`, `-m`, `-e` switches). It also
only patches `PackageReference` versions; it never touches the package's own version.

## 2. Current State

### 2.1 Packages produced by the build

| Package id | Project | Version | Assembly/FileVersion |
|---|---|---|---|
| NDOInterfaces | `NDOInterfaces/NDOInterfaces.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.Mapping | `NDO.Mapping/NDO.Mapping/NDO.Mapping.csproj` | 6.0.0 | **6.0.0** (3 parts) |
| NDO.ProviderFactory | `NDO.ProviderFactory/NDO.ProviderFactory/NDO.ProviderFactory.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.SchemaGenerator | `NDO.SchemaGenerator/NDO.SchemaGenerator/NDO.SchemaGenerator.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.dll | `NDODLL/NDO.csproj` | 6.0.0 | 6.0.0.0 |
| NDO.JsonFormatter | `NdoJsonFormatter/NdoJsonFormatter/NDO.JsonFormatter.csproj` | 5.1.1 | 5.1.1.0 |
| ndo.mysql, ndo.mysqlconnector, ndo.oracle, ndo.postgre, ndo.sqlite, ndo.sqlserver | `Provider/*/NDO.*/NDO.*.csproj` | 5.1.0 | 5.1.0.0 |
| NDO.Build | `nuget/NDO.Build.nuspec` (packed with `nuget.exe`) | 6.0.1.0 | – |

Notes:
- **Versions are scattered.** There is no `Directory.Build.props` or
  `Directory.Packages.props`. Every version is hard-coded in its project file.
- **NDO.Build and NDOEnhancer are coupled.** NDO.Build packages the binaries of
  `NDOEnhancer/NDOEnhancer` (Version 6.0.1.0) and `NDOEnhancer.BuildTask`. The
  NDOEnhancer version should equal the NDO.Build version.

### 2.2 How dependencies are expressed

There are two different mechanisms.

**ProjectReference (inside the core).** These references form the core graph:

```
NDOInterfaces
 ├── NDO.Mapping ── NDO.SchemaGenerator ─┐
 ├── NDO.ProviderFactory ────────────────┤
 ├───────────────────────────────────────┴── NDO.dll
 ├── ndo.<provider> (6x)
 └── NDOEnhancer (also Mapping, ProviderFactory, SchemaGenerator, Ecma335)
```

`dotnet pack` turns a ProjectReference into a package dependency `>= <Version of
the referenced project>`. So **no reference needs to be edited** here. The
dependent package only needs its own version bump, because its nuspec now
points to a different dependency version.

**PackageReference (outside the core).**

| Consumer | References |
|---|---|
| NDO.JsonFormatter | `ndo.dll` 5.1.1 |
| SimpleMappingTool (`Mapping.csproj`) | `NDO.mapping` 5.1.1 |
| Provider UISupport projects (5x) | `NDOInterfaces` 5.1.0 |
| IntegrationTests | `NDO.JsonFormatter` 5.1.0 |
| PureBusinessClasses | `ndo.dll` 5.1.1 |
| FormatterUnitTests | `ndo.dll` 5.1.1, `ndo.sqlserver` 5.1.0 |
| PersistentEnhancerTestClasses | `ndo.dll` 5.0.0 |
| UnitTestGenerator/* | `NDO.dll`, `NDO.Build`, `NDO.SqlServer`, `NDO.Mapping`, `NDO.ProviderFactory`, `NDOInterfaces` 5.0.0 |

Package ids are spelled with inconsistent casing (`ndo.dll`, `NDO.dll`,
`NDO.mapping`), so all matching must be case-insensitive.

### 2.3 Problems with the current build

- **The patch step is broken.** `Make/NDO.proj` calls `PatchNdoVersion -i` on
  NDO.Mapping, NDO.ProviderFactory, the providers and NDOEnhancer. These projects
  no longer have an `NDOInterfaces` PackageReference, so these steps fail.
- **PatchNdoVersion is limited:**
  - it only searches the first `ItemGroup` that contains a `PackageReference`;
  - it reformats the file through `XDocument.Save`;
  - it can't bump a project's own version.
- **Copying to `BuiltPackages` is Windows-only and incomplete.** It is done by
  per-project `PostBuild` targets using `cmd copy`. The providers and
  NDO.JsonFormatter don't copy at all, and NDO.dll copies no `.snupkg`.
- **Some projects can't be built with `dotnet build`:**
  - `NDOPackage` is a VSIX project and needs the VSSDK targets of Visual Studio's MSBuild.
  - `NDOEnhancer.BuildTask` is an old-style project with GAC references to
    `Microsoft.Build.Utilities.v4.0`.
  - `UISupport/NDO.UISupport` is old-style, and its HintPath points to a
    `netstandard2.0` folder that no longer exists.
- **The NuGet package source points to the wrong drive.** The user-level source
  named `NDO` is `C:\Projekte\NDO\BuiltPackages`, but the repository lives on `D:`.
  The repository has no `nuget.config` of its own.
- **`Make/DebugBuild.cmd` is broken.** It references a `test.proj` and a
  `nuget.config` that don't exist.

## 3. Requirements

1. **One place to enter versions.** The developer enters the new version of a
   package in one single file.
2. **Producer patching.** The script sets `Version`, `AssemblyVersion` and
   `FileVersion` in that package's csproj, or the version in its nuspec.
3. **Consumer patching.** Every `PackageReference` to a changed package is
   updated in all csproj files.
4. **Propagation.** Every package that depends on a changed package, directly
   or transitively, automatically gets a new version. Then steps 2 and 3 apply
   to it as well.
5. **Building.** Compilation uses `dotnet build <csproj>`, in dependency order.
6. **Collection.** All resulting `.nupkg` / `.snupkg` files end up in `BuiltPackages`.

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
  X itself was not bumped manually" lead to deeply nested conditions and
  property functions.
- **Editing XML in place.** `XmlPoke` rewrites the file and can't easily target
  attributes on specific items without reformatting.
- **Diagnostics.** It's hard to produce a readable dry-run report.

MSBuild stays the right tool *inside* each csproj: pack settings, copying,
enhancer integration. It should no longer be the orchestrator.

### 4.2 Alternatives

| Option | Pros | Cons |
|---|---|---|
| **MSBuild orchestration file** (status quo) | Already present; no new runtime | Weak at graph and conditional logic; painful XML patching; hard to debug |
| **PowerShell 7 script** | Ubiquitous on Windows; good XML support via `[xml]`; easy process calls | Different language from the rest of the code base; no typed version handling; `[xml]` also reformats unless handled carefully; PS 5.1 vs. 7 differences |
| **C# file-based app** (`dotnet run build.cs`, .NET 10+) | Same language as NDO and the existing tools; `XDocument`, `NuGet.Versioning`, `System.Text.Json` available; no `.csproj` needed (`#:package` directives); debuggable in VS/VS Code; cross-platform | Requires the .NET 10 SDK (already required by the net10.0/net11.0 targets) |
| **Cake / NUKE** | Mature build DSLs in C#; built-in tasks for `dotnet build/pack`, NuGet, MSBuild via vswhere | Extra dependency and learning curve; NUKE adds a build project and bootstrap scripts; heavyweight for one repository |
| **Central Package Management** (`Directory.Packages.props`) | All `PackageReference` versions in one file; consumer patching becomes a single-file edit | Only covers consumers, not the producers' own versions or propagation. It **complements** a script and doesn't replace it |

### 4.3 Recommendation

Use a **C# file-based app**: `Make/build.cs`, run with `dotnet run Make/build.cs`.
It is the smallest step that covers every requirement:

- It is the same language as the code base and the existing tools
  (PatchNdoVersion, MakeEnhancerDate, AddMappingToVsix), which it can replace.
- It has typed version handling through `NuGet.Versioning`.
- It needs no build project or bootstrapper. A single file is executed directly
  by the SDK.
- It is easy to provide a `--dry-run` mode with a clear report.

Optional second step: introduce `Directory.Packages.props` for the
*PackageReference* versions of NDO packages. Then the script only has to update
that one file for the consumers.

## 5. Recommended Design

### 5.1 Version manifest – the single source of truth

`Make/packages.json` lists every package that the build produces. It is the
**only** file the developer edits to release a new version.

```json
{
  "packageSource": "../BuiltPackages",
  "configuration": "Release",
  "propagation": "patch",
  "packages": [
    { "id": "NDOInterfaces",       "project": "NDOInterfaces/NDOInterfaces.csproj",                                  "version": "6.0.0" },
    { "id": "NDO.Mapping",         "project": "NDO.Mapping/NDO.Mapping/NDO.Mapping.csproj",                          "version": "6.0.0" },
    { "id": "NDO.ProviderFactory", "project": "NDO.ProviderFactory/NDO.ProviderFactory/NDO.ProviderFactory.csproj",  "version": "6.0.0" },
    { "id": "NDO.SchemaGenerator", "project": "NDO.SchemaGenerator/NDO.SchemaGenerator/NDO.SchemaGenerator.csproj",  "version": "6.0.0" },
    { "id": "NDO.dll",             "project": "NDODLL/NDO.csproj",                                                   "version": "6.0.0" },
    { "id": "NDO.JsonFormatter",   "project": "NdoJsonFormatter/NdoJsonFormatter/NDO.JsonFormatter.csproj",          "version": "5.1.1" },
    { "id": "ndo.sqlserver",       "project": "Provider/SqlServerProvider/NDO.SqlServer/NDO.SqlServer.csproj",       "version": "5.1.0" },
    { "id": "NDO.Build",           "project": "nuget/NDO.Build.nuspec", "builder": "nuget",
      "versionFollowers": [ "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj" ],
      "buildFirst": [ "NDOEnhancer/NDOEnhancer/NDOEnhancer.csproj",
                      { "project": "NDOEnhancer.BuildTask/NDOEnhancer.BuildTask/NDOEnhancer.BuildTask.csproj", "builder": "msbuild" } ],
      "version": "6.0.1" }
  ]
}
```

The other providers are listed in the same way. Dependencies are **not** listed
in the manifest. They are derived from the project files, so they can never get
out of sync with the real references.

`versionFollowers` names projects that get the same version but are not packed
themselves, like NDOEnhancer.

### 5.2 Dependency graph

The script scans every `*.csproj` under the repository root and builds the graph:

- It skips `bin/`, `obj/`, `packages/` and an exclude list of legacy folders
  such as `Tutorial`, `UnitTests` and `ClassGenerator`. These still use NDO 1.x–4.x
  GAC or HintPath references.
- A **producer** is a project listed in the manifest.
- An **edge** `A → B` ("B depends on A") is created when:
  - project B contains `<PackageReference Include="A">`, matched case-insensitively
    against the manifest ids; or
  - project B contains a `<ProjectReference>` whose resolved full path is the
    producer project of A.
- **Consumers** are all projects with an edge, whether or not they are packages
  themselves. Tests and tools are consumers but not producers.

The graph is checked for cycles and sorted topologically.

### 5.3 Algorithm

```
1. Load manifest; load graph (5.2).

2. Detect changes
   changed = { p | manifest.version(p) != version in p's project file }

3. Propagate
   for p in topological order:
       if p not in changed and any dependency of p is in changed:
           p.version = bump(current version of p, policy)  // e.g. 5.1.0 -> 5.1.1
           changed += p
   Write the propagated versions back into packages.json
   (so the manifest is again the single source of truth).

4. Patch producers (for each p in changed)
   csproj:  <Version>x.y.z</Version>
            <AssemblyVersion>x.y.z.0</AssemblyVersion>
            <FileVersion>x.y.z.0</FileVersion>
   nuspec:  <version>x.y.z</version>
   versionFollowers: same three properties.

5. Patch consumers (for each project in the repository)
   for each <PackageReference Include=id Version=v> with id in changed:
       set Version to the new version
   ProjectReferences are left untouched (the version flows in at pack time).

6. Print a report (always) – stop here with --dry-run or --no-build.

7. Build changed packages in topological order (see 5.4).

8. Collect *.nupkg / *.snupkg into packageSource.
```

Rules for patching:
- **Keep formatting.** Edit the text in place with a regex limited to the found
  element, or use `XDocument` with `LoadOptions.PreserveWhitespace` and save
  without reformatting and with the original encoding/BOM. The diff of a project
  file must show only the changed version lines.
- **Normalize version formats.** Assembly and file versions always use 4 parts.
  This also fixes the 3-part format in NDO.Mapping.
- **Patch every occurrence.** If a property or reference appears in several
  conditional `PropertyGroup`s or `ItemGroup`s, all occurrences are patched.
- **Report unknown references.** If a reference uses a version range or a
  property (`Version="$(...)"`), the script reports it instead of guessing.

### 5.4 Building

Each changed package is built as follows, in topological order:

```
dotnet build <csproj> -c Release
```

`GeneratePackageOnBuild=true` is already set in all package projects, so
`dotnet build` also produces the `.nupkg` / `.snupkg`. Consumers that are not
packages, like tests, are only patched and not built. An optional `--all` switch
builds them too.

**Restore.** `dotnet build` restores implicitly. The packages that the
consumers need must be in a configured package source before their restore.
That works because the topological order guarantees producers are built and
collected first.

**NuGet cache.** If a package is rebuilt *without* a version change, the stale
copy in `~/.nuget/packages/<id>/<version>` must be deleted first. Otherwise
consumers keep restoring the old content. This replaces the `DeletePackages`
target of `NDO.proj`.

**Collecting packages.** The script copies `bin/<Configuration>/<id>.<version>.nupkg`
and `.snupkg` to `BuiltPackages`. The alternative is to set `PackageOutputPath`
centrally in a `Directory.Build.props`. Either way, the `cmd copy` `PostBuild`
targets can be removed from the project files.

**Exceptions that can't use `dotnet build`:**

| Project | Reason | Handling |
|---|---|---|
| `NDOPackage/NDOPackage.csproj` (VSIX) | Needs the VSSDK targets of Visual Studio | Build with Visual Studio's `MSBuild.exe`, found via `vswhere -latest -find MSBuild\**\Bin\MSBuild.exe`. Not part of the package build; only built with `--vsix` |
| `NDOEnhancer.BuildTask` | Old-style project, GAC references to Microsoft.Build | Short term: MSBuild via vswhere (`"builder": "msbuild"`). Long term: convert it to an SDK-style project using `Microsoft.Build.Utilities.Core`, then use `dotnet build` |
| `NDO.Build` (nuspec) | Not a project | First build NDOEnhancer and NDOEnhancer.BuildTask (`buildFirst`), then run `nuget/NuGet.exe pack NDO.Build.nuspec -Version <version> -OutputDirectory <packageSource>`. Long term: a pack-only SDK project (`NoBuild`, `IncludeBuildOutput=false`) so that `dotnet pack` is enough |
| Tools (`MakeEnhancerDate`, `AddMappingToVsix`) | Prerequisites of NDOEnhancer and NDOPackage | Built with `dotnet build -c Release` before the first project that needs them |

### 5.5 Command line

```
dotnet run Make/build.cs [options]

  --dry-run          Show detected changes, propagated bumps and all file
                     edits; change nothing.
  --no-build         Patch versions and references only.
  --only <id>        Build only this package (and nothing that depends on it).
  --rebuild <id>     Rebuild a package without a version change
                     (clears its NuGet cache entry).
  --all              Also build consumers that are not packages (tests, tools).
  --bump patch|minor Propagation policy for dependent packages (default: patch).
  -c <config>        Configuration (default: Release).
```

Example session:

```
> # edit Make/packages.json: NDOInterfaces 6.0.0 -> 6.0.1
> dotnet run Make/build.cs --dry-run
Changed (manual):      NDOInterfaces          6.0.0 -> 6.0.1
Changed (propagated):  NDO.Mapping            6.0.0 -> 6.0.1
                       NDO.ProviderFactory    6.0.0 -> 6.0.1
                       NDO.SchemaGenerator    6.0.0 -> 6.0.1
                       NDO.dll                6.0.0 -> 6.0.1
                       ndo.sqlserver          5.1.0 -> 5.1.1   (… other providers)
                       NDO.JsonFormatter      5.1.1 -> 5.1.2
Reference updates:     SimpleMappingTool/Mapping.csproj  NDO.mapping 5.1.1 -> 6.0.1
                       Provider/.../SqlServerUISupport.csproj  NDOInterfaces 5.1.0 -> 6.0.1
                       …
Build order:           NDOInterfaces, NDO.Mapping, NDO.ProviderFactory, …
> dotnet run Make/build.cs
```

### 5.6 Propagation policy

By default a dependent package gets a **patch** bump. If the developer has
already raised a dependent's version in the manifest, for example to a new
minor version for an API change, that manual version wins and the package is
not bumped again.

Note: with ProjectReferences, the dependency in the produced nuspec is a
*minimum* version (`>= x`). Bumping the dependent is still necessary. It
guarantees that the published dependent package is a new artifact that points
to the new dependency, and that old consumers don't receive a changed package
under an unchanged version.

## 6. Migration Steps

1. **Create the manifest.** Write `Make/packages.json` from the current project
   versions (table 2.1).
2. **Implement the script.** Write `Make/build.cs` with `--dry-run` first, and
   verify the report against the current repository. A first run without manual
   changes must report "nothing changed".
3. **Unify version formats.** Use 4-part Assembly/FileVersion everywhere; this
   is done automatically on the first patch.
4. **Replace the copy targets.** Remove the `cmd copy` `PostBuild` targets, or
   replace them with a central `PackageOutputPath`.
5. **Add a `nuget.config`** at the repository root with `BuiltPackages` as a
   relative package source. This removes the dependency on the user-level
   source (currently pointing to `C:\Projekte\NDO\BuiltPackages`).
6. **Retire the old patch step.** Remove the `PatchNdoVersion` target and the
   tool from `Make/NDO.proj`. Then keep `NDO.proj` only as a thin wrapper for
   the VSIX build, or delete it.
7. **Clean up legacy scripts.** Delete or fix `Make/DebugBuild.cmd`.
8. **Optional: Central Package Management.** Introduce `Directory.Packages.props`
   for the NDO package versions used by consumers.
9. **Optional: modernize the remaining legacy projects.** Convert
   `NDOEnhancer.BuildTask` and `UISupport/NDO.UISupport` to SDK-style so that
   only the VSIX still needs Visual Studio's MSBuild.

## 7. Open Questions

- **Test projects.** Should test projects (IntegrationTests, FormatterUnitTests,
  PureBusinessClasses, UnitTestGenerator/*) always follow the newest package
  versions, or keep pinned versions? Alternatively, should they switch to
  ProjectReferences, as the NDODLL tests already do?
- **Provider propagation.** Should providers be bumped when NDOInterfaces
  changes? They depend on it through a ProjectReference, so the default policy
  says yes.
- **VSIX version.** Should the VSIX (`NDOPackage/source.extension.vsixmanifest`,
  currently 5.0.0) be part of the manifest and follow NDO.dll or NDO.Build?
- **Pre-release versions.** Is a pre-release suffix (`6.1.0-beta1`) needed?
  `NuGet.Versioning` supports it, but Assembly/FileVersion then have to drop the suffix.

Resume this session with:
claude --resume f5170928-a07b-4b74-8448-44f4716daaa0