// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Library : ArpanTECH.RSAx
// @CodeProject: http://www.codeproject.com/Articles/421656/RSA-Library-with-Private-Key-Encryption-in-Csharp  

using System;
using System.Numerics;
using System.Xml;

namespace ArpanTECH
{
	/// <summary>
	/// Utility class for RSAx
	/// </summary>
	public static class RSAxUtils
	{
		/// <summary>
		/// Creates a RSAxParameters class from a given XMLKeyInfo string.
		/// </summary>
		/// <param name="XMLKeyInfo">Key Data.</param>
		/// <param name="ModulusSize">RSA Modulus Size</param>
		/// <returns>RSAxParameters class</returns>
		public static RSAxParameters GetRSAxParameters(string XMLKeyInfo, int ModulusSize)
		{
			bool hasCRTInfo = false;
			bool hasPrivateInfo = false;
			bool hasPublicInfo = false;

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(XMLKeyInfo);
			}
			catch (System.Exception ex)
			{
				throw new Exception("Malformed KeyInfo XML: " + ex.Message);
			}

			byte[] modulus = new byte[0];
			byte[] exponent = new byte[0];
			byte[] d = new byte[0];
			byte[] p = new byte[0];
			byte[] q = new byte[0];
			byte[] dp = new byte[0];
			byte[] dq = new byte[0]; 
			byte[] inverseQ = new byte[0];

			try
			{
				modulus = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Modulus").InnerText);
				exponent = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Exponent").InnerText);
				hasPublicInfo = true;
			}
			catch { }

			try
			{
				modulus = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Modulus").InnerText);
				d = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("D").InnerText);
				exponent = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Exponent").InnerText);
				hasPrivateInfo = true;
			}
			catch { }

			try
			{
				modulus = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Modulus").InnerText);
				p = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("P").InnerText);
				q = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("Q").InnerText);
				dp = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("DP").InnerText);
				dq = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("DQ").InnerText);
				inverseQ = Convert.FromBase64String(doc.DocumentElement.SelectSingleNode("InverseQ").InnerText);
				hasCRTInfo = true;
			}
			catch { }

			if (hasCRTInfo && hasPrivateInfo)
			{
				return new RSAxParameters(modulus, exponent, d, p, q, dp, dq, inverseQ, ModulusSize);
			}
			else if (hasPrivateInfo)
			{
				return new RSAxParameters(modulus, exponent, d, ModulusSize);
			}
			else if (hasPublicInfo)
			{
				return new RSAxParameters(modulus, exponent, ModulusSize);
			}

			throw new Exception("Could not process XMLKeyInfo. Incomplete key information.");

			
		}

		/// <summary>
		/// Converts a non-negative integer to an octet string of a specified length.
		/// </summary>
		/// <param name="x">The integer to convert.</param>
		/// <param name="xLen">Length of output octets.</param>
		/// <param name="makeLittleEndian">If True little-endian converntion is followed, big-endian otherwise.</param>
		/// <returns></returns>
		public static byte[] I2OSP(BigInteger x, int xLen, bool makeLittleEndian)
		{
			byte[] result = new byte[xLen];
			int index = 0;
			while ((x > 0) && (index < result.Length))
			{
				result[index++] = (byte)(x % 256);
				x /= 256;
			}
			if (!makeLittleEndian)
				Array.Reverse(result);
			return result;
		}

		/// <summary>
		/// Converts a byte array to a non-negative integer.
		/// </summary>
		/// <param name="data">The number in the form of a byte array.</param>
		/// <param name="isLittleEndian">Endianness of the byte array.</param>
		/// <returns>An non-negative integer from the byte array of the specified endianness.</returns>
		public static BigInteger OS2IP(byte[] data, bool isLittleEndian)
		{
			BigInteger bi = 0;
			if (isLittleEndian)
			{
				for (int i = 0; i < data.Length; i++)
				{
					bi += BigInteger.Pow(256, i) * data[i];
				}
			}
			else
			{
				for (int i = 1; i <= data.Length; i++)
				{
					bi += BigInteger.Pow(256, i - 1) * data[data.Length - i];
				}
			}
			return bi;
		}

		/// <summary>
		/// Performs Bitwise Ex-OR operation to two given byte arrays.
		/// </summary>
		/// <param name="A">The first byte array.</param>
		/// <param name="B">The second byte array.</param>
		/// <returns>The bitwise Ex-OR result.</returns>
		public static byte[] XOR(byte[] A, byte[] B)
		{
			if (A.Length != B.Length)
			{
				throw new ArgumentException("XOR: Parameter length mismatch");
			}
			else
			{
				byte[] R = new byte[A.Length];

				for (int i = 0; i < A.Length; i++)
				{
					R[i] = (byte)(A[i] ^ B[i]);
				}
				return R;
			}
		}

		internal static void FixByteArraySign(ref byte[] bytes)
		{
			if ((bytes[bytes.Length - 1] & 0x80) > 0)
			{
				byte[] temp = new byte[bytes.Length];
				Array.Copy(bytes, temp, bytes.Length);
				bytes = new byte[temp.Length + 1];
				Array.Copy(temp, bytes, temp.Length);
			}
		}


	}
}
