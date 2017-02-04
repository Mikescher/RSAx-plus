using MSHC.MVVM;
using System.Windows.Input;

namespace RSAx_Toolkit
{
	class Page_Convert_VM : ObservableObject
	{
		private string _textPEM = "";
		public string TextPEM { get { return _textPEM; } set { _textPEM = value; OnPropertyChanged(); } }
		
		private string _textXML = "";
		public string TextXML { get { return _textXML; } set { _textXML = value; OnPropertyChanged(); } }

		public ICommand XML2PEMCommand => new RelayCommand(ConvertToPEM);
		public ICommand PEM2XMLCommand => new RelayCommand(ConvertTOXML);

		private void ConvertToPEM()
		{

		}

		private void ConvertTOXML()
		{

		}
	}
}
