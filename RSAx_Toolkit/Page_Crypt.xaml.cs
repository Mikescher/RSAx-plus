namespace RSAx_Toolkit
{
	/// <summary>
	/// Interaction logic for Page_Crypt.xaml
	/// </summary>
	public partial class Page_Crypt
	{
		public Page_Crypt()
		{
			InitializeComponent();
			DataContext = new Page_Crypt_VM();
		}
	}
}
