// NDO build script. The concept is described in Concepts/BuildProcess.md.
//
// Usage: dotnet run Make/build.cs [options]
//
//   --dry-run            Show detected changes, propagated bumps, all file edits
//                        and the build plan; change nothing.
//   --no-build           Patch versions and references only.
//   --all                Build all packages and all consumers.
//   --only <id>          Build only this package (repeatable); consumers that
//                        depend on it are built as well.
//   --rebuild <id>       Rebuild a package without a version change (repeatable).
//   --no-consumers       Don't build consumers.
//   --test               Run 'dotnet test' on the built test projects.
//   --bump patch|minor   Propagation policy (default: "propagation" in the manifest).
//   -c <config>          Configuration of packages (default: manifest).

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;

try
{
    return new Build(Options.Parse(args)).Run();
}
catch (BuildException ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("ERROR: " + ex.Message);
    return 1;
}

class Options
{
    public bool DryRun, NoBuild, All, NoConsumers, Test;
    public List<string> Only = new(), Rebuild = new();
    public string? Bump, Configuration;

    public static Options Parse(string[] args)
    {
        var o = new Options();
        for (int i = 0; i < args.Length; i++)
        {
            string Next() => i + 1 < args.Length ? args[++i] : throw new BuildException($"Option {args[i]} needs a value.");
            switch (args[i])
            {
                case "--dry-run": o.DryRun = true; break;
                case "--no-build": o.NoBuild = true; break;
                case "--all": o.All = true; break;
                case "--only": o.Only.Add(Next()); break;
                case "--rebuild": o.Rebuild.Add(Next()); break;
                case "--no-consumers": o.NoConsumers = true; break;
                case "--test": o.Test = true; break;
                case "--bump": o.Bump = Next(); break;
                case "-c": case "--configuration": o.Configuration = Next(); break;
                default: throw new BuildException($"Unknown option '{args[i]}'.");
            }
        }
        return o;
    }
}

class BuildException(string message) : Exception(message);

/// <summary>A version without pre-release suffix. A 4th part is accepted if it is 0.</summary>
readonly record struct PackageVersion(int Major, int Minor, int Patch) : IComparable<PackageVersion>
{
    public static PackageVersion Parse(string s, string context)
    {
        if (!TryParse(s, out var v))
            throw new BuildException($"{context}: '{s}' is not a version of the form major.minor.patch.");
        return v;
    }

    public static bool TryParse(string s, out PackageVersion v)
    {
        v = default;
        var parts = s.Trim().Split('.');
        if (parts.Length < 2 || parts.Length > 4)
            return false;
        var n = new int[4];
        for (int i = 0; i < parts.Length; i++)
            if (!int.TryParse(parts[i], out n[i]) || n[i] < 0)
                return false;
        if (n[3] != 0)
            return false;
        v = new PackageVersion(n[0], n[1], n[2]);
        return true;
    }

    public PackageVersion Bump(string policy) => policy switch
    {
        "patch" => this with { Patch = Patch + 1 },
        "minor" => new PackageVersion(Major, Minor + 1, 0),
        _ => throw new BuildException($"Unknown propagation policy '{policy}'. Use patch or minor."),
    };

    public int CompareTo(PackageVersion o) =>
        Major != o.Major ? Major.CompareTo(o.Major) : Minor != o.Minor ? Minor.CompareTo(o.Minor) : Patch.CompareTo(o.Patch);

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
    public string AssemblyVersion => $"{Major}.{Minor}.{Patch}.0";
}

/// <summary>A project or nuspec file. Edits are done on the text to keep the formatting.</summary>
class ProjectFile
{
    public string Path = "";
    public string Text = "";
    public string OriginalText = "";
    public Encoding Encoding = new UTF8Encoding(false);
    public XDocument? Xml;
    public List<string> ProjectReferences = new();   // full paths
    public List<string> PackageReferences = new();   // package ids as written
    public List<string> Edits = new();

    public bool IsDirty => Text != OriginalText;

    public static ProjectFile Load(string path)
    {
        var bytes = File.ReadAllBytes(path);
        bool bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
        var encoding = new UTF8Encoding(bom);
        var text = encoding.GetString(bytes, bom ? 3 : 0, bytes.Length - (bom ? 3 : 0));
        var file = new ProjectFile { Path = path, Text = text, OriginalText = text, Encoding = encoding };
        try
        {
            file.Xml = XDocument.Parse(text);
        }
        catch (Exception ex)
        {
            throw new BuildException($"{path} can't be parsed: {ex.Message}");
        }
        var dir = System.IO.Path.GetDirectoryName(path)!;
        foreach (var e in file.Xml.Descendants().Where(e => e.Name.LocalName == "ProjectReference"))
        {
            var include = (string?)e.Attribute("Include");
            if (include != null)
                file.ProjectReferences.Add(System.IO.Path.GetFullPath(System.IO.Path.Combine(dir, include.Replace('\\', System.IO.Path.DirectorySeparatorChar))));
        }
        foreach (var e in file.Xml.Descendants().Where(e => e.Name.LocalName == "PackageReference"))
        {
            var include = (string?)e.Attribute("Include");
            if (include != null)
                file.PackageReferences.Add(include);
        }
        return file;
    }

    public string? Property(string name) =>
        Xml!.Descendants().FirstOrDefault(e => e.Name.LocalName == name && e.Parent?.Name.LocalName == "PropertyGroup")?.Value.Trim();

    public void Save()
    {
        var bytes = Encoding.GetPreamble().Concat(Encoding.GetBytes(Text)).ToArray();
        File.WriteAllBytes(Path, bytes);
    }
}

record BuildStep(string Project, string Builder, string? Configuration);

class Package
{
    public string Id = "";
    public string Project = "";
    public string Builder = "dotnet";
    public PackageVersion Manifest, Current, Target;
    public List<string> VersionFollowers = new();
    public List<BuildStep> BuildFirst = new();
    public HashSet<Package> Dependencies = new();
    public string? Change;   // null, "manual" or "propagated"
    public bool IsNuspec => Project.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase);
}

record Consumer(string Project, bool Test, string? Configuration)
{
    public HashSet<Package> Dependencies { get; } = new();
}

class Build(Options options)
{
    static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;
    static readonly string[] SkippedDirectories = { "bin", "obj", "packages", ".vs", ".git", "node_modules" };

    static readonly Regex PropertyGroupRx = new(@"<PropertyGroup\b(?:[^>]*[^/>])?>.*?</PropertyGroup>", RegexOptions.Singleline);
    static readonly Regex MetadataRx = new(@"<metadata\b[^>]*>.*?</metadata>", RegexOptions.Singleline);
    static readonly Regex PackageReferenceRx = new(@"<PackageReference\b(?<attrs>[^>]*?)(?:/>|>(?<body>.*?)</PackageReference>)", RegexOptions.Singleline);
    static readonly Regex IncludeRx = new(@"\bInclude\s*=\s*""(?<v>[^""]*)""");
    static readonly Regex VersionAttributeRx = new(@"(?<pre>\bVersion\s*=\s*"")(?<v>[^""]*)(?<post>"")");
    static readonly Regex VersionElementRx = new(@"(?<pre><Version>)(?<v>[^<]*)(?<post></Version>)");

    readonly Dictionary<string, ProjectFile> files = new(PathComparer);
    readonly List<string> warnings = new();
    readonly List<Package> packages = new();
    readonly List<Consumer> consumers = new();
    readonly HashSet<string> pinned = new(PathComparer);
    readonly List<string> exclude = new();
    readonly List<BuildStep> consumersBuildFirst = new();
    readonly HashSet<string> referencesUpdated = new(PathComparer);
    string? enhancer;
    string root = "", manifestPath = "", manifestText = "", packageSource = "", configuration = "", policy = "";

    public int Run()
    {
        LoadManifest();
        var scanned = ScanProjects();
        BuildGraph();
        var order = SortTopologically();
        DetectChanges(order);
        PatchProducers();
        PatchConsumers(scanned);
        PatchEnhancer(scanned);

        var toBuild = SelectPackages(order);
        var consumersToBuild = SelectConsumers(toBuild);
        Report(order, toBuild, consumersToBuild);

        if (options.DryRun)
        {
            Console.WriteLine("Dry run – nothing changed.");
            return 0;
        }

        SaveChanges();
        if (options.NoBuild)
            return 0;

        foreach (var p in toBuild)
            BuildPackage(p);
        var failed = new List<string>();
        if (consumersToBuild.Count > 0 && consumersBuildFirst.Count > 0)
        {
            Header("Prerequisites of the consumers");
            foreach (var step in consumersBuildFirst)
                BuildProject(step.Project, step.Builder, step.Configuration ?? configuration);
        }
        foreach (var c in consumersToBuild)
            if (!BuildConsumer(c))
                failed.Add(Rel(c.Project));

        Console.WriteLine();
        Console.WriteLine($"Built packages: {(toBuild.Count == 0 ? "none" : string.Join(", ", toBuild.Select(p => $"{p.Id} {p.Target}")))}");
        if (failed.Count > 0)
        {
            Console.Error.WriteLine("Failed consumers:");
            failed.ForEach(f => Console.Error.WriteLine("  " + f));
            return 1;
        }
        return 0;
    }

    // ---------------------------------------------------------------- manifest

    void LoadManifest()
    {
        var scriptDir = AppContext.GetData("EntryPointFileDirectoryPath") as string ?? FindMakeDirectory();
        manifestPath = Path.Combine(scriptDir, "packages.json");
        if (!File.Exists(manifestPath))
            throw new BuildException($"Manifest {manifestPath} not found.");
        root = Path.GetFullPath(Path.Combine(scriptDir, ".."));
        manifestText = File.ReadAllText(manifestPath);

        var json = JsonNode.Parse(manifestText, documentOptions: new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true })!;
        packageSource = Path.GetFullPath(Path.Combine(scriptDir, (string?)json["packageSource"] ?? "../BuiltPackages"));
        configuration = options.Configuration ?? (string?)json["configuration"] ?? "Release";
        policy = options.Bump ?? (string?)json["propagation"] ?? "patch";
        new PackageVersion(0, 0, 0).Bump(policy);   // validates the policy

        foreach (var e in json["exclude"]?.AsArray() ?? new JsonArray())
            exclude.Add(Abs((string)e!));
        foreach (var e in json["pinned"]?.AsArray() ?? new JsonArray())
            pinned.Add(Abs((string)e!));

        foreach (var node in json["packages"]!.AsArray())
        {
            var id = (string?)node!["id"] ?? throw new BuildException("A package in the manifest has no id.");
            var p = new Package
            {
                Id = id,
                Project = Abs((string?)node["project"] ?? throw new BuildException($"Package {id} has no project.")),
                Builder = (string?)node["builder"] ?? "dotnet",
                Manifest = PackageVersion.Parse((string?)node["version"] ?? "", $"Manifest version of {id}"),
            };
            foreach (var f in node["versionFollowers"]?.AsArray() ?? new JsonArray())
                p.VersionFollowers.Add(Abs((string)f!));
            p.BuildFirst.AddRange(ParseBuildSteps(node["buildFirst"]));
            if (packages.Any(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
                throw new BuildException($"Package {id} is listed twice in the manifest.");
            packages.Add(p);
        }

        foreach (var node in json["consumers"]?.AsArray() ?? new JsonArray())
        {
            var c = new Consumer(Abs((string)node!["project"]!), (bool?)node["test"] ?? false, (string?)node["configuration"]);
            if (packages.Any(p => PathComparer.Equals(p.Project, c.Project)))
                throw new BuildException($"{Rel(c.Project)} is listed as package and as consumer.");
            consumers.Add(c);
        }

        consumersBuildFirst.AddRange(ParseBuildSteps(json["consumersBuildFirst"]));
        if ((string?)json["enhancer"] is { } enhancerProject)
            enhancer = Abs(enhancerProject);

        foreach (var id in options.Only.Concat(options.Rebuild))
            if (FindPackage(id) == null)
                throw new BuildException($"Unknown package id '{id}'.");
    }

    IEnumerable<BuildStep> ParseBuildSteps(JsonNode? node) =>
        (node?.AsArray() ?? new JsonArray()).Select(s => s is JsonValue
            ? new BuildStep(Abs((string)s!), "dotnet", null)
            : new BuildStep(Abs((string)s!["project"]!), (string?)s["builder"] ?? "dotnet", (string?)s["configuration"]));

    static string FindMakeDirectory()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
        {
            var make = Path.Combine(dir.FullName, "Make");
            if (File.Exists(Path.Combine(make, "packages.json")))
                return make;
        }
        throw new BuildException("Make/packages.json not found.");
    }

    // ---------------------------------------------------------------- graph

    List<ProjectFile> ScanProjects()
    {
        var result = new List<ProjectFile>();
        void Walk(string dir)
        {
            if (exclude.Contains(dir, PathComparer))
                return;
            foreach (var f in Directory.EnumerateFiles(dir, "*.csproj"))
            {
                try
                {
                    result.Add(GetFile(f));
                }
                catch (BuildException ex)
                {
                    warnings.Add(ex.Message + " The project is skipped.");
                }
            }
            foreach (var d in Directory.EnumerateDirectories(dir))
                if (!SkippedDirectories.Contains(Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
                    Walk(d);
        }
        Walk(root);
        return result;
    }

    ProjectFile GetFile(string path)
    {
        if (!files.TryGetValue(path, out var file))
        {
            if (!System.IO.File.Exists(path))
                throw new BuildException($"{Rel(path)} doesn't exist.");
            files[path] = file = ProjectFile.Load(path);
        }
        return file;
    }

    Package? FindPackage(string id) => packages.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    Package? PackageOfProject(string path) => packages.FirstOrDefault(p => PathComparer.Equals(p.Project, path));

    /// <summary>All packages reachable from the given projects, through project and package references.</summary>
    HashSet<Package> Reachable(IEnumerable<string> start)
    {
        var result = new HashSet<Package>();
        var visited = new HashSet<string>(PathComparer);
        var stack = new Stack<string>(start);
        while (stack.Count > 0)
        {
            var path = stack.Pop();
            if (!visited.Add(path) || path.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!System.IO.File.Exists(path))
            {
                warnings.Add($"Referenced project {Rel(path)} doesn't exist.");
                continue;
            }
            var file = GetFile(path);
            foreach (var r in file.ProjectReferences)
            {
                if (PackageOfProject(r) is { } p)
                    result.Add(p);
                stack.Push(r);
            }
            if (pinned.Contains(path))
                continue;
            foreach (var id in file.PackageReferences)
                if (FindPackage(id) is { } p && result.Add(p))
                    stack.Push(p.Project);
        }
        return result;
    }

    void BuildGraph()
    {
        foreach (var p in packages)
        {
            var start = new List<string>();
            if (!p.IsNuspec)
                start.Add(p.Project);
            start.AddRange(p.VersionFollowers);
            start.AddRange(p.BuildFirst.Select(s => s.Project));
            p.Dependencies = Reachable(start);
            if (p.Dependencies.Contains(p))
                throw new BuildException($"Package {p.Id} depends on itself (cycle in the dependency graph).");

            if (!p.IsNuspec)
            {
                var file = GetFile(p.Project);
                var packageId = file.Property("PackageId") ?? file.Property("AssemblyName") ?? Path.GetFileNameWithoutExtension(p.Project);
                if (!packageId.Equals(p.Id, StringComparison.OrdinalIgnoreCase))
                    warnings.Add($"{Rel(p.Project)}: PackageId '{packageId}' differs from manifest id '{p.Id}'.");
            }
        }
        foreach (var c in consumers)
            c.Dependencies.UnionWith(Reachable(new[] { c.Project }));
    }

    List<Package> SortTopologically()
    {
        var order = new List<Package>();
        var remaining = new List<Package>(packages);
        while (remaining.Count > 0)
        {
            var next = remaining.FirstOrDefault(p => p.Dependencies.All(order.Contains))
                ?? throw new BuildException("Cycle in the dependency graph: " + string.Join(", ", remaining.Select(p => p.Id)));
            order.Add(next);
            remaining.Remove(next);
        }
        return order;
    }

    // ---------------------------------------------------------------- versions

    void DetectChanges(List<Package> order)
    {
        foreach (var p in order)
        {
            p.Current = ReadVersion(p);
            if (p.Manifest.CompareTo(p.Current) != 0)
            {
                p.Change = "manual";
                p.Target = p.Manifest;
                if (p.Manifest.CompareTo(p.Current) < 0)
                    warnings.Add($"{p.Id}: manifest version {p.Manifest} is lower than the project version {p.Current}.");
            }
            else if (p.Dependencies.Any(d => d.Change != null))
            {
                p.Change = "propagated";
                p.Target = p.Current.Bump(policy);
                UpdateManifestVersion(p);
            }
            else
            {
                p.Target = p.Current;
            }
        }
    }

    PackageVersion ReadVersion(Package p)
    {
        var file = GetFile(p.Project);
        if (p.IsNuspec)
        {
            var v = file.Xml!.Descendants().FirstOrDefault(e => e.Name.LocalName == "version" && e.Parent?.Name.LocalName == "metadata")?.Value
                ?? throw new BuildException($"{Rel(p.Project)} has no <version>.");
            return PackageVersion.Parse(v, Rel(p.Project));
        }
        var version = file.Property("Version") ?? throw new BuildException($"{Rel(p.Project)} has no <Version> property.");
        return PackageVersion.Parse(version, Rel(p.Project));
    }

    void UpdateManifestVersion(Package p)
    {
        var idMatch = new Regex(@"""id""\s*:\s*""" + Regex.Escape(p.Id) + @"""").Match(manifestText);
        var nextId = new Regex(@"""id""\s*:").Match(manifestText, idMatch.Index + idMatch.Length);
        var versionMatch = new Regex(@"(""version""\s*:\s*"")[^""]*("")").Match(manifestText, idMatch.Index);
        if (!idMatch.Success || !versionMatch.Success || (nextId.Success && nextId.Index < versionMatch.Index))
            throw new BuildException($"The version of {p.Id} can't be located in {manifestPath}.");
        manifestText = manifestText[..versionMatch.Index]
            + versionMatch.Groups[1].Value + p.Target + versionMatch.Groups[2].Value
            + manifestText[(versionMatch.Index + versionMatch.Length)..];
    }

    // ---------------------------------------------------------------- patching

    void PatchProducers()
    {
        foreach (var p in packages)
        {
            var file = GetFile(p.Project);
            if (p.IsNuspec)
                file.Text = MetadataRx.Replace(file.Text, m => ReplaceElement(file, m.Value, "version", p.Target.ToString()));
            else
                PatchVersionProperties(file, p.Target);
            foreach (var f in p.VersionFollowers)
                PatchVersionProperties(GetFile(f), p.Target);
        }
    }

    void PatchVersionProperties(ProjectFile file, PackageVersion v)
    {
        file.Text = PropertyGroupRx.Replace(file.Text, m =>
        {
            var block = ReplaceElement(file, m.Value, "Version", v.ToString());
            block = ReplaceElement(file, block, "AssemblyVersion", v.AssemblyVersion);
            return ReplaceElement(file, block, "FileVersion", v.AssemblyVersion);
        });
    }

    static string ReplaceElement(ProjectFile file, string text, string name, string value) =>
        new Regex($@"(<{name}>)\s*([^<]*?)\s*(</{name}>)").Replace(text, m =>
        {
            if (m.Groups[2].Value == value)
                return m.Value;
            file.Edits.Add($"{name} {m.Groups[2].Value} -> {value}");
            return m.Groups[1].Value + value + m.Groups[3].Value;
        });

    static readonly Regex TargetFrameworkRx = new(@"\bnet(\d+)\.(\d+)\b");
    static readonly Regex EnhancerTargetFrameworkRx = new(@"<TargetFrameworks?>[^<]*</TargetFrameworks?>");
    static readonly Regex EnhancerPathRx = new(@"(?<=NDOEnhancer[\\/]bin[\\/](?:debug|release)[\\/])net\d+\.\d+(?=[\\/])", RegexOptions.IgnoreCase);

    /// <summary>
    /// The enhancer targets only the highest .NET version of the package projects.
    /// Every path to the enhancer's output (Exec tasks of test projects, nuspec files) follows that version.
    /// </summary>
    void PatchEnhancer(List<ProjectFile> scanned)
    {
        if (enhancer == null)
            return;
        var frameworks = packages.Where(p => !p.IsNuspec)
            .Select(p => GetFile(p.Project))
            .SelectMany(f => TargetFrameworkRx.Matches(f.Property("TargetFrameworks") ?? f.Property("TargetFramework") ?? ""))
            .Select(m => (Major: int.Parse(m.Groups[1].Value), Minor: int.Parse(m.Groups[2].Value)))
            .ToList();
        if (frameworks.Count == 0)
            throw new BuildException("The highest .NET version of the package projects can't be determined.");
        var (major, minor) = frameworks.Max();
        var tfm = $"net{major}.{minor}";

        var enhancerFile = GetFile(enhancer);
        enhancerFile.Text = PropertyGroupRx.Replace(enhancerFile.Text, m => EnhancerTargetFrameworkRx.Replace(m.Value, t =>
        {
            var replacement = $"<TargetFramework>{tfm}</TargetFramework>";
            if (t.Value != replacement)
                enhancerFile.Edits.Add($"{t.Value} -> {replacement}");
            return replacement;
        }));

        foreach (var file in scanned.Concat(packages.Where(p => p.IsNuspec).Select(p => GetFile(p.Project))).Distinct())
        {
            file.Text = EnhancerPathRx.Replace(file.Text, m =>
            {
                if (m.Value == tfm)
                    return m.Value;
                file.Edits.Add($"NDOEnhancer path {m.Value} -> {tfm}");
                return tfm;
            });
        }
    }

    void PatchConsumers(List<ProjectFile> scanned)
    {
        foreach (var file in scanned.Where(f => !pinned.Contains(f.Path)))
        {
            file.Text = PackageReferenceRx.Replace(file.Text, m =>
            {
                var id = IncludeRx.Match(m.Groups["attrs"].Value).Groups["v"].Value;
                var p = FindPackage(id);
                if (p == null)
                    return m.Value;
                var rx = VersionAttributeRx.IsMatch(m.Groups["attrs"].Value) ? VersionAttributeRx : VersionElementRx;
                var vm = rx.Match(m.Value);
                if (!vm.Success)
                {
                    warnings.Add($"{Rel(file.Path)}: PackageReference {id} has no version.");
                    return m.Value;
                }
                var current = vm.Groups["v"].Value;
                if (!PackageVersion.TryParse(current, out var cv))
                {
                    warnings.Add($"{Rel(file.Path)}: PackageReference {id} uses version '{current}', which isn't patched.");
                    return m.Value;
                }
                if (cv.CompareTo(p.Target) == 0 && current == p.Target.ToString())
                    return m.Value;
                referencesUpdated.Add(file.Path);
                file.Edits.Add($"PackageReference {id} {current} -> {p.Target}");
                return m.Value[..vm.Index] + vm.Groups["pre"].Value + p.Target + vm.Groups["post"].Value + m.Value[(vm.Index + vm.Length)..];
            });
        }
    }

    void SaveChanges()
    {
        foreach (var file in files.Values.Where(f => f.IsDirty))
            file.Save();
        if (manifestText != System.IO.File.ReadAllText(manifestPath))
            System.IO.File.WriteAllText(manifestPath, manifestText, new UTF8Encoding(false));
    }

    // ---------------------------------------------------------------- planning and report

    List<Package> SelectPackages(List<Package> order)
    {
        if (options.Only.Count > 0)
            return order.Where(p => options.Only.Contains(p.Id, StringComparer.OrdinalIgnoreCase)).ToList();
        return order.Where(p => options.All
            || p.Change != null
            || referencesUpdated.Contains(p.Project)
            || !System.IO.File.Exists(Path.Combine(packageSource, $"{p.Id}.{p.Target}.nupkg"))
            || options.Rebuild.Contains(p.Id, StringComparer.OrdinalIgnoreCase)).ToList();
    }

    List<Consumer> SelectConsumers(List<Package> toBuild)
    {
        if (options.NoConsumers)
            return new();
        return consumers.Where(c => options.All || c.Dependencies.Overlaps(toBuild)).ToList();
    }

    void Report(List<Package> order, List<Package> toBuild, List<Consumer> consumersToBuild)
    {
        Console.WriteLine("Version changes:");
        var changed = order.Where(p => p.Change != null).ToList();
        if (changed.Count == 0)
            Console.WriteLine("  none");
        var width = order.Max(p => p.Id.Length);
        foreach (var p in changed)
            Console.WriteLine($"  {p.Id.PadRight(width)}  {p.Current,-8} -> {p.Target,-8}  ({p.Change})");

        Console.WriteLine("File edits:");
        var edited = files.Values.Where(f => f.Edits.Count > 0).OrderBy(f => f.Path, PathComparer).ToList();
        if (edited.Count == 0)
            Console.WriteLine("  none");
        foreach (var f in edited)
        {
            Console.WriteLine("  " + Rel(f.Path));
            f.Edits.ForEach(e => Console.WriteLine("      " + e));
        }
        if (manifestText != System.IO.File.ReadAllText(manifestPath))
            Console.WriteLine($"  {Rel(manifestPath)}\n      propagated versions");

        if (warnings.Count > 0)
        {
            Console.WriteLine("Warnings:");
            warnings.ForEach(w => Console.WriteLine("  " + w));
        }

        if (!options.NoBuild)
        {
            Console.WriteLine($"Packages to build ({configuration}):");
            Console.WriteLine("  " + (toBuild.Count == 0 ? "none" : string.Join(", ", toBuild.Select(p => p.Id))));
            Console.WriteLine("Consumers to build:");
            if (consumersToBuild.Count == 0)
                Console.WriteLine("  none");
            else if (consumersBuildFirst.Count > 0)
                Console.WriteLine("  first: " + string.Join(", ", consumersBuildFirst.Select(s => $"{Path.GetFileNameWithoutExtension(s.Project)} ({s.Configuration ?? configuration})")));
            foreach (var c in consumersToBuild)
                Console.WriteLine($"  {Rel(c.Project)}{(c.Test && options.Test ? " (+ test)" : "")}");
        }
        Console.WriteLine();
    }

    // ---------------------------------------------------------------- building

    void BuildPackage(Package p)
    {
        Header($"Package {p.Id} {p.Target}");
        Directory.CreateDirectory(packageSource);
        DeleteFromNuGetCache(p);
        foreach (var step in p.BuildFirst)
            BuildProject(step.Project, step.Builder, step.Configuration ?? configuration);

        if (p.Builder == "nuget")
        {
            Exec(Path.Combine(root, "nuget", "NuGet.exe"), Path.GetDirectoryName(p.Project)!,
                "pack", Path.GetFileName(p.Project), "-Version", p.Target.ToString(), "-OutputDirectory", packageSource);
            if (!System.IO.File.Exists(Path.Combine(packageSource, $"{p.Id}.{p.Target}.nupkg")))
                throw new BuildException($"nuget pack didn't produce {p.Id}.{p.Target}.nupkg.");
            return;
        }

        BuildProject(p.Project, p.Builder, configuration);
        var outputDir = Path.Combine(Path.GetDirectoryName(p.Project)!, "bin", configuration);
        var copied = 0;
        foreach (var ext in new[] { ".nupkg", ".snupkg" })
        {
            var name = $"{p.Id}.{p.Target}{ext}";
            var source = Directory.Exists(outputDir)
                ? Directory.EnumerateFiles(outputDir, "*" + ext).FirstOrDefault(f => Path.GetFileName(f).Equals(name, StringComparison.OrdinalIgnoreCase))
                : null;
            if (source == null)
                continue;
            System.IO.File.Copy(source, Path.Combine(packageSource, Path.GetFileName(source)), overwrite: true);
            Console.WriteLine($"Copied {Path.GetFileName(source)} to {Rel(packageSource)}");
            if (ext == ".nupkg")
                copied++;
        }
        if (copied == 0)
            throw new BuildException($"{p.Id}.{p.Target}.nupkg not found in {Rel(outputDir)}.");
    }

    bool BuildConsumer(Consumer c)
    {
        var config = c.Configuration ?? configuration;
        Header($"Consumer {Rel(c.Project)}");
        try
        {
            BuildProject(c.Project, "dotnet", config);
            if (c.Test && options.Test)
                Exec("dotnet", root, "test", c.Project, "-c", config, "--no-build");
            return true;
        }
        catch (BuildException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return false;
        }
    }

    void BuildProject(string project, string builder, string config)
    {
        switch (builder)
        {
            case "dotnet":
                Exec("dotnet", root, "restore", project, "--force");
                Exec("dotnet", root, "build", project, "-c", config, "--no-restore");
                break;
            case "msbuild":
                Exec(FindMSBuild(), root, project, "-restore", "-nologo", "-v:minimal", $"-p:Configuration={config}");
                break;
            default:
                throw new BuildException($"Unknown builder '{builder}' for {Rel(project)}.");
        }
    }

    void DeleteFromNuGetCache(Package p)
    {
        var cache = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        var dir = Path.Combine(cache, p.Id.ToLowerInvariant(), p.Target.ToString());
        if (Directory.Exists(dir))
        {
            Console.WriteLine($"Deleting {dir}");
            Directory.Delete(dir, recursive: true);
        }
    }

    string? msbuild;

    string FindMSBuild()
    {
        if (msbuild != null)
            return msbuild;
        var vswhere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vswhere.exe");
        if (!System.IO.File.Exists(vswhere))
            throw new BuildException("vswhere.exe not found. Visual Studio is needed to build old-style projects.");
        var psi = new ProcessStartInfo(vswhere) { RedirectStandardOutput = true, UseShellExecute = false };
        foreach (var a in new[] { "-latest", "-prerelease", "-products", "*", "-requires", "Microsoft.Component.MSBuild", "-find", @"MSBuild\**\Bin\MSBuild.exe" })
            psi.ArgumentList.Add(a);
        using var process = Process.Start(psi)!;
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        msbuild = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault()
            ?? throw new BuildException("MSBuild.exe of Visual Studio not found.");
        return msbuild;
    }

    void Exec(string fileName, string workingDirectory, params string[] arguments)
    {
        Console.WriteLine($"> {Path.GetFileName(fileName)} {string.Join(" ", arguments.Select(a => a.Contains(' ') ? $"\"{a}\"" : a))}");
        var psi = new ProcessStartInfo(fileName) { WorkingDirectory = workingDirectory, UseShellExecute = false };
        foreach (var a in arguments)
            psi.ArgumentList.Add(a);
        using var process = Process.Start(psi) ?? throw new BuildException($"{fileName} can't be started.");
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new BuildException($"'{Path.GetFileName(fileName)} {string.Join(" ", arguments)}' failed with exit code {process.ExitCode}.");
    }

    static void Header(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 78));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', 78));
    }

    string Abs(string relative) => Path.GetFullPath(Path.Combine(root.Length > 0 ? root : ".", relative.Replace('\\', Path.DirectorySeparatorChar)));
    string Rel(string path) => Path.GetRelativePath(root, path).Replace('\\', '/');
}
