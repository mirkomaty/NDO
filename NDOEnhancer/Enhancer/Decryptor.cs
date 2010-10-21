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
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung f�r Decryptor.
	/// </summary>
	internal class Decryptor
	{
		public static string Decrypt(string encr)
		{
			ASCIIEncoding textConverter = new ASCIIEncoding();
			byte[] key = {0x03,0x2A,0xB0,0x4C,0x22,0x11,0x45,0x21,0x2F,0x3F,0x0C,0xD7,0x2A,0x50,0xEB,0x48,0x5C,0x06,0xEB,0x85,0xD5,0xBD,0xC6,0x06,0x79,0xFB,0xDE,0x16,0x43,0x12,0x78,0xC1};
			byte[] iV = {0xD8,0x75,0xC2,0x87,0x2E,0x10,0x60,0xB5,0xAC,0x90,0x3E,0x2A,0x55,0xCB,0xB0,0x8B};
			RijndaelManaged myRijndael = new RijndaelManaged();

			//Get a decryptor that uses the same key and IV as the encryptor.
			ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, iV);

			byte[] encrypted = new byte[encr.Length / 2];
			for (int i = 0; i < encr.Length; i += 2)
			{
				encrypted[i / 2] = byte.Parse(encr.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
			}
			//Now decrypt the previously encrypted message using the decryptor
			// obtained in the above step.
			MemoryStream msDecrypt = new MemoryStream(encrypted);
			CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

			byte[] fromEncrypt = new byte[encrypted.Length];

			//Read the data out of the crypto stream.
			csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

			//Convert the byte array back into a string.
			return textConverter.GetString(fromEncrypt);
		}

	}
}
