using System;
using System.Runtime.InteropServices;

namespace RSAxPlus.OpenSSLKey
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CERT_NAME_BLOB
	{
		public int cbData;
		public IntPtr pbData;
	}
}