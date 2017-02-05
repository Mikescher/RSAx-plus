using ArpanTECH.OpenSSLKey;
using MSHC.MVVM;
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

		public ICommand XML2PEMCommand => new RelayCommand(ConvertToPEM);
		public ICommand PEM2XMLCommand => new RelayCommand(ConvertToXML);

		private void ConvertToPEM()
		{

		}

		private void ConvertToXML()
		{
			try
			{
				var key = OpenSSLKey.PEMKeyToXMLKey(TextPEM, "");

				if (key.KeyPrivate == null)
				{
					TextXML = XDocument.Parse(key.KeyPublic).ToString();
				}
				else
				{
					TextXML = XDocument.Parse(key.KeyPrivate).ToString();
				}
			}
			catch (Exception e)
			{
				TextXML = e.ToString();
			}
		}
	}
}
