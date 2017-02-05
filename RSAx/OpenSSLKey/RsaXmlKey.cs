namespace RSAxPlus.OpenSSLKey
{
	public sealed class RsaXmlKey
	{
		public readonly string KeyPublic;
		public readonly string KeyPrivate;
		public readonly int KeySize;

		public RsaXmlKey(string pub, string priv, int size)
		{
			KeyPublic = pub;
			KeyPrivate = priv;
			KeySize = size;
		}

		public RsaXmlKey(string pub, int size)
		{
			KeyPublic = pub;
			KeyPrivate = null;
			KeySize = size;
		}
	}
}
