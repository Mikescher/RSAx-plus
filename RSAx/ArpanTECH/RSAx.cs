// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Library : ArpanTECH.RSAx
// @CodeProject: http://www.codeproject.com/Articles/421656/RSA-Library-with-Private-Key-Encryption-in-Csharp  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace RSAxPlus.ArpanTECH
{
	/// <summary>
	/// The main RSAx Class
	/// </summary>
	public class RSAx : IDisposable
	{
		private readonly RSAxParameters rsaParams;
		private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		/// <summary>
		/// Initialize the RSA class.
		/// </summary>
		/// <param name="rsaParams">Preallocated RSAxParameters containing the required keys.</param>
		/// <param name="useCRT">uses CRT for private key decryption</param>
		private RSAx(RSAxParameters rsaParams, bool useCRT)
		{
			this.rsaParams = rsaParams;
			UseCRTForPublicDecryption = useCRT;
		}

		/// <summary>
		/// Initialize the RSA class from a XML KeyInfo string.
		/// </summary>
		/// <param name="keyInfo">XML Containing Key Information</param>
		/// <param name="modulusSize">Length of RSA Modulus in bits.</param>
		public static RSAx CreateFromXML(string keyInfo, int modulusSize)
		{
			return new RSAx(RSAxUtils.GetRSAxParameters(keyInfo, modulusSize), true);
		}

		/// <summary>
		/// Initialize the RSA class from a PEM string.
		/// </summary>
		/// <param name="keyInfo">PEM string</param>
		/// <param name="pw">password</param>
		public static RSAx CreateFromPEM(string keyInfo, string pw = "")
		{
			var key = OpenSSLKey.OpenSSLKey.PEMKeyToXMLKey(keyInfo, pw);
			if (key.KeyPrivate != null)
				return new RSAx(RSAxUtils.GetRSAxParameters(key.KeyPrivate, key.KeySize), true);
			else
				return new RSAx(RSAxUtils.GetRSAxParameters(key.KeyPublic, key.KeySize), true);
		}

		/// <summary>
		/// Hash Algorithm to be used for OAEP encoding.
		/// </summary>
		public RSAxParameters.RSAxHashAlgorithm RSAxHashAlgorithm
		{
			set
			{
				rsaParams.HashAlgorithm = value;
			}
		}

		/// <summary>
		/// If True, and if the parameters are available, uses CRT for private key decryption. (Much Faster)
		/// </summary>
		public bool UseCRTForPublicDecryption { get; set; }

		/// <summary>
		/// Releases all the resources.
		/// </summary>
		public void Dispose()
		{
			rsaParams.Dispose();
		}

		#region PRIVATE FUNCTIONS

		/// <summary>
		/// Low level RSA Process function for use with private key.
		/// Should never be used; Because without padding RSA is vulnerable to attacks.  Use with caution.
		/// </summary>
		/// <param name="plainText">Data to encrypt. Length must be less than Modulus size in octets.</param>
		/// <param name="usePrivate">True to use Private key, else Public.</param>
		/// <returns>Encrypted Data</returns>
		private byte[] RSAProcess(byte[] plainText, bool usePrivate)
		{
			if (usePrivate && !rsaParams.HasPrivateInfo)
				throw new CryptographicException("RSA Process: Incomplete Private Key Info");

			if (usePrivate == false && !rsaParams.HasPublicInfo)
				throw new CryptographicException("RSA Process: Incomplete Public Key Info");

			BigInteger e = usePrivate ? rsaParams.D : rsaParams.E;

			BigInteger pt = RSAxUtils.OS2IP(plainText, false);
			BigInteger m = BigInteger.ModPow(pt, e, rsaParams.N);
			
			if (m.Sign == -1)
				return RSAxUtils.I2OSP(m + rsaParams.N, rsaParams.OctetsInModulus, false);
			else
				return RSAxUtils.I2OSP(m, rsaParams.OctetsInModulus, false);
		}

		/// <summary>
		/// Low level RSA Decryption function for use with private key. Uses CRT and is Much faster.
		/// Should never be used; Because without padding RSA is vulnerable to attacks. Use with caution.
		/// </summary>
		/// <param name="data">Data to encrypt. Length must be less than Modulus size in octets.</param>
		/// <returns>Encrypted Data</returns>
		private byte[] RSADecryptPrivateCRT(byte[] data)
		{
			if (rsaParams.HasPrivateInfo && rsaParams.HasCRTInfo)
			{
				BigInteger c = RSAxUtils.OS2IP(data, false);

				BigInteger m1 = BigInteger.ModPow(c, rsaParams.DP, rsaParams.P);
				BigInteger m2 = BigInteger.ModPow(c, rsaParams.DQ, rsaParams.Q);
				BigInteger h = (m1 - m2) * rsaParams.InverseQ % rsaParams.P;
				BigInteger m = m2 + rsaParams.Q * h;

				if (m.Sign == -1)
					return RSAxUtils.I2OSP(m + rsaParams.N, rsaParams.OctetsInModulus, false);
				else
					return RSAxUtils.I2OSP(m, rsaParams.OctetsInModulus, false); 
			}
			else
			{
				throw new CryptographicException("RSA Decrypt CRT: Incomplete Key Info");
			}
		}
		
		private byte[] RSAProcessEncodePKCS(byte[] message, bool usePrivate)
		{
			if (message.Length > rsaParams.OctetsInModulus - 11)
			{
				throw new ArgumentException("Message too long.");
			}
			else
			{
				// RFC3447 : Page 24. [RSAES-PKCS1-V1_5-ENCRYPT ((n, e), M)]
				// EM = 0x00 || 0x02 || PS || 0x00 || Msg 

				List<byte> pckSv15Msg = new List<byte> {0x00, 0x02};
				
				int paddingLength = rsaParams.OctetsInModulus - message.Length - 3;

				byte[] ps = new byte[paddingLength];
				rng.GetNonZeroBytes(ps);

				pckSv15Msg.AddRange(ps);
				pckSv15Msg.Add(0x00);

				pckSv15Msg.AddRange(message);

				return RSAProcess(pckSv15Msg.ToArray() ,  usePrivate);
			}
		}

		/// <summary>
		/// Mask Generation Function
		/// </summary>
		/// <param name="z">Initial pseudorandom Seed.</param>
		/// <param name="l">Length of output required.</param>
		/// <returns></returns>
		private byte[] MGF(byte[] z, int l)
		{
			if (l > Math.Pow(2, 32))
			{
				throw new ArgumentException("Mask too long.");
			}

			List<byte> result = new List<byte>();
			for (int i = 0; i <= l / rsaParams.HLen; i++)
			{
				List<byte> data = new List<byte>();
				data.AddRange(z);
				data.AddRange(RSAxUtils.I2OSP(i, 4, false));
				result.AddRange(rsaParams.ComputeHash(data.ToArray()));
			}

			if (l > result.Count)
			{
				throw new ArgumentException("Invalid Mask Length.");
			}

			return result.GetRange(0, l).ToArray();

		}

	   
		private byte[] RSAProcessEncodeOAEP(byte[] m, byte[] p, bool usePrivate)
		{
			//                           +----------+---------+-------+
			//                      DB = |  lHash   |    PS   |   M   |
			//                           +----------+---------+-------+
			//                                          |
			//                +----------+              V
			//                |   seed   |--> MGF ---> XOR
			//                +----------+              |
			//                      |                   |
			//             +--+     V                   |
			//             |00|    XOR <----- MGF <-----|
			//             +--+     |                   |
			//               |      |                   |
			//               V      V                   V
			//             +--+----------+----------------------------+
			//       EM =  |00|maskedSeed|          maskedDB          |
			//             +--+----------+----------------------------+

			int mLen = m.Length;
			if (mLen > rsaParams.OctetsInModulus - 2 * rsaParams.HLen - 2)
			{
				throw new ArgumentException("Message too long.");
			}
			else
			{
				byte[] ps = new byte[rsaParams.OctetsInModulus - mLen - 2 * rsaParams.HLen - 2];
				//4. pHash = Hash(P),
				byte[] pHash = rsaParams.ComputeHash(p);
				
				//5. DB = pHash||PS||01||M.
				List<byte> dblist = new List<byte>();
				dblist.AddRange(pHash);
				dblist.AddRange(ps);
				dblist.Add(0x01);
				dblist.AddRange(m);
				byte[] db = dblist.ToArray();

				//6. Generate a random octet string seed of length hLen.
				byte[] seed = new byte[rsaParams.HLen];
				rng.GetBytes(seed);

				//7. dbMask = MGF(seed, k - hLen -1).
				byte[] dbMask = MGF(seed, rsaParams.OctetsInModulus - rsaParams.HLen - 1);

				//8. maskedDB = DB XOR dbMask
				byte[] maskedDB = RSAxUtils.XOR(db, dbMask);

				//9. seedMask = MGF(maskedDB, hLen)
				byte[] seedMask = MGF(maskedDB, rsaParams.HLen);

				//10. maskedSeed = seed XOR seedMask.
				byte[] maskedSeed = RSAxUtils.XOR(seed, seedMask);

				//11. EM = 0x00 || maskedSeed || maskedDB.
				var result = new byte[] { 0x00 }.Concat(maskedSeed).Concat(maskedDB).ToArray();

				return RSAProcess(result, usePrivate);
			}
		}

		
		private byte[] Decrypt(byte[] message, byte [] parameters, bool usePrivate, bool fOAEP)
		{
			byte[] em;
			try
			{
				if (usePrivate && UseCRTForPublicDecryption && rsaParams.HasCRTInfo)
				{
					em = RSADecryptPrivateCRT(message);
				}
				else
				{
					em = RSAProcess(message, usePrivate);
				}
			}
			catch (CryptographicException ex)
			{
				throw new CryptographicException("Exception while Decryption: " + ex.Message);
			}
			catch
			{
				throw new Exception("Exception while Decryption: ");
			}

			try
			{
				if (fOAEP) //DECODE OAEP
				{
					if (em.Length == rsaParams.OctetsInModulus && em.Length > 2 * rsaParams.HLen + 1)
					{
						byte[] pHash = rsaParams.ComputeHash(parameters);
						if (em[0] == 0) // RFC3447 Format : http://tools.ietf.org/html/rfc3447
						{
							var maskedSeed = em.ToList().GetRange(1, rsaParams.HLen).ToArray();
							var maskedDB = em.ToList().GetRange(1 + rsaParams.HLen, em.Length - rsaParams.HLen - 1).ToArray();
							var seedMask = MGF(maskedDB, rsaParams.HLen);
							var seed = RSAxUtils.XOR(maskedSeed, seedMask);
							var dbMask = MGF(seed, rsaParams.OctetsInModulus - rsaParams.HLen - 1);
							var db = RSAxUtils.XOR(maskedDB, dbMask);

							if (db.Length >= rsaParams.HLen + 1)
							{
								byte[] pHashInner = db.ToList().GetRange(0, rsaParams.HLen).ToArray();
								List<byte> psM = db.ToList().GetRange(rsaParams.HLen, db.Length - rsaParams.HLen);
								int pos = psM.IndexOf(0x01);
								if (pos >= 0 && pos < psM.Count)
								{
									List<byte> list01M = psM.GetRange(pos, psM.Count - pos);
									byte[] m;
									if (list01M.Count > 1)
									{
										m = list01M.GetRange(1, list01M.Count - 1).ToArray();
									}
									else
									{
										m = new byte[0];
									}
									bool success = true;
									for (int i = 0; i < rsaParams.HLen; i++)
									{
										if (pHashInner[i] != pHash[i])
										{
											success = false;
											break;
										}
									}

									if (success)
									{
										return m;
									}

									throw new CryptographicException("OAEP Decode Error");
								}
								// #3: Invalid Encoded Message Length.
								throw new CryptographicException("OAEP Decode Error");
							}
							// #2: Invalid Encoded Message Length.
							throw new CryptographicException("OAEP Decode Error");
						}
						// Standard : ftp://ftp.rsasecurity.com/pub/rsalabs/rsa_algorithm/rsa-oaep_spec.pdf
						//OAEP : THIS STADNARD IS NOT IMPLEMENTED
						throw new CryptographicException("OAEP Decode Error");
					}
					// #1: Invalid Encoded Message Length.
					throw new CryptographicException("OAEP Decode Error");
				}

				// DECODE PKCS v1.5

				if (em.Length >= 11)
				{
					if (em[0] == 0x00 && em[1] == 0x02)
					{
						int startIndex = 2;
						List<byte> ps = new List<byte>();
						for (int i = startIndex; i < em.Length; i++)
						{
							if (em[i] != 0)
							{
								ps.Add(em[i]);
							}
							else
							{
								break;
							}
						}

						if (ps.Count >= 8)
						{
							int decodedDataIndex = startIndex + ps.Count + 1;
							if (decodedDataIndex < em.Length - 1)
							{
								List<byte> data = new List<byte>();
								for (int i = decodedDataIndex; i < em.Length; i++)
								{
									data.Add(em[i]);
								}
								return data.ToArray();
							}

							return new byte[0];
							//throw new CryptographicException("PKCS v1.5 Decode Error #4: No Data");
						}

						// #3: Invalid Key / Invalid Random Data Length
						throw new CryptographicException("PKCS v1.5 Decode Error");
					}

					// #2: Invalid Key / Invalid Identifiers
					throw new CryptographicException("PKCS v1.5 Decode Error");
				}

				// #1: Invalid Key / PKCS Encoding
				throw new CryptographicException("PKCS v1.5 Decode Error");
			}
			catch (CryptographicException ex)
			{
				throw new CryptographicException("Exception while decoding: " + ex.Message);
			}
			catch
			{
				throw new CryptographicException("Exception while decoding");
			}
		}

		#endregion

		#region PUBLIC FUNCTIONS

		/// <summary>
		/// Encrypts the given message with RSA, performs OAEP Encoding.
		/// </summary>
		/// <param name="message">Message to Encrypt. Maximum message length is (ModulusLengthInOctets - 2 * HashLengthInOctets - 2)</param>
		/// <param name="oaepParams">Optional OAEP parameters. Normally Empty. But, must match the parameters while decryption.</param>
		/// <param name="usePrivate">True to use Private key for encryption. False to use Public key.</param>
		/// <returns>Encrypted message.</returns>
		public byte[] Encrypt(byte[] message, byte[] oaepParams, bool usePrivate)
		{
			return RSAProcessEncodeOAEP(message, oaepParams, usePrivate);
		}

		/// <summary>
		/// Encrypts the given message with RSA.
		/// </summary>
		/// <param name="message">Message to Encrypt. Maximum message length is For OAEP [ModulusLengthInOctets - (2 * HashLengthInOctets) - 2] and for PKCS [ModulusLengthInOctets - 11]</param>
		/// <param name="usePrivate">True to use Private key for encryption. False to use Public key.</param>
		/// <param name="fOAEP">True to use OAEP encoding (Recommended), False to use PKCS v1.5 Padding.</param>
		/// <returns>Encrypted message.</returns>
		public byte[] Encrypt(byte[] message, bool usePrivate, bool fOAEP)
		{
			if (fOAEP)
			{
				return RSAProcessEncodeOAEP(message, new byte[0], usePrivate);
			}
			else
			{
				return RSAProcessEncodePKCS(message, usePrivate);
			}
		}

		/// <summary>
		/// Encrypts the given message using RSA Public Key.
		/// </summary>
		/// <param name="message">Message to Encrypt. Maximum message length is For OAEP [ModulusLengthInOctets - (2 * HashLengthInOctets) - 2] and for PKCS [ModulusLengthInOctets - 11]</param>
		/// <param name="fOAEP">True to use OAEP encoding (Recommended), False to use PKCS v1.5 Padding.</param>
		/// <returns>Encrypted message.</returns>
		public byte[] Encrypt(byte[] message,  bool fOAEP)
		{
			if (fOAEP)
			{
				return RSAProcessEncodeOAEP(message, new byte[0], false);
			}
			else
			{
				return RSAProcessEncodePKCS(message, false);
			}
		}
		
		/// <summary>
		/// Decrypts the given RSA encrypted message.
		/// </summary>
		/// <param name="message">The encrypted message.</param>
		/// <param name="usePrivate">True to use Private key for decryption. False to use Public key.</param>
		/// <param name="fOAEP">True to use OAEP.</param>
		/// <returns>Encrypted byte array.</returns>
		public byte[] Decrypt(byte[] message, bool usePrivate, bool fOAEP)
		{
			return Decrypt(message, new byte[0], usePrivate, fOAEP);
		}

		/// <summary>
		/// Decrypts the given RSA encrypted message.
		/// </summary>
		/// <param name="message">The encrypted message.</param>
		/// <param name="oaepParams">Parameters to the OAEP algorithm (Must match the parameter while Encryption).</param>
		/// <param name="usePrivate">True to use Private key for decryption. False to use Public key.</param>
		/// <returns>Encrypted byte array.</returns>
		public byte[] Decrypt(byte[] message, byte[] oaepParams, bool usePrivate)
		{
			return Decrypt(message, oaepParams, usePrivate, true);
		}

		/// <summary>
		/// Decrypts the given RSA encrypted message using Private key.
		/// </summary>
		/// <param name="message">The encrypted message.</param>
		/// <param name="fOAEP">True to use OAEP.</param>
		/// <returns>Encrypted byte array.</returns>
		public byte[] Decrypt(byte[] message,  bool fOAEP)
		{
			return Decrypt(message, new byte[0], true, fOAEP);
		}

		#endregion

	}
}

