using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PatchNdoVersion
{
    internal class Program
    {
		static readonly string sourceRevisionTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project>
  <PropertyGroup>
    <SourceRevisionId>{0}</SourceRevisionId>
  </PropertyGroup>
</Project>";

        static int Main(string[] args)
        {
			if (args.Length < 2)
			{
				Console.WriteLine( "usage: PatchNdoVersion <ProjFile> <Version>" );
				return -1;
			}

			var projFile = args[0];
			var version = args[1];

			Regex regex = new Regex(@"\d+\.\d+\.\d+");
			if (!regex.Match(version).Success)
			{
				Console.WriteLine( "Version must be \\d+\\.\\d+\\.\\d+" );
				return -2;
			}

			if (!File.Exists(projFile))
			{
				Console.WriteLine( $"File doesn't exist: '{projFile}'" );
				return -3;
			}

			try
			{
				string ndoRootPath = AppDomain.CurrentDomain.BaseDirectory;

				do
				{
					ndoRootPath = Path.GetFullPath( Path.Combine( ndoRootPath, ".." ) );
				} while (!Directory.Exists( Path.Combine( ndoRootPath, ".git" ) ));

				var revision = GetRevision(ndoRootPath);

				XDocument doc = XDocument.Load( projFile );
				var project = doc.Root!;
				bool hasVersionElement = false;

				//Version 5.0.0
				//FileVersion 5.0.0.0
				//AssemblyVersion 5.0.0.0
				//SourceRevisionId abc0123

				var pgElement = project.Elements("PropertyGroup").First();
				var element = pgElement.Element("Version");
				if (element == null)
					pgElement.Add( new XElement( "Version", version ) );

				foreach (var pg in project.Elements("PropertyGroup"))
				{
					element = pg.Element("Version");
					if (element != null)  // We are in the right property group
					{
						hasVersionElement = true;

						element.Value = version;
						var assemblyVersion = pg.Element("AssemblyVersion");
						var longVersion = version + ".0";
						if (assemblyVersion != null)
							assemblyVersion.Value = longVersion;
						else
							pg.Add( new XElement( "AssemblyVersion", longVersion ) );

						var fileVersion = pg.Element("FileVersion");
						if (fileVersion != null)
							fileVersion.Value = longVersion;
						else
							pg.Add( new XElement( "FileVersion", longVersion ) );

						break;
					}
				}

				if (!hasVersionElement)
					throw new Exception( "Project file. doesn't have a Version tag. Add a version tag to the first PropertyGroup element in the project file." );

				doc.Save(projFile);

				var sourceRevFile = Path.Combine( Path.GetDirectoryName(projFile)!, "SourceRevisionId.props" );
				using (StreamWriter sw = new StreamWriter( sourceRevFile, false, Encoding.UTF8 ))
				{
					sw.Write( sourceRevisionTemplate.Replace( "{0}", revision ) );
				}

				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine( ex.ToString() );
				return -4;
			}
		}

		static string GetRevision(string ndoPath)
        {
			string? head = null;
			char[] buf = new char[7];
			var gitDir = Path.Combine(ndoPath, ".git");
			using (StreamReader sr = new StreamReader( Path.Combine( gitDir, "HEAD" ) ))
			{
				sr.Read( buf, 0, 5 );
				head = sr.ReadToEnd().Trim();
			}

			var refHeadPath = Path.Combine( gitDir, head!.Replace( "/", "\\" ) );
			if (!File.Exists( refHeadPath ))
				throw new Exception( "Ref head doesn't exist: " + refHeadPath );

			using (StreamReader sr = new StreamReader( refHeadPath ))
			{
				sr.Read( buf, 0, 7 );  // first 7 chars of the head				
			}

			return new string( buf );
		}
    }
}
