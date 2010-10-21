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
using System.Globalization;

namespace ILCode
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
