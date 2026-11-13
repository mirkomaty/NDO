# Packing NDO.Build with `dotnet pack` instead of NuGet.exe

## Problem

`Make/build.cs` (`BuildPackage`) creates the NDO.Build package by calling
`nuget/NuGet.exe pack NDO.Build.nuspec`. NuGet.exe is a Windows executable and
doesn't work on Linux.

## Solution

`dotnet pack` can't process a `.nuspec` file directly; it needs a project.
A minimal helper project that doesn't compile anything and just points to the
nuspec solves this. This was tested on Linux with the .NET 11 SDK: the
resulting `NDO.Build.<version>.nupkg` has the same structure as described in
the nuspec (`build/**` with the enhancer output, `build/NDO.Build.targets`,
`license/license.txt`, `readme.md`).

### Helper project `nuget/NDO.Build.Pack.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Only used to pack NDO.Build.nuspec with "dotnet pack". -->
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <IncludeBuildOutput>false</IncludeBuildOutput>
    <NoBuild>true</NoBuild>
    <NuspecFile>NDO.Build.nuspec</NuspecFile>
  </PropertyGroup>
</Project>
```

- The project is named after the nuspec (`<nuspec name>.Pack.csproj`), so
  `build.cs` can derive the name from `p.Project` and further nuspec packages
  can be added the same way.
- `TargetFramework` is `netstandard2.0`, because nothing is compiled. This way
  the file doesn't need to be touched when the .NET version changes.
- csproj and nuspec are in the same directory, so relative paths work and the
  nuspec base path is `nuget/`, as expected by the `src` attributes.

### Change in `Make/build.cs` (`BuildPackage`)

Replace the NuGet.exe call with:

```csharp
if (p.Builder == "nuget")
{
    var dir = Path.GetDirectoryName(p.Project)!;
    var packProject = Path.GetFileNameWithoutExtension(p.Project) + ".Pack.csproj";
    Exec("dotnet", dir, "pack", packProject, "-o", packageSource);
    if (!System.IO.File.Exists(Path.Combine(packageSource, $"{p.Id}.{p.Target}.nupkg")))
        throw new BuildException($"dotnet pack didn't produce {p.Id}.{p.Target}.nupkg.");
    return;
}
```

The version doesn't have to be passed: `PatchProducers` already writes the
target version into the nuspec's `<version>` element. (`NuspecProperties`
would only replace `$version$` tokens anyway.)

### Required fix in `NDO.Build.nuspec`

Linux file systems are case sensitive. The nuspec references
`..\NDOEnhancer\NDOEnhancer\bin\release\net11.0\**`, but the directory is
`bin/Release`. Packing fails with:

```
Could not find a part of the path '.../NDOEnhancer/NDOEnhancer/bin/release/net11.0'
```

Change the path to `..\NDOEnhancer\NDOEnhancer\bin\Release\net11.0\**`.
Backslashes and the leading `/` in `target="/build"` work fine on Linux.

## Side effects checked

- `ScanProjects()` in `build.cs` finds the helper project (it scans all
  `*.csproj`). This is harmless: `PatchConsumers` only touches
  `PackageReference` versions and `PatchEnhancer` only NDOEnhancer paths;
  neither occurs in the helper project.
- There are no `Directory.Build.props/targets` in the repository or in parent
  directories, so nothing is imported unintentionally.
- `dotnet pack` creates `nuget/obj/`, which is already covered by `.gitignore`.

## Optional follow-ups

- Rename `"builder": "nuget"` in `Make/packages.json` (e.g. to `"nuspec"`),
  since NuGet.exe is no longer used.
- Remove `nuget/NuGet.exe` from the repository.
