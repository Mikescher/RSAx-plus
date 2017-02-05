//**********************************************************************************
//
//OpenSSLKey
// .NET 2.0  OpenSSL Public & Private Key Parser
//
/*
Copyright (c) 2000  JavaScience Consulting,  Michel Gallant

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
//***********************************************************************************
//
//  opensslkey.cs
//
//  Reads and parses:
//    (1) OpenSSL PEM or DER public keys
//    (2) OpenSSL PEM or DER traditional SSLeay private keys (encrypted and unencrypted)
//    (3) PKCS #8 PEM or DER encoded private keys (encrypted and unencrypted)
//  Keys in PEM format must have headers/footers .
//  Encrypted Private Key in SSLEay format not supported in DER
//  Removes header/footer lines.
//  For traditional SSLEAY PEM private keys, checks for encrypted format and
//  uses PBE to extract 3DES key.
//  For SSLEAY format, only supports encryption format: DES-EDE3-CBC
//  For PKCS #8, only supports PKCS#5 v2.0  3des.
//  Parses private and public key components and returns .NET RSA object.
//  Creates dummy unsigned certificate linked to private keypair and
//  optionally exports to pkcs #12
//
// See also: 
//  http://www.openssl.org/docs/crypto/pem.html#PEM_ENCRYPTION_FORMAT 
//**************************************************************************************
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ArpanTECH.OpenSSLKey
{
	public static class OpenSSLKey
	{
		private const string PEMPrivHeader = "-----BEGIN RSA PRIVATE KEY-----";
		private const string PEMPrivFooter = "-----END RSA PRIVATE KEY-----";

		private const string PEMPubHeader = "-----BEGIN PUBLIC KEY-----";
		private const string PEMPubFooter = "-----END PUBLIC KEY-----";

		private const string PEMP8Header = "-----BEGIN PRIVATE KEY-----";
		private const string PEMP8Footer = "-----END PRIVATE KEY-----";

		private const string PEMP8EncHeader = "-----BEGIN ENCRYPTED PRIVATE KEY-----";
		private const string PEMP8EncFooter = "-----END ENCRYPTED PRIVATE KEY-----";

		/// <summary>
		/// Decode PEM pubic, private or pkcs8 key
		/// </summary>
		public static RsaXmlKey PEMKeyToXMLKey(string pemstr, string pw)
		{
			if (pemstr.StartsWith(PEMPubHeader) && pemstr.EndsWith(PEMPubFooter))
			{
				var pempublickey = DecodeOpenSSLPublicKey(pemstr);
				if (pempublickey == null) throw new Exception("DecodeOpenSSLPublicKey failed");

				RSACryptoServiceProvider rsa = DecodeX509PublicKey(pempublickey);
				if (rsa == null) throw new Exception("DecodeX509PublicKey failed");

				var xmlpublickey = rsa.ToXmlString(false);
				return new RsaXmlKey(xmlpublickey, rsa.KeySize);
			}
			
			if (pemstr.StartsWith(PEMPrivHeader) && pemstr.EndsWith(PEMPrivFooter))
			{
				var pemprivatekey = DecodeOpenSSLPrivateKey(pemstr, pw);
				if (pemprivatekey == null) throw new Exception("DecodeOpenSSLPrivateKey failed");

				RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(pemprivatekey);
				if (rsa == null) throw new Exception("DecodeRSAPrivateKey failed");

				var xmlprivatekey = rsa.ToXmlString(true);
				var xmlpublickey = rsa.ToXmlString(false);

				return new RsaXmlKey(xmlpublickey, xmlprivatekey, rsa.KeySize);
			}
			
			if (pemstr.StartsWith(PEMP8Header) && pemstr.EndsWith(PEMP8Footer))
			{
				var pkcs8Privatekey = DecodePkcs8PrivateKey(pemstr);
				if (pkcs8Privatekey == null) throw new Exception("DecodePkcs8PrivateKey failed");

				RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8Privatekey);
				if (rsa == null) throw new Exception("DecodePrivateKeyInfo failed");

				var xmlprivatekey = rsa.ToXmlString(true);
				var xmlpublickey = rsa.ToXmlString(false);
				return new RsaXmlKey(xmlpublickey, xmlprivatekey, rsa.KeySize);
			}
			
			if (pemstr.StartsWith(PEMP8EncHeader) && pemstr.EndsWith(PEMP8EncFooter))
			{
				var pkcs8Encprivatekey = DecodePkcs8EncPrivateKey(pemstr);
				if (pkcs8Encprivatekey == null) throw new Exception("DecodePkcs8EncPrivateKey failed");

				RSACryptoServiceProvider rsa = DecodeEncryptedPrivateKeyInfo(pkcs8Encprivatekey, pw);
				if (rsa == null) throw new Exception("DecodeEncryptedPrivateKeyInfo failed");

				var xmlprivatekey = rsa.ToXmlString(true);
				var xmlpublickey = rsa.ToXmlString(false);
				return new RsaXmlKey(xmlpublickey, xmlprivatekey, rsa.KeySize);
			}

			throw new Exception("Not a PEM public, private key or a PKCS #8");
		}

		/// <summary>
		/// Get the binary PKCS #8 PRIVATE key
		/// </summary>
		private static byte[] DecodePkcs8PrivateKey(string instr)
		{
			string pemstr = instr.Trim();
			if (!pemstr.StartsWith(PEMP8Header) || !pemstr.EndsWith(PEMP8Footer)) throw new Exception("pemp8 has invalid format");
			StringBuilder sb = new StringBuilder(pemstr);
			sb.Replace(PEMP8Header, "");  //remove headers/footers, if present
			sb.Replace(PEMP8Footer, "");

			string pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
			
			return Convert.FromBase64String(pubstr);
		}

		/// <summary>
		/// Parses binary asn.1 PKCS #8 PrivateKeyInfo; returns RSACryptoServiceProvider
		/// </summary>
		/// <param name="pkcs8"></param>
		/// <returns></returns>
		private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
		{
			// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
			// this byte[] includes the sequence byte and terminal encoded null 
			byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
			// ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
			MemoryStream mem = new MemoryStream(pkcs8);
			int lenstream = (int)mem.Length;
			BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading

			try
			{
				var twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("twobytes has invalid value");

				var bt = binr.ReadByte();
				if (bt != 0x02) throw new Exception("bt != 0x02");

				twobytes = binr.ReadUInt16();

				if (twobytes != 0x0001) throw new Exception("twobytes != 0x0001");

				var seq = binr.ReadBytes(15);
				if (!CompareBytearrays(seq, seqOID)) //make sure Sequence for OID is correct
					throw new Exception("seq != seqOID");

				bt = binr.ReadByte();
				if (bt != 0x04) //expect an Octet string 
					throw new Exception("bt != 0x04");

				bt = binr.ReadByte(); //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
				if (bt == 0x81)
					binr.ReadByte();
				else if (bt == 0x82)
					binr.ReadUInt16();
				//------ at this stage, the remaining sequence should be the RSA private key

				byte[] rsaprivkey = binr.ReadBytes((int) (lenstream - mem.Position));
				RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
				return rsacsp;
			}
			finally
			{
				binr.Close();
			}
		}

		/// <summary>
		/// Get the binary PKCS #8 Encrypted PRIVATE key
		/// </summary>
		private static byte[] DecodePkcs8EncPrivateKey(string instr)
		{
			string pemstr = instr.Trim();
			if (!pemstr.StartsWith(PEMP8EncHeader) || !pemstr.EndsWith(PEMP8EncFooter)) throw new Exception("PEMP8Enc has invalid format");
			StringBuilder sb = new StringBuilder(pemstr);
			sb.Replace(PEMP8EncHeader, "");  //remove headers/footers, if present
			sb.Replace(PEMP8EncFooter, "");

			string pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
			
			return Convert.FromBase64String(pubstr);
		}

		/// <summary>
		/// Parses binary asn.1 EncryptedPrivateKeyInfo; returns RSACryptoServiceProvider
		/// </summary>
		private static RSACryptoServiceProvider DecodeEncryptedPrivateKeyInfo(byte[] encpkcs8, string password)
		{
			// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
			// this byte[] includes the sequence byte and terminal encoded null 
			byte[] oidpkcs5PBES2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0D };
			byte[] oidpkcs5PBKDF2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0C };
			byte[] oiddesEDE3CBC = { 0x06, 0x08, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x03, 0x07 };

			// ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
			MemoryStream mem = new MemoryStream(encpkcs8);
			BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading

			try
			{
				var twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("encpkcs8 has invalid format");

				twobytes = binr.ReadUInt16(); //inner sequence
				if (twobytes == 0x8130)
					binr.ReadByte();
				else if (twobytes == 0x8230)
					binr.ReadInt16();


				var seq = binr.ReadBytes(11);
				if (!CompareBytearrays(seq, oidpkcs5PBES2)) //is it a OIDpkcs5PBES2 ?
					throw new Exception("encpkcs8 has invalid format");

				twobytes = binr.ReadUInt16(); //inner sequence for pswd salt
				if (twobytes == 0x8130)
					binr.ReadByte();
				else if (twobytes == 0x8230)
					binr.ReadInt16();

				twobytes = binr.ReadUInt16(); //inner sequence for pswd salt
				if (twobytes == 0x8130)
					binr.ReadByte();
				else if (twobytes == 0x8230)
					binr.ReadInt16();

				seq = binr.ReadBytes(11); //read the Sequence OID
				if (!CompareBytearrays(seq, oidpkcs5PBKDF2)) //is it a OIDpkcs5PBKDF2 ?
					throw new Exception("encpkcs8 has invalid format");

				twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130)
					binr.ReadByte();
				else if (twobytes == 0x8230)
					binr.ReadInt16();

				var bt = binr.ReadByte();
				if (bt != 0x04) //expect octet string for salt
					throw new Exception("encpkcs8 has invalid format");

				int saltsize = binr.ReadByte();
				var salt = binr.ReadBytes(saltsize);

				bt = binr.ReadByte();
				if (bt != 0x02) //expect an integer for PBKF2 interation count
					throw new Exception("encpkcs8 has invalid format");

				int itbytes = binr.ReadByte(); //PBKD2 iterations should fit in 2 bytes.
				int iterations;
				if (itbytes == 1)
					iterations = binr.ReadByte();
				else if (itbytes == 2)
					iterations = 256 * binr.ReadByte() + binr.ReadByte();
				else
					throw new Exception("encpkcs8 has invalid format");

				twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130)
					binr.ReadByte();
				else if (twobytes == 0x8230)
					binr.ReadInt16();


				var seqdes = binr.ReadBytes(10);
				if (!CompareBytearrays(seqdes, oiddesEDE3CBC)) //is it a OIDdes-EDE3-CBC ?
					throw new Exception("encpkcs8 has invalid format");

				bt = binr.ReadByte();
				if (bt != 0x04) //expect octet string for IV
					throw new Exception("encpkcs8 has invalid format");

				int ivsize = binr.ReadByte();
				var iv = binr.ReadBytes(ivsize);

				bt = binr.ReadByte();
				if (bt != 0x04) // expect octet string for encrypted PKCS8 data
					throw new Exception("encpkcs8 has invalid format");


				bt = binr.ReadByte();

				int encblobsize;
				if (bt == 0x81)
					encblobsize = binr.ReadByte(); // data size in next byte
				else if (bt == 0x82)
					encblobsize = 256 * binr.ReadByte() + binr.ReadByte();
				else
					encblobsize = bt; // we already have the data size

				var encryptedpkcs8 = binr.ReadBytes(encblobsize);

				var pkcs8 = DecryptPBDK2(encryptedpkcs8, salt, iv, MakeStringSecure(password), iterations);

				//----- With a decrypted pkcs #8 PrivateKeyInfo blob, decode it to an RSA ---
				RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8);
				return rsa;
			}
			finally
			{
				binr.Close();
			}
		}

		/// <summary>
		/// Uses PBKD2 to derive a 3DES key and decrypts data
		/// </summary>
		private static byte[] DecryptPBDK2(byte[] edata, byte[] salt, byte[] iv, SecureString secpswd, int iterations)
		{
			byte[] psbytes = new byte[secpswd.Length];
			var unmanagedPswd = Marshal.SecureStringToGlobalAllocAnsi(secpswd);
			Marshal.Copy(unmanagedPswd, psbytes, 0, psbytes.Length);
			Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPswd);

			Rfc2898DeriveBytes kd = new Rfc2898DeriveBytes(psbytes, salt, iterations);
			TripleDES decAlg = TripleDES.Create();
			decAlg.Key = kd.GetBytes(24);
			decAlg.IV = iv;
			MemoryStream memstr = new MemoryStream();
			var decrypt = new CryptoStream(memstr, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
			decrypt.Write(edata, 0, edata.Length);
			decrypt.Flush();
			decrypt.Close();    // this is REQUIRED.
			byte[] cleartext = memstr.ToArray();
			return cleartext;
		}

		/// <summary>
		/// Get the binary RSA PUBLIC key
		/// </summary>
		private static byte[] DecodeOpenSSLPublicKey(string instr)
		{
			string pemstr = instr.Trim();
			if (!pemstr.StartsWith(PEMPubHeader) || !pemstr.EndsWith(PEMPubFooter)) throw new Exception("PEM-public-key has invalid format");
			StringBuilder sb = new StringBuilder(pemstr);
			sb.Replace(PEMPubHeader, "");  //remove headers/footers, if present
			sb.Replace(PEMPubFooter, "");

			string pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
			
			return Convert.FromBase64String(pubstr);
		}

		/// <summary>
		/// Parses binary asn.1 X509 SubjectPublicKeyInfo; returns RSACryptoServiceProvider
		/// </summary>
		private static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509Key)
		{
			// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
			byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
			// ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
			MemoryStream mem = new MemoryStream(x509Key);
			BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading

			try
			{
				var twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("x509Key has invalid format");

				var seq = binr.ReadBytes(15);
				if (!CompareBytearrays(seq, seqOID)) //make sure Sequence for OID is correct
					throw new Exception("x509Key has invalid format");

				twobytes = binr.ReadUInt16();
				if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8203)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("x509Key has invalid format");

				var bt = binr.ReadByte();
				if (bt != 0x00) //expect null byte next
					throw new Exception("x509Key has invalid format");

				twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("x509Key has invalid format");

				twobytes = binr.ReadUInt16();
				byte lowbyte;
				byte highbyte;

				if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
				{
					highbyte = 0x00;
					lowbyte = binr.ReadByte(); // read next bytes which is bytes in modulus
				}
				else if (twobytes == 0x8202)
				{
					highbyte = binr.ReadByte(); //advance 2 bytes
					lowbyte = binr.ReadByte();
				}
				else
				{
					throw new Exception("x509Key has invalid format");
				}

				byte[] modint = {lowbyte, highbyte, 0x00, 0x00}; //reverse byte order since asn.1 key uses big endian order
				int modsize = BitConverter.ToInt32(modint, 0);

				byte firstbyte = binr.ReadByte();
				binr.BaseStream.Seek(-1, SeekOrigin.Current);

				if (firstbyte == 0x00)
				{
					//if first byte (highest order) of modulus is zero, don't include it
					binr.ReadByte(); //skip this null byte
					modsize -= 1; //reduce modulus buffer size by 1
				}

				byte[] modulus = binr.ReadBytes(modsize); //read the modulus bytes

				if (binr.ReadByte() != 0x02) //expect an Integer for the exponent data
					throw new Exception("x509Key has invalid format");

				int expbytes = binr.ReadByte(); // should only need one byte for actual exponent data (for all useful values)
				byte[] exponent = binr.ReadBytes(expbytes);

				// ------- create RSACryptoServiceProvider instance and initialize with public key -----
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
				RSAParameters rsaKeyInfo = new RSAParameters
				{
					Modulus = modulus,
					Exponent = exponent
				};
				rsa.ImportParameters(rsaKeyInfo);
				return rsa;
			}
			finally
			{
				binr.Close();
			}

		}

		/// <summary>
		/// Parses binary ans.1 RSA private key; returns RSACryptoServiceProvider
		/// </summary>
		private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
		{
			// ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
			MemoryStream mem = new MemoryStream(privkey);
			BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
			try
			{
				var twobytes = binr.ReadUInt16();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte(); //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16(); //advance 2 bytes
				else
					throw new Exception("rsa-private-key has invalid format");

				twobytes = binr.ReadUInt16();
				if (twobytes != 0x0102) //version number
					throw new Exception("rsa-private-key has invalid version");

				var bt = binr.ReadByte();
				if (bt != 0x00)
					throw new Exception("rsa-private-key has invalid format");


				//------  all private key components are Integer sequences ----
				var elems = GetIntegerSize(binr);
				var modulus = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var e = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var d = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var p = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var q = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var dp = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var dq = binr.ReadBytes(elems);

				elems = GetIntegerSize(binr);
				var iq = binr.ReadBytes(elems);

				// ------- create RSACryptoServiceProvider instance and initialize with public key -----
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
				RSAParameters rsAparams = new RSAParameters
				{
					Modulus = modulus,
					Exponent = e,
					D = d,
					P = p,
					Q = q,
					DP = dp,
					DQ = dq,
					InverseQ = iq
				};
				rsa.ImportParameters(rsAparams);
				return rsa;
			}
			finally
			{
				binr.Close();
			}
		}
		
		private static int GetIntegerSize(BinaryReader binr)
		{
			int count;
			var bt = binr.ReadByte();
			if (bt != 0x02)     //expect integer
				return 0;
			bt = binr.ReadByte();

			if (bt == 0x81)
			{
				count = binr.ReadByte();    // data size in next byte
			}
			else if (bt == 0x82)
			{
				var highbyte = binr.ReadByte();
				var lowbyte = binr.ReadByte();
				byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
				count = BitConverter.ToInt32(modint, 0);
			}
			else
			{
				count = bt;     // we already have the data size
			}
			
			while (binr.ReadByte() == 0x00)
			{   //remove high order zeros in data
				count -= 1;
			}
			binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
			return count;
		}

		//-----  Get the binary RSA PRIVATE key, decrypting if necessary ----
		private static byte[] DecodeOpenSSLPrivateKey(string instr, string password)
		{
			string pemstr = instr.Trim();
			byte[] binkey;
			if (!pemstr.StartsWith(PEMPrivHeader) || !pemstr.EndsWith(PEMPrivFooter)) throw new Exception("PEM-private-key has invalid format");

			StringBuilder sb = new StringBuilder(pemstr);
			sb.Replace(PEMPrivHeader, "");  //remove headers/footers, if present
			sb.Replace(PEMPrivFooter, "");

			string pvkstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace

			try
			{
				// if there are no PEM encryption info lines, this is an UNencrypted PEM private key
				binkey = Convert.FromBase64String(pvkstr);
				return binkey;
			}
			catch (FormatException)
			{
				//if can't b64 decode, it must be an encrypted private key
				//WriteToOutput("Not an unencrypted OpenSSL PEM private key");  
			}

			StringReader str = new StringReader(pvkstr);

			//-------- read PEM encryption info. lines and extract salt -----
			var procLine = str.ReadLine();
			if (procLine == null)
				throw new Exception("ssl-private-key has invalid format");
			if (!procLine.StartsWith("Proc-Type: 4,ENCRYPTED"))
				throw new Exception("ssl-private-key has invalid format");

			string saltline = str.ReadLine();
			if (saltline == null)
				throw new Exception("ssl-private-key has invalid format");
			if (!saltline.StartsWith("DEK-Info: DES-EDE3-CBC,"))
				throw new Exception("ssl-private-key has invalid format");

			string saltstr = saltline.Substring(saltline.IndexOf(",", StringComparison.Ordinal) + 1).Trim();
			byte[] salt = new byte[saltstr.Length / 2];
			for (int i = 0; i < salt.Length; i++)
				salt[i] = Convert.ToByte(saltstr.Substring(i * 2, 2), 16);
			if (str.ReadLine() != "")
				throw new Exception("ssl-private-key has invalid format");

			//------ remaining b64 data is encrypted RSA key ----
			string encryptedstr = str.ReadToEnd();

			binkey = Convert.FromBase64String(encryptedstr);

			//------ Get the 3DES 24 byte key using PDK used by OpenSSL ----

			//Console.Write("\nEnter password to derive 3DES key: ");
			//String pswd = Console.ReadLine();
			byte[] deskey = GetOpenSSL3DESKey(salt, MakeStringSecure(password), 1, 2);    // count=1 (for OpenSSL implementation); 2 iterations to get at least 24 bytes
			//showBytes("3DES key", deskey) ;

			//------ Decrypt the encrypted 3des-encrypted RSA private key ------
			byte[] rsakey = DecryptKey(binkey, deskey, salt);   //OpenSSL uses salt value in PEM header also as 3DES IV
			
			return rsakey;  //we have a decrypted RSA private key
		}

		private static SecureString MakeStringSecure(string s)
		{
			var securePassword = new SecureString();

			foreach (char c in s)
				securePassword.AppendChar(c);

			securePassword.MakeReadOnly();
			return securePassword;
		}

		// ----- Decrypt the 3DES encrypted RSA private key ----------

		private static byte[] DecryptKey(byte[] cipherData, byte[] desKey, byte[] iv)
		{
			MemoryStream memst = new MemoryStream();
			TripleDES alg = TripleDES.Create();
			alg.Key = desKey;
			alg.IV = iv;

			CryptoStream cs = new CryptoStream(memst, alg.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(cipherData, 0, cipherData.Length);
			cs.Close();

			byte[] decryptedData = memst.ToArray();
			return decryptedData;
		}
		
		//-----   OpenSSL PBKD uses only one hash cycle (count); miter is number of iterations required to build sufficient bytes ---
		private static byte[] GetOpenSSL3DESKey(byte[] salt, SecureString secpswd, int count, int miter)
		{
			int HASHLENGTH = 16;    //MD5 bytes
			byte[] keymaterial = new byte[HASHLENGTH * miter];     //to store contatenated Mi hashed results


			byte[] psbytes = new byte[secpswd.Length];
			var unmanagedPswd = Marshal.SecureStringToGlobalAllocAnsi(secpswd);
			Marshal.Copy(unmanagedPswd, psbytes, 0, psbytes.Length);
			Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPswd);

			//UTF8Encoding utf8 = new UTF8Encoding();
			//byte[] psbytes = utf8.GetBytes(pswd);

			// --- contatenate salt and pswd bytes into fixed data array ---
			byte[] data00 = new byte[psbytes.Length + salt.Length];
			Array.Copy(psbytes, data00, psbytes.Length);        //copy the pswd bytes
			Array.Copy(salt, 0, data00, psbytes.Length, salt.Length);   //concatenate the salt bytes

			// ---- do multi-hashing and contatenate results  D1, D2 ...  into keymaterial bytes ----
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = data00;
			byte[] hashtarget = new byte[HASHLENGTH + data00.Length];   //fixed length initial hashtarget

			for (int j = 0; j < miter; j++)
			{
				// ----  Now hash consecutively for count times ------
				if (j == 0)
				{
					result = data00;    //initialize 
				}
				else
				{
					Array.Copy(result, hashtarget, result.Length);
					Array.Copy(data00, 0, hashtarget, result.Length, data00.Length);
					result = hashtarget;
					//WriteToOutput("Updated new initial hash target:") ;
					//showBytes(result) ;
				}

				for (int i = 0; i < count; i++)
					result = md5.ComputeHash(result);
				Array.Copy(result, 0, keymaterial, j * HASHLENGTH, result.Length);  //contatenate to keymaterial
			}
			//showBytes("Final key material", keymaterial);
			byte[] deskey = new byte[24];
			Array.Copy(keymaterial, deskey, deskey.Length);

			Array.Clear(psbytes, 0, psbytes.Length);
			Array.Clear(data00, 0, data00.Length);
			Array.Clear(result, 0, result.Length);
			Array.Clear(hashtarget, 0, hashtarget.Length);
			Array.Clear(keymaterial, 0, keymaterial.Length);

			return deskey;
		}
		
		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				return false;
			int i = 0;
			foreach (byte c in a)
			{
				if (c != b[i])
					return false;
				i++;
			}
			return true;
		}
	}
}
