using ArpanTECH;
using MSHC.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RSAx_Toolkit
{
	class Page_Crypt_VM : ObservableObject
	{
		private string _key = "";
		public string Key { get { return _key; } set { _key = value; OnPropertyChanged();} }

		private string _textPlain = "";
		public string TextPlain { get { return _textPlain; } set { _textPlain = value; OnPropertyChanged(); } }

		private string _textCrypt = "";
		public string TextCrypt { get { return _textCrypt; } set { _textCrypt = value; OnPropertyChanged(); } }

		private bool _invertPPK = false;
		public bool InvertPPK { get { return _invertPPK; } set { _invertPPK = value; OnPropertyChanged(); } }
		
		public ObservableCollection<int> ModSizeList { get; } = new ObservableCollection<int>(new[] { 512, 1024, 2048, 4096, 8192, 10240, 16384 });

		private int _modSize = 1024;
		public int ModSize { get { return _modSize; } set { _modSize = value; OnPropertyChanged(); UpdateOctets(); } }

		private int _maxOctets = 86;
		public int MaxOctets { get { return _maxOctets; } private set { _maxOctets = value; OnPropertyChanged(); } }
		
		private bool _useOAEP = true;
		public bool UseOAEP { get { return _useOAEP; } set { _useOAEP = value; OnPropertyChanged(); UpdateOctets(); } }

		public ObservableCollection<RSAxParameters.RSAxHashAlgorithm> HashAlgorithmList { get; } = new ObservableCollection<RSAxParameters.RSAxHashAlgorithm>(new[] { RSAxParameters.RSAxHashAlgorithm.SHA1, RSAxParameters.RSAxHashAlgorithm.SHA256, RSAxParameters.RSAxHashAlgorithm.SHA512 });

		private RSAxParameters.RSAxHashAlgorithm _hashAlgorithm = RSAxParameters.RSAxHashAlgorithm.SHA1;
		public RSAxParameters.RSAxHashAlgorithm HashAlgorithm { get { return _hashAlgorithm; } set { _hashAlgorithm = value; OnPropertyChanged(); } }

		public ICommand EncryptCommand => new RelayCommand(Encrypt);
		public ICommand DecryptCommand => new RelayCommand(Decrypt);

		public Page_Crypt_VM()
		{
			HashAlgorithmList.CollectionChanged += (o, e) => UpdateOctets();
		}

		private void Encrypt()
		{
			try
			{
				RSAx rsax = new RSAx(Key, ModSize) { RSAxHashAlgorithm = HashAlgorithm };

				byte[] ct = rsax.Encrypt(Encoding.UTF8.GetBytes(TextPlain), InvertPPK, UseOAEP);
				TextCrypt = string.Join("\r\n", GetChunks(Convert.ToBase64String(ct), 50));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception while Encryption: " + ex.Message);
			}
		}

		private void Decrypt()
		{
			try
			{
				RSAx rsax = new RSAx(Key, ModSize) { RSAxHashAlgorithm = HashAlgorithm };

				byte[] pt = rsax.Decrypt(Convert.FromBase64String(TextCrypt.Replace("\r", "").Replace("\n", "")), InvertPPK, UseOAEP);
				TextPlain = Encoding.UTF8.GetString(pt);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception while Decryption: " + ex.Message);
			}
		}

		private void UpdateOctets()
		{
			int hLen = new[] { 20, 32, 64, 0 }[(int)HashAlgorithm];

			if (UseOAEP)
				MaxOctets = (ModSize / 8) - 2 * hLen - 2;
			else
				MaxOctets = (ModSize / 8) - 11;
		}

		public IEnumerable<string> GetChunks(string sourceString, int chunkLength)
		{
			using (var sr = new StringReader(sourceString))
			{
				var buffer = new char[chunkLength];
				int read;
				while ((read = sr.Read(buffer, 0, chunkLength)) == chunkLength)
				{
					yield return new string(buffer, 0, read);
				}
			}
		}
	}
}
