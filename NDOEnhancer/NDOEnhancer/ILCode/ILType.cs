using NDOEnhancer;
using NDOEnhancer.Ecma335;
using System;
using System.Text.RegularExpressions;

namespace NDOEnhancer.ILCode
{
	public class ILType
	{
		private static readonly string vtPrefix = "valuetype";
		private static readonly string classPrefix = "class";

		public static Type GetNetType( string ilTypeName )
		{
			return Type.GetType( GetNetTypeName( ilTypeName ) );
		}

		public static string GetNetTypeName( string ilTypeName )
		{
			ilTypeName = ilTypeName.Trim();
			Regex regex = new Regex("System.Nullable`1<(.*)>");
			Match match = regex.Match(ilTypeName);
			if (match.Success)
				ilTypeName = match.Groups[1].Value;

			if (ilTypeName == "bool")
				return "System.Boolean";
			else if (ilTypeName == "byte")
				return "System.Byte";
			else if (ilTypeName == "sbyte")
				return "System.SByte";
			else if (ilTypeName == "char")
				return "System.Char";
			else if (ilTypeName == "unsigned char")
				return "System.UChar";
			else if (ilTypeName == "short" || ilTypeName == "int16")
				return "System.Int16";
			else if (ilTypeName == "unsigned int16")
				return "System.UInt16";
			else if (ilTypeName == "unsigned int8")
				return "System.Byte";
			else if (ilTypeName == "unsigned int8[]")
				return "System.Byte[]";
			else if (ilTypeName == "int" || ilTypeName == "int32")
				return "System.Int32";
			else if (ilTypeName == "unsigned int32")
				return "System.UInt32";
			else if (ilTypeName == "long" || ilTypeName == "int64")
				return "System.Int64";
			else if (ilTypeName == "unsigned int64")
				return "System.UInt64";
			else if (ilTypeName == "float32" || ilTypeName == "float" || ilTypeName == "single")
				return "System.Single";
			else if (ilTypeName == "float64" || ilTypeName == "double")
				return "System.Double";
			else if (ilTypeName == "string")
				return "System.String";
			else
			{
				var dottedName = new EcmaDottedName();
				string tn = ilTypeName;
				if (tn.StartsWith( vtPrefix ))
					tn = tn.Substring( 10 );
				else if (tn.StartsWith( classPrefix ))
					tn = tn.Substring( 6 );
				tn = tn.Trim();
				if (tn.StartsWith( $"{Corlib.Name}" )) // Corlib.Name is inclosed in []
					tn = tn.Substring( Corlib.Name.Length ).Trim();
				if (!tn.StartsWith( "[" ))
				{
					dottedName.Parse( tn );
					return dottedName.UnquotedName;
				}

				tn = tn.Substring( 1 );
				int pos = tn.IndexOf("]");
				dottedName.Parse( tn.Substring( 0, pos ) );
				var assyName = dottedName.UnquotedName;
				dottedName = new EcmaDottedName();
				dottedName.Parse( tn.Substring( pos + 1 ) );
				var typeName = dottedName.UnquotedName;
				return ( typeName + ", " + assyName );
			}
		}
	}
}
