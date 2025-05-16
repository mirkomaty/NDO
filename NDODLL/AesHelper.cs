using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace NDO
{
	class AesHelper
	{

		public static string Encrypt( string s, byte[] privateKey )
		{
			if (String.IsNullOrEmpty( s ))
				return String.Empty;

			using (var aes = Aes.Create())
			{
				aes.Key = privateKey;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;
				var input = Encoding.UTF8.GetBytes(s);
				aes.GenerateIV();  // This creates the Initialization Vector
				var iv = aes.IV;
				using (var encrypter = aes.CreateEncryptor( privateKey, iv ))
				using (var cipherStream = new MemoryStream())
				{
					using (var tCryptoStream = new CryptoStream( cipherStream, encrypter, CryptoStreamMode.Write ))
					using (var tBinaryWriter = new BinaryWriter( tCryptoStream ))
					{
						// Prepend IV to data.
						// Write directly to the Memorystream
						cipherStream.Write( iv, 0, iv.Length );
						tBinaryWriter.Write( input );
						tCryptoStream.FlushFinalBlock();
					}

					return Convert.ToBase64String( cipherStream.ToArray() );
				}
			}
		}

		public static string Decrypt( string s, byte[] privateKey )
		{
			if (s == null)
				return null;
			if (s == String.Empty)
				return String.Empty;
			var aes = Aes.Create();
			aes.Key = privateKey;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			byte[] input = Convert.FromBase64String(s);

			// Get first 16 bytes as IV and use it to decrypt
			var iv = new byte[16];
			Array.Copy( input, 0, iv, 0, iv.Length );

			using (var ms = new MemoryStream())
			{
				using (var cs = new CryptoStream( ms, aes.CreateDecryptor( privateKey, iv ), CryptoStreamMode.Write ))
				using (var binaryWriter = new BinaryWriter( cs ))
				{
					// Decrypt Cipher Text from Message
					binaryWriter.Write(
						input,
						iv.Length,
						input.Length - iv.Length
					);
				}

				return Encoding.UTF8.GetString( ms.ToArray() );
			}
		}
	}
}
