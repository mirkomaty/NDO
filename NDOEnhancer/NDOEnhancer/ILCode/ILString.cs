//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Globalization;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Zusammenfassung für ILString.
	/// </summary>
	internal class ILString
	{
		public static string DecodeBytes(string bytes)
		{
			string[] arr = bytes.Trim().Split(' ');
			// First byte is the length of the string
			byte len = byte.Parse(arr[0], NumberStyles.HexNumber);
			if (len > arr.Length - 1)
				throw new Exception("Internal Error #17 in ILString::DecodeBytes");

			byte[] barr = new byte[len];

			for (int i = 0; i < len; i++)
			{
				string s = arr[i + 1];
				if (s == string.Empty)
					barr[i] = 0;
				else
					barr[i] = byte.Parse(s, NumberStyles.HexNumber);
			}

			return new System.Text.UTF8Encoding().GetString(barr);
		}

		public static string CodeBytes(string s)
		{
			string bytes = string.Empty;
			byte[] arr = new System.Text.UTF8Encoding().GetBytes(s);
			int len = arr.Length;
			bytes = len.ToString("X2") + ' ';
			foreach(byte b in arr)
				bytes += b.ToString("X2") + ' ';
			return bytes.Substring(0, bytes.Length - 1);
		}

	}
}
