using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	public class Cryptography
	{
		private readonly string pass;
		private readonly string salt;

		public Cryptography()
		{
			var l = ReadonlyVars.UserName.ToArray();
			var sb = new StringBuilder();
			l.ToList().ForEach(x => sb.Append(x).Append(ReadonlyVars.MachineName));
			pass = sb.ToString() + "reghesdehmbpomp";
			salt = sb.ToString().ToUpper().Reverse() + "4614151651";
		}

		//public byte[] Encrypt(string text)
		//{
		//	var rijndael = new RijndaelManaged();

		//	var deriveBytes = new Rfc2898DeriveBytes(s, Encoding.UTF8.GetBytes(pass), 1000);
		//	rijndael.IV = deriveBytes.GetBytes(rijndael.BlockSize / 8);
		//	rijndael.Key = deriveBytes.GetBytes(rijndael.KeySize / 8);

		//	byte[] strBytes = Encoding.UTF8.GetBytes(text);
		//	byte[] encBytes = null;
		//	using (ICryptoTransform encryptor = rijndael.CreateEncryptor())
		//	{
		//		encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
		//	}

		//	return encBytes;
		//}

		//public string Decrypt(byte[] s)
		//{
		//		var rijndael = new RijndaelManaged();
		//	var deriveBytes = new Rfc2898DeriveBytes(s, Encoding.UTF8.GetBytes(pass), 1000);
		//	rijndael.IV = deriveBytes.GetBytes(rijndael.BlockSize /8);
		//	rijndael.Key = deriveBytes.GetBytes(rijndael.KeySize / 8);

		//	byte[] decBytes = null;
		//	using (ICryptoTransform decryptor = rijndael.CreateDecryptor())
		//	{
		//		decBytes = decryptor.TransformFinalBlock(s, 0, s.Length);
		//	}

		//	return Encoding.UTF8.GetString(decBytes);
		//}

		public byte[] Encrypt(string str)
		{

			var rijndael = new RijndaelManaged();
			byte[] key, iv;
			GenerateKeyFromPassword(
				pass, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
			rijndael.Key = key;
			rijndael.IV = iv;

			var strBytes = Encoding.UTF8.GetBytes(str);

			var encryptor = rijndael.CreateEncryptor();
			var encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);

			encryptor.Dispose();
			return encBytes;
		}

		public string Decrypt(byte[] b)
		{

			var rijndael = new RijndaelManaged();

			byte[] key, iv;
			GenerateKeyFromPassword(
				pass, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
			rijndael.Key = key;
			rijndael.IV = iv;


			var decryptor =	rijndael.CreateDecryptor();

			var decBytes = decryptor.TransformFinalBlock(b, 0, b.Length);

			decryptor.Dispose();

			return Encoding.UTF8.GetString(decBytes);
		}

		private void GenerateKeyFromPassword(string password,
			int keySize, out byte[] key, int blockSize, out byte[] iv)
		{
			var salt = Encoding.UTF8.GetBytes(this.salt);
			var deriveBytes = new Rfc2898DeriveBytes(password, salt);
			deriveBytes.IterationCount = 1000;

			key = deriveBytes.GetBytes(keySize / 8);
			iv = deriveBytes.GetBytes(blockSize / 8);
		}
	}
}
