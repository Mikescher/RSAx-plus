using MSHC.MVVM;
using RSAxPlus.OpenSSLKey;
using RSAxPlus.Utils;
using System;
using System.Windows.Input;
using System.Xml.Linq;

namespace RSAx_Toolkit
{
	class Page_Convert_VM : ObservableObject
	{
		private string _textPEM = "";
		public string TextPEM { get { return _textPEM; } set { _textPEM = value; OnPropertyChanged(); } }
		
		private string _textXML = "";
		public string TextXML { get { return _textXML; } set { _textXML = value; OnPropertyChanged(); } }

		public ICommand XML2PEMPublicCommand => new RelayCommand(ConvertPublicToPEM);
		public ICommand XML2PEMPrivateCommand => new RelayCommand(ConvertPrivateToPEM);
		public ICommand PEM2XMLPublicCommand => new RelayCommand(ConvertPublicToXML);
		public ICommand PEM2XMLPrivateCommand => new RelayCommand(ConvertPrivateToXML);

		private void ConvertPublicToPEM()
		{
			try
			{
				TextPEM = PEMUtils.PublicXKMSKeyToPEM(TextXML);
			}
			catch (Exception e)
			{
				TextPEM = e.ToString();
			}
		}

		private void ConvertPrivateToPEM()
		{
			try
			{
				TextPEM = PEMUtils.PrivateXKMSKeyToPEM(TextXML);
			}
			catch (Exception e)
			{
				TextPEM = e.ToString();
			}
		}

		private void ConvertPublicToXML()
		{
			try
			{
				TextXML = XDocument.Parse(OpenSSLKey.PEMKeyToXKMSKey(TextPEM, "").KeyPublic).ToString();
			}
			catch (Exception e)
			{
				TextXML = e.ToString();
			}
		}

		private void ConvertPrivateToXML()
		{
			try
			{
				TextXML = XDocument.Parse(OpenSSLKey.PEMKeyToXKMSKey(TextPEM, "").KeyPrivate).ToString();
			}
			catch (Exception e)
			{
				TextXML = e.ToString();
			}
		}
	}
}
