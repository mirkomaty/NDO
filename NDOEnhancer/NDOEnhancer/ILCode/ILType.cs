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
			else if (ilTypeName == "byte" || ilTypeName == "uint8")
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
			else if (ilTypeName == "unsigned int8[]" || ilTypeName == "uint8[]")
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
                string tn = ilTypeName;
                if (tn.StartsWith( vtPrefix ))
                    tn = tn.Substring( 10 );
                else if (tn.StartsWith( classPrefix ))
                    tn = tn.Substring( 6 );
                tn = tn.Trim();

                var typeSpec = new EcmaTypeSpec();
                typeSpec.Parse( tn );
                var assyName = String.Empty;
                if (typeSpec.ResolutionScope != String.Empty)
					assyName = typeSpec.ResolutionScope.Substring(1, typeSpec.ResolutionScope.Length - 2);

                var dottedName = new EcmaDottedName();
                dottedName.Parse( typeSpec.TypenameWithoutScope );
                var typeName = dottedName.UnquotedName;
				if (assyName != "")
					return typeName + ", " + assyName;
				return typeName;
            }
		}
	}
}
