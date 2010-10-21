//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


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
