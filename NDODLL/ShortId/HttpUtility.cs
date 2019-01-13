// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*
The MIT License(MIT)

Copyright(c) .NET Foundation and Contributors

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;
using System.Diagnostics;
using System.Text;

namespace NDO.HttpUtil
{
    /// <summary>
    /// Utility class for Encoding and Decoding strings.
    /// </summary>
    /// <remarks>
    /// This class is essentially a copy of the methods from .Net Core.
    /// We duplicate the code here to avoid dependencies on System.Web.
    /// </remarks>
    public class HttpUtility
    {
        /// <summary>
        /// Encodes a string with percent encoding as suggested in RFC 3986
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode( string str )
        {
            if (str == null)
                return str;
            byte[] bytes = Encoding.UTF8.GetBytes( str );
            return Encoding.ASCII.GetString( UrlEncode( bytes, 0, bytes.Length ) );
        }

        /// <summary>
        /// Decodes an UrlEncoded string back to a .NET string (internally UTF16).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UrlDecode( string s )
        {
            byte[] bytes = Encoding.ASCII.GetBytes( s );
            return Encoding.UTF8.GetString( UrlDecode( bytes, 0, bytes.Length ) );
        }

        static byte[] UrlDecode( byte[] bytes, int offset, int count )
        {
            if (!ValidateUrlEncodingParameters( bytes, offset, count ))
            {
                return null;
            }

            int decodedBytesCount = 0;
            byte[] decodedBytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                int pos = offset + i;
                byte b = bytes[pos];

                if (b == '+')
                {
                    b = (byte)' ';
                }
                else if (b == '%' && i < count - 2)
                {
                    int h1 = HexToInt( (char)bytes[pos + 1] );
                    int h2 = HexToInt( (char)bytes[pos + 2] );

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        b = (byte)((h1 << 4) | h2);
                        i += 2;
                    }
                }

                decodedBytes[decodedBytesCount++] = b;
            }

            if (decodedBytesCount < decodedBytes.Length)
            {
                byte[] newDecodedBytes = new byte[decodedBytesCount];
                Array.Copy( decodedBytes, newDecodedBytes, decodedBytesCount );
                decodedBytes = newDecodedBytes;
            }

            return decodedBytes;
        }

        public static int HexToInt( char h )
        {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }

        private static bool ValidateUrlEncodingParameters( byte[] bytes, int offset, int count )
        {
            if (bytes == null && count == 0)
            {
                return false;
            }

            if (bytes == null)
            {
                throw new ArgumentNullException( nameof( bytes ) );
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException( nameof( offset ) );
            }
            if (count < 0 || offset + count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException( nameof( count ) );
            }

            return true;
        }

        public static char IntToHex( int n )
        {
            Debug.Assert( n < 0x10 );

            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'a');
        }

        // Set of safe chars, from RFC 1738.4 minus '+'
        public static bool IsUrlSafeChar( char ch )
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                return true;

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        protected internal static byte[] UrlEncode( byte[] bytes, int offset, int count )
        {
            if (!ValidateUrlEncodingParameters( bytes, offset, count ))
            {
                return null;
            }

            int cSpaces = 0;
            int cUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];

                if (ch == ' ')
                    cSpaces++;
                else if (!IsUrlSafeChar( ch ))
                    cUnsafe++;
            }

            // nothing to expand?
            if (cSpaces == 0 && cUnsafe == 0)
            {
                // DevDiv 912606: respect "offset" and "count"
                if (0 == offset && bytes.Length == count)
                {
                    return bytes;
                }
                else
                {
                    var subarray = new byte[count];
                    Buffer.BlockCopy( bytes, offset, subarray, 0, count );
                    return subarray;
                }
            }

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + cUnsafe * 2];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                char ch = (char)b;

                if (IsUrlSafeChar( ch ))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex( (b >> 4) & 0xf );
                    expandedBytes[pos++] = (byte)IntToHex( b & 0x0f );
                }
            }

            return expandedBytes;
        }
    }
}
