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
            var path1 = Path.Combine( root, @"NDOInterfaces\bin\Release\net8.0\NDOInterfaces.dll" );
            var path2 = Path.Combine( root, @"NDO.Mapping\NDO.Mapping\bin\Release\net8.0\NDO.mapping.dll" );
            using (var zipFile = ZipFile.Open( args[0], ZipArchiveMode.Update ))
            {
                FileInfo fi = new FileInfo(path1);
                zipFile.CreateEntryFromFile( fi.FullName, "MappingTool/" + fi.Name );
				fi = new FileInfo(path2);
				zipFile.CreateEntryFromFile( fi.FullName, "MappingTool/" + fi.Name );
			}
		}
    }
}
