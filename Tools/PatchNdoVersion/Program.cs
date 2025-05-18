using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace PatchNdoVersion
{
    internal class Program
    {
        static int Main(string[] args)
        {
			if (args.Length < 3)
			{
				Console.WriteLine( "usage: PatchNdoVersion <ProjFile> -i <NDOInterfaces-Version> -n <NDO-Version> -e <NDOEnhancer-Version>" );
				return -1;
			}

			var projFile = args[0];
			string? iVersion = null;
            string? eVersion = null;
            string? nVersion = null;


            if (!File.Exists(projFile))
			{
				Console.WriteLine( $"File doesn't exist: '{projFile}'" );
				return -3;
			}


            try
            {
				Regex regex = new Regex(@"^\d+\.\d+\.\d+");
                var changed = false;
                var i = Array.IndexOf(args, "-i");
				if (i > 0)
				{
					if (args.Length < i + 2)
                        throw new Exception( "Option -i needs a parameter." );
					
					iVersion = args[i + 1];

					if (!regex.Match( iVersion ).Success)
						throw new Exception( "Parameter of -i must be a version string" );
				}

                var e = Array.IndexOf(args, "-e");
                if (e > 0)
                {
                    if (args.Length < e + 2)
                        throw new Exception( "Option -e needs a parameter." );

                    eVersion = args[e + 1];

                    if (!regex.Match( eVersion ).Success)
                        throw new Exception( "Parameter of -e must be a version string" );
                }

                var n = Array.IndexOf(args, "-n");
                if (n > 0)
                {
                    if (args.Length < n + 2)
                        throw new Exception( "Option -n needs a parameter." );

                    nVersion = args[n + 1];

                    if (!regex.Match( nVersion ).Success)
                        throw new Exception( "Parameter of -n must be a version string" );
                }
                
				string ndoRootPath = AppDomain.CurrentDomain.BaseDirectory;

				XDocument doc = XDocument.Load( projFile );
				var project = doc.Root!;

				var prElement = project.Elements("ItemGroup").FirstOrDefault(pg => pg.Element("PackageReference") != null); ;
				if (prElement == null)
					throw new Exception( "Project file doesn't have PackageReference items" );

				if (iVersion != null)
				{
					var iElement = prElement.Elements("PackageReference").FirstOrDefault(el => el.Attribute("Include")?.Value == "NDOInterfaces");
					if (iElement != null)
					{
                        if (iElement.Attribute( "Version" )?.Value != iVersion)
                        {
                            iElement.Attribute( "Version" )!.Value = iVersion;
                            changed = true;
                        }
					}
					else
					{
						throw new Exception( "Project needs a PackageReference to NDOInterfaces" );
					}
				}

                if (eVersion != null)
                {
                    var eElement = prElement.Elements("PackageReference").FirstOrDefault(el => el.Attribute("Include")?.Value == "NDOEnhancer");
                    if (eElement != null)
                    {
                        if (eElement.Attribute( "Version" )?.Value != eVersion)
                        {
                            eElement.Attribute( "Version" )!.Value = eVersion;
                            changed = true;
                        }
                    }
                    else
                    {
                        throw new Exception( "Project needs a PackageReference to NDOEnhancer" );
                    }
                }

                if (nVersion != null)
                {
                    var element = prElement.Elements("PackageReference").FirstOrDefault(el => el.Attribute("Include")?.Value.ToLower() == "ndo.dll");
                    if (element != null)
                    {
                        if (element.Attribute( "Version" )?.Value != nVersion)
                        {
                            element.Attribute( "Version" )!.Value = nVersion;
                            changed = true;
                        }
                    }
                    else
                    {
                        throw new Exception( "Project needs a PackageReference to NDO.dll" );
                    }
                }

                if (changed)
    				doc.Save(projFile);

				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine( ex.ToString() );
				return -1;
			}
		}
    }
}
