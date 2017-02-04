// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Library : ArpanTECH.RSAx
// @CodeProject: http://www.codeproject.com/Articles/421656/RSA-Library-with-Private-Key-Encryption-in-Csharp  

using System;
using System.Numerics;
using System.Security.Cryptography;

namespace ArpanTECH
{
	/// <summary>
	/// Class to keep the basic RSA parameters like Keys, and other information.
	/// </summary>
	public class RSAxParameters : IDisposable
	{
		private HashAlgorithm ha = SHA1.Create();

		public enum RSAxHashAlgorithm { SHA1, SHA256, SHA512, UNDEFINED };

		public void Dispose()
		{
			ha.Dispose();
		}

		/// <summary>
		/// Computes the hash from the given data.
		/// </summary>
		/// <param name="data">The data to hash.</param>
		/// <returns>Hash of the data.</returns>
		public byte[] ComputeHash(byte[] data)
		{
			return ha.ComputeHash(data);
		}

		/// <summary>
		/// Gets and sets the HashAlgorithm for RSA-OAEP padding.
		/// </summary>
		public RSAxHashAlgorithm HashAlgorithm
		{
			get
			{
				RSAxHashAlgorithm al = RSAxHashAlgorithm.UNDEFINED;
				switch (ha.GetType().ToString())
				{
					case "SHA1":
						al = RSAxHashAlgorithm.SHA1;
						break;

					case "SHA256":
						al = RSAxHashAlgorithm.SHA256;
						break;

					case "SHA512":
						al = RSAxHashAlgorithm.SHA512;
						break;
				}
				return al;
			}

			set
			{
				switch (value)
				{
					case RSAxHashAlgorithm.SHA1:
						ha = SHA1.Create();
						HLen = 20;
						break;

					case RSAxHashAlgorithm.SHA256:
						ha = SHA256.Create();
						HLen = 32;
						break;

					case RSAxHashAlgorithm.SHA512:
						ha = SHA512.Create();
						HLen = 64;
						break;
				}
			}
		}

		public bool HasCRTInfo { get; } = false;
		public bool HasPrivateInfo { get; } = false;
		public bool HasPublicInfo { get; } = false;
		public int OctetsInModulus { get; }

		public int HLen { get; private set; } = 20;

		public BigInteger N { get; }
		public BigInteger P { get; }
		public BigInteger Q { get; }
		public BigInteger DP { get; }
		public BigInteger DQ { get; }
		public BigInteger InverseQ { get; }
		public BigInteger E { get; }
		public BigInteger D { get; }

		/// <summary>
		/// Initialize the RSA class. It's assumed that both the Public and Extended Private info are there. 
		/// </summary>
		/// <param name="rsaParams">Preallocated RSAParameters containing the required keys.</param>
		/// <param name="modulusSize">Modulus size in bits</param>
		public RSAxParameters(RSAParameters rsaParams, int modulusSize)
		{
		   // rsaParams;
			OctetsInModulus = modulusSize / 8;
			E = RSAxUtils.OS2IP(rsaParams.Exponent, false);
			D = RSAxUtils.OS2IP(rsaParams.D, false);
			N = RSAxUtils.OS2IP(rsaParams.Modulus, false);
			P = RSAxUtils.OS2IP(rsaParams.P, false);
			Q = RSAxUtils.OS2IP(rsaParams.Q, false);
			DP = RSAxUtils.OS2IP(rsaParams.DP, false);
			DQ = RSAxUtils.OS2IP(rsaParams.DQ, false);
			InverseQ = RSAxUtils.OS2IP(rsaParams.InverseQ, false);
			HasCRTInfo = true;
			HasPublicInfo = true;
			HasPrivateInfo = true;
		}

		/// <summary>
		/// Initialize the RSA class. Only the public parameters.
		/// </summary>
		/// <param name="modulus">Modulus of the RSA key.</param>
		/// <param name="exponent">Exponent of the RSA key</param>
		/// <param name="modulusSize">Modulus size in number of bits. Ex: 512, 1024, 2048, 4096 etc.</param>
		public RSAxParameters(byte[] modulus, byte[] exponent, int modulusSize)
		{
			// rsaParams;
			OctetsInModulus = modulusSize / 8;
			E = RSAxUtils.OS2IP(exponent, false);
			N = RSAxUtils.OS2IP(modulus, false);
			HasPublicInfo = true;
		}

		/// <summary>
		/// Initialize the RSA class.
		/// </summary>
		/// <param name="modulus">Modulus of the RSA key.</param>
		/// <param name="exponent">Exponent of the RSA key</param>
		/// /// <param name="d">Exponent of the RSA key</param>
		/// <param name="modulusSize">Modulus size in number of bits. Ex: 512, 1024, 2048, 4096 etc.</param>
		public RSAxParameters(byte[] modulus, byte[] exponent, byte [] d, int modulusSize)
		{
			// rsaParams;
			OctetsInModulus = modulusSize / 8;
			E = RSAxUtils.OS2IP(exponent, false);
			N = RSAxUtils.OS2IP(modulus, false);
			D = RSAxUtils.OS2IP(d, false);
			HasPublicInfo = true;
			HasPrivateInfo = true;
		}

		/// <summary>
		/// Initialize the RSA class. For CRT.
		/// </summary>
		/// <param name="modulus">Modulus of the RSA key.</param>
		/// <param name="exponent">Exponent of the RSA key</param>
		/// /// <param name="d">Exponent of the RSA key</param>
		/// <param name="p">P paramater of RSA Algorithm.</param>
		/// <param name="q">Q paramater of RSA Algorithm.</param>
		/// <param name="dp">DP paramater of RSA Algorithm.</param>
		/// <param name="dq">DQ paramater of RSA Algorithm.</param>
		/// <param name="inverseQ">InverseQ paramater of RSA Algorithm.</param>
		/// <param name="modulusSize">Modulus size in number of bits. Ex: 512, 1024, 2048, 4096 etc.</param>
		public RSAxParameters(byte[] modulus, byte[] exponent, byte[] d, byte[] p, byte [] q, byte [] dp, byte [] dq, byte [] inverseQ, int modulusSize)
		{
			// rsaParams;
			OctetsInModulus = modulusSize / 8;
			E = RSAxUtils.OS2IP(exponent, false);
			N = RSAxUtils.OS2IP(modulus, false);
			D = RSAxUtils.OS2IP(d, false);
			P = RSAxUtils.OS2IP(p, false);
			Q = RSAxUtils.OS2IP(q, false);
			DP = RSAxUtils.OS2IP(dp, false);
			DQ = RSAxUtils.OS2IP(dq, false);
			InverseQ = RSAxUtils.OS2IP(inverseQ, false);
			HasCRTInfo = true;
			HasPublicInfo = true;
			HasPrivateInfo = true;
		}

	}
}
