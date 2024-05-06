
// Copyright (c) 2002-2024 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using System.Runtime.InteropServices;
using System.Text;

namespace NDOEnhancer.Ecma335.Bytes
{
    public class EcmaBytes
    {
        public bool Parse( byte[] bytes, IEnumerable<Type> ctorTypes, out IEnumerable<object?> ctorValues, out Dictionary<string, object> namedParams )
        {
            int pos = 0;
            var prolog = (UInt16)ReadParam( bytes, typeof( UInt16 ), ref pos )!;

            if (prolog != 1)
                throw new Exception( "Prolog of bytes section in CustomAttribute must be 1" );

            var result = new List<object?>();
            ctorValues = result;
            foreach (var t in ctorTypes)
            {
                result.Add( ReadParam( bytes, t, ref pos ) );
            }

            namedParams = new Dictionary<string, object>();

            var numNamed = (UInt16)ReadParam( bytes, typeof( UInt16 ), ref pos )!;

            for(int i = 0; i < numNamed; i++)
            {
                // Read named parameter
            }

            return true;
        }


        private object? ReadParam( byte[] bytes, Type type, ref int pos )
        {
            if (type.FullName == "System.String" || type.FullName == "System.Type")
            {
                string? para;
                int len;
                try
                {
                    len = PackedLength.Read( bytes, ref pos );
                }
                catch(IndexOutOfRangeException)
                {
                    throw new Exception( $"Can't read string from CustomAttribute constructor parameters. The byte array is too short. Position: {pos}" );
                }

                if (len == -1)
                    para = null;
                else
                    para = new System.Text.UTF8Encoding().GetString( bytes, pos, len );

                pos += len;

                if (para != null && para != string.Empty)
                {
                    if (para[para.Length - 1] == '\0')
                        para = para.Substring( 0, para.Length - 1 );
                }

                return para;
            }
            else if (type.FullName == "System.Boolean")
            {
                if (pos >= bytes.Length)
                    throw new Exception( $"Can't read boolean from CustomAttribute constructor parameters. The byte array is too short. Position: {pos}" );
                // Standard: A bool is a single byte with value 0( false ) or 1( true )
                return bytes[pos++] == 1;
            }
            else if (type.FullName == "System.Char")
            {
                if (pos >= bytes.Length - 2)
                    throw new Exception( $"Can't read char from CustomAttribute constructor parameters. The byte array is too short. Position: {pos}" );
                // Standard: char is a two-byte Unicode character
                var c = Encoding.Unicode.GetChars( bytes, pos, 2 );
                pos += 2;
                return c[0];
            }
            else if (typeof( System.Enum ).IsAssignableFrom( type ))
            {
                if (pos >= bytes.Length - 4)
                    throw new Exception( $"Can't read int from CustomAttribute constructor parameters. The byte array is too short. Position: {pos}" );
                int para = (int)ReadParam(bytes, typeof(int), ref pos)!; //((bytes[pos+3] * 256 + bytes[pos+2]) * 256 + bytes[pos+1]) * 256 + bytes[pos];
                return Enum.ToObject( type, para );
            }

            var size = Marshal.SizeOf( type );
            
            if (pos + size > bytes.Length)
                throw new Exception( $"Can't read {type.FullName} from CustomAttribute constructor parameters. The byte array is too short. Position: {pos}" );

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr addressInPinnedObject = (handle.AddrOfPinnedObject() + pos);
            object? returnedObject = Marshal.PtrToStructure(addressInPinnedObject, type);
            handle.Free();
            pos += size;
            return returnedObject;
        }
    }
}
