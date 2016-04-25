//
// Copyright (c) 2002-2016 Mirko Matytschak 
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


using System;

namespace ILCode
{
	/// <summary>
	/// Zusammenfassung für PackedLength.
	/// </summary>
	internal class PackedLength
	{
/*
·	If the value lies between 0 (0x00) and 127 (0x7F), inclusive, encode as a one-byte integer (bit #7 is clear, value held in bits #6 through #0)
·	If the value lies between 2^8 (0x80) and 2^14 – 1 (0x3FFF), inclusive, encode as a two-byte integer with bit #15 set, bit #14 clear (value held in bits #13 through #0)
·	Otherwise, encode as a 4-byte integer, with bit #31 set, bit #30 set, bit #29 clear (value held in bits #28 through #0)
·	A null string should be represented with the reserved single byte 0xFF, and no following data
*/

		public static int Read(byte[] arr, ref int fromPos)
		{
			Byte b = arr[fromPos++];
			int result;
			if (b < 127)
				return (int) b;
			if (b == 255)
				return -1;
			byte tester = (byte) (b & 0xC0);
			if (tester == 0x80)  // 2 byte
			{
				result = b & 0x3f;
				result <<= 8;
				result |= arr[fromPos++];
			}
			else if (tester == 0xc0)
			{
				result = b & 0x1f;
				for (int i = 0; i < 3; i++)
				{
					result <<= 8;
					result |= arr[fromPos++];
				}
			}
			else 
				throw new Exception("Packed Length: Wrong tester: " + tester.ToString("X2"));
			return result;			
		}
	}
}
