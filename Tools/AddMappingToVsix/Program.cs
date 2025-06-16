using System.IO.Compression;

namespace AddMappingToVsix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var vsixFile = args[0];
            string releasePath = Path.GetDirectoryName(vsixFile)!;
            var root = Path.GetFullPath(Path.Combine(releasePath, @"..\..\.."));
            var mappingDir = Path.Combine( root, @"SimpleMappingTool\bin\Release\net8.0-windows" );
            var files = Directory.GetFiles(mappingDir);
            using (var zipFile = ZipFile.Open( args[0], ZipArchiveMode.Update ))
            {
                foreach (var file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    zipFile.CreateEntryFromFile( fi.FullName, "MappingTool/" + fi.Name );
                }
            }
        }
    }
}
