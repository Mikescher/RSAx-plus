using System;
using System.Runtime.InteropServices;

namespace ArpanTECH.OpenSSLKey
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CRYPT_KEY_PROV_INFO
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public String pwszContainerName;
		[MarshalAs(UnmanagedType.LPWStr)]
		public String pwszProvName;
		public uint dwProvType;
		public uint dwFlags;
		public uint cProvParam;
		public IntPtr rgProvParam;
		public uint dwKeySpec;
	}
}