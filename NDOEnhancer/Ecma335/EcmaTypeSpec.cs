using System;

namespace NDOEnhancer.Ecma335
{
    public class EcmaTypeSpec : IEcmaDefinition
    {
        public string Content { get; private set; } = String.Empty;
        public string ResolutionScope { get; private set; } = String.Empty;

        public string TypenameWithoutScope { get; private set; } = String.Empty;

        public int NextTokenPosition { get; private set; } = 0;
    
        public bool Parse( string input )
        {
            int p = 0;
            if (input[0] == '[')
            {
                p = input.IndexOf(']');
                if (p == -1)
                    throw new EcmaILParserException( "]", "[", input );
                p++;
                Content = ResolutionScope = input.Substring( 0, p );
            }

            var slashedName = new EcmaSlashedName();
            var success = slashedName.Parse( input.Substring( p ) );
            if (!success)
                throw new Exception( $"TypeSpec should contain a SlashedName in: {input}" );

            Content += slashedName.Content;
            TypenameWithoutScope = slashedName.Content;
            NextTokenPosition = p + slashedName.NextTokenPosition;
            return true;
        }
    }
}

/*

typeSpec : className
| '[' name1 ']'
| '[' '.module' name1 ']'
| type
;

className : '[' name1 ']' slashedName
| '[' '.module' name1 ']' slashedName
| slashedName
;

slashedName : name1
| slashedName '/' name1
;


name1 : id => empty
| DOTTEDNAME
| name1 '.' name1
;

 */