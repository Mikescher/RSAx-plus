﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace RSAxPlus.Utils
{
	public static class PEMUtils
	{
		public static string PrivateXKMSKeyToPEM(string keyxml)
		{
			var rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(keyxml);
			
			return ExportPrivateKey(rsa);
		}

		/// <summary>
		/// http://stackoverflow.com/a/23739932/1761622
		/// </summary>
		private static string ExportPrivateKey(RSACryptoServiceProvider csp)
		{
			TextWriter outputStream = new StringWriter();

			if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", nameof(csp));
			var parameters = csp.ExportParameters(true);
			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				writer.Write((byte)0x30); // SEQUENCE
				using (var innerStream = new MemoryStream())
				{
					var innerWriter = new BinaryWriter(innerStream);
					EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
					EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
					EncodeIntegerBigEndian(innerWriter, parameters.D);
					EncodeIntegerBigEndian(innerWriter, parameters.P);
					EncodeIntegerBigEndian(innerWriter, parameters.Q);
					EncodeIntegerBigEndian(innerWriter, parameters.DP);
					EncodeIntegerBigEndian(innerWriter, parameters.DQ);
					EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
					var length = (int)innerStream.Length;
					EncodeLength(writer, length);
					writer.Write(innerStream.GetBuffer(), 0, length);
				}

				var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
				outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
				// Output as Base64 with lines chopped at 64 characters
				for (var i = 0; i < base64.Length; i += 64)
				{
					outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
				}
				outputStream.WriteLine("-----END RSA PRIVATE KEY-----");

				return outputStream.ToString();
			}
		}

		public static string PublicXKMSKeyToPEM(string keyxml)
		{
			var rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(keyxml);

			return ExportPublicKeyToPEMFormat(rsa);
		}

		/// <summary>
		/// http://stackoverflow.com/a/25591659/1761622
		/// </summary>
		private static string ExportPublicKeyToPEMFormat(RSACryptoServiceProvider csp)
		{
			TextWriter outputStream = new StringWriter();

			var parameters = csp.ExportParameters(false);
			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				writer.Write((byte)0x30); // SEQUENCE
				using (var innerStream = new MemoryStream())
				{
					var innerWriter = new BinaryWriter(innerStream);
					EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
					EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent);

					//All Parameter Must Have Value so Set Other Parameter Value Whit Invalid Data  (for keeping Key Structure  use "parameters.Exponent" value for invalid data)
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.D
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.P
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.Q
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.DP
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.DQ
					EncodeIntegerBigEndian(innerWriter, parameters.Exponent); // instead of parameters.InverseQ

					var length = (int)innerStream.Length;
					EncodeLength(writer, length);
					writer.Write(innerStream.GetBuffer(), 0, length);
				}

				var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
				outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
				// Output as Base64 with lines chopped at 64 characters
				for (var i = 0; i < base64.Length; i += 64)
				{
					outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
				}
				outputStream.WriteLine("-----END PUBLIC KEY-----");

				return outputStream.ToString();

			}
		}

		private static void EncodeLength(BinaryWriter stream, int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative");
			if (length < 0x80)
			{
				// Short form
				stream.Write((byte)length);
			}
			else
			{
				// Long form
				var temp = length;
				var bytesRequired = 0;
				while (temp > 0)
				{
					temp >>= 8;
					bytesRequired++;
				}
				stream.Write((byte)(bytesRequired | 0x80));
				for (var i = bytesRequired - 1; i >= 0; i--)
				{
					stream.Write((byte)(length >> (8 * i) & 0xff));
				}
			}
		}

		private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
		{
			stream.Write((byte)0x02); // INTEGER
			var prefixZeros = value.TakeWhile(b => b == 0).Count();
			if (value.Length - prefixZeros == 0)
			{
				EncodeLength(stream, 1);
				stream.Write((byte)0);
			}
			else
			{
				if (forceUnsigned && value[prefixZeros] > 0x7f)
				{
					// Add a prefix zero to force unsigned if the MSB is 1
					EncodeLength(stream, value.Length - prefixZeros + 1);
					stream.Write((byte)0);
				}
				else
				{
					EncodeLength(stream, value.Length - prefixZeros);
				}
				for (var i = prefixZeros; i < value.Length; i++)
				{
					stream.Write(value[i]);
				}
			}
		}
	}
}
