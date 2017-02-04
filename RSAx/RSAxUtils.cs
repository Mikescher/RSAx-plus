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
		/// <param name="xmlKeyInfo">Key Data.</param>
		/// <param name="modulusSize">RSA Modulus Size</param>
		/// <returns>RSAxParameters class</returns>
		public static RSAxParameters GetRSAxParameters(string xmlKeyInfo, int modulusSize)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(xmlKeyInfo);
			}
			catch (Exception ex)
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

			var docelem = doc.DocumentElement;

			if (docelem == null) throw new Exception("Could not process XMLKeyInfo. No key information.");

			var nodeMod = docelem.SelectSingleNode("Modulus");
			var nodeExp = docelem.SelectSingleNode("Exponent");
			var nodeD = docelem.SelectSingleNode("D");
			var nodeP = docelem.SelectSingleNode("P");
			var nodeQ = docelem.SelectSingleNode("Q");
			var nodeDP = docelem.SelectSingleNode("DP");
			var nodeDQ = docelem.SelectSingleNode("DQ");
			var nodeInvQ = docelem.SelectSingleNode("InverseQ");

			if (nodeMod != null)  modulus  = Convert.FromBase64String(nodeMod.InnerText);
			if (nodeExp != null)  exponent = Convert.FromBase64String(nodeExp.InnerText);
			if (nodeD != null)    d        = Convert.FromBase64String(nodeD.InnerText);
			if (nodeP != null)    p        = Convert.FromBase64String(nodeP.InnerText);
			if (nodeQ != null)    q        = Convert.FromBase64String(nodeQ.InnerText);
			if (nodeDP != null)   dp       = Convert.FromBase64String(nodeDP.InnerText);
			if (nodeDQ != null)   dq       = Convert.FromBase64String(nodeDQ.InnerText);
			if (nodeInvQ != null) inverseQ = Convert.FromBase64String(nodeInvQ.InnerText);

			var hasPublicInfo = (nodeMod != null) && (nodeExp != null);
			var hasPrivateInfo = hasPublicInfo && (nodeD != null);
			var hasCRTInfo = (nodeMod != null) && (nodeP != null) && (nodeQ != null) && (nodeDP != null) && (nodeDQ != null) && (nodeInvQ != null);

			if (hasCRTInfo && hasPrivateInfo)
				return new RSAxParameters(modulus, exponent, d, p, q, dp, dq, inverseQ, modulusSize);

			if (hasPrivateInfo)
				return new RSAxParameters(modulus, exponent, d, modulusSize);

			if (hasPublicInfo)
				return new RSAxParameters(modulus, exponent, modulusSize);

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

			byte[] r = new byte[A.Length];
			for (int i = 0; i < A.Length; i++)
			{
				r[i] = (byte)(A[i] ^ B[i]);
			}
			return r;
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
