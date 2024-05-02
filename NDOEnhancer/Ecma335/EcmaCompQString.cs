using System;

namespace NDOEnhancer.Ecma335
{
    public class EcmaCompQString : IEcmaDefinition
    {
        int nextTokenPosition;
        public int NextTokenPosition
        {
            get { return nextTokenPosition; }
        }
        string content = String.Empty;
        public string Content
        {
            get { return content; }
        }

        public bool Parse( string input )
        {
            var p = 0;
            if (input[p] != '"')
                return false;
            bool firstRun = true;

            do
            {
                if (!firstRun)
                    content += input[p++];
                if (input[p] != '"')
                    throw new EcmaILParserException( "leading \"", "+", input );
                content += input[p++];
                var q = input.IndexOf('"', p + 1);
                if (q == -1)
                    throw new EcmaILParserException( "trailing \"", "leading\"", input );
                var length = q - p + 1;
                content += input.Substring( p, length );
                p += length;
            } while (p < input.Length && input[p] == '+');

            nextTokenPosition = p;
            return true;
        }
    }
}
