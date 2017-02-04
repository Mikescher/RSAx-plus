using MSHC.MVVM;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows.Input;
using System.Xml.Linq;

namespace RSAx_Toolkit
{
	class Page_Keygen_VM : ObservableObject
	{
		public ObservableCollection<int> ModSizeList { get; } = new ObservableCollection<int>(new[] { 512, 1024, 2048, 4096, 8192, 10240, 16384 });

		private int _modSize = 1024;
		public int ModSize { get { return _modSize; } set { _modSize = value; OnPropertyChanged(); } }

		private string _keyPublic = "";
		public string KeyPublic { get { return _keyPublic; } set { _keyPublic = value; OnPropertyChanged(); } }
		
		private string _keyPrivate = "";
		public string KeyPrivate { get { return _keyPrivate; } set { _keyPrivate = value; OnPropertyChanged(); } }

		public ICommand GenerateCommand => new RelayCommand(Generate);

		private void Generate()
		{
			RSACryptoServiceProvider csp = new RSACryptoServiceProvider(ModSize);
			KeyPrivate = XDocument.Parse(csp.ToXmlString(true)).ToString();
			KeyPublic = XDocument.Parse(csp.ToXmlString(false)).ToString();

		}
	}
}
