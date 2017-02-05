RSA Library with Private Key Encryption in C#
=============================================

forked from [RSAx](https://www.codeproject.com/articles/421656/rsa-library-with-private-key-encryption-in-csharp)

A small library for RSA public/private key encryption in C# with a focus for compatibility with the corresponding PHP functions.

Supports reading keys in the PEM format


##Example

###C#:

(with this library)

~~~csharp

string pubkey = "-----BEGIN PUBLIC KEY-----" + "\n"+
                "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAOdImQbdvCm2ZHmRGBFm+cvRHqyA8vSA" + "\n"+
                "1EXMEfwHXvPZ4zQwdQT3568IwyvABzuc2v6a5xFSPUCJLayvgoLJPNMCAwEAAQ==" + "\n"+
                "-----END PUBLIC KEY-----";

RSAx rsax = RSAx.CreateFromPEM(key);
byte[] ctx = rsax.Encrypt(Encoding.UTF8.GetBytes(input), false);
return Convert.ToBase64String(ctx);

~~~

###PHP:

(native [openssl](http://php.net/manual/en/book.openssl.php) methods)

~~~php

define("KEY", "-----BEGIN PRIVATE KEY-----
MIIBUwIBADANBgkqhkiG9w0BAQEFAASCAT0wggE5AgEAAkEA50iZBt28KbZkeZEY
EWb5y9EerIDy9IDURcwR/Ade89njNDB1BPfnrwjDK8AHO5za/prnEVI9QIktrK+C
gsk80wIDAQABAkB5z/Ww9RYGTicLFA0+FSNZYrGqH1xWxIeIn1uVhvhOq6IrlioX
UboWdAuuOk4vL7xrHEeH0iqRBP+QxkRD+qF5AiEA+KK6os1wbhnZwC/L7f+y8yQ3
DKLybUwBq0JuWJ/KUtcCIQDuIkwXE6gCObpmHs3sDkAEB6+JBoGsfrCMeypshfoi
ZQIgBrvpkCU+SU0b76+bt1t4jktJzmbPaBRp6yiGcpIJWcsCIGayJcxh7ree+7Lk
n/uoHZVfVyUpyCyCqlK7Hw2ULc49AiATLSXEDVuo6e/KUdDKutQ/geonXSqJ8Q43
rAvX/eL3Xw==
-----END PRIVATE KEY-----")

function decrypt_rsa($input) {
	global $config;

	$pkey = openssl_pkey_get_private(KEY);

	$decrypted = "";
	$encrypted = base64_decode($input);

	if(!openssl_private_decrypt($encrypted, $decrypted, $pkey)) return false;

	return $decrypted;
}

~~~

###Javascript:

(with [jsencrypt](https://github.com/travist/jsencrypt), needs [jsbn and jsbn2](http://www-cs-students.stanford.edu/%7Etjw/jsbn/), even though the author says otherwise...)

~~~javascript

<script type="text/javascript" src="jsbn.js"></script>
<script type="text/javascript" src="jsbn2.js"></script>
<script type="text/javascript" src="rsa.js"></script>

<script type="text/javascript">
    function encrypt(text) {
		let keystr = "-----BEGIN PUBLIC KEY-----" + "\n"+
		             "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAOdImQbdvCm2ZHmRGBFm+cvRHqyA8vSA" + "\n"+
		             "1EXMEfwHXvPZ4zQwdQT3568IwyvABzuc2v6a5xFSPUCJLayvgoLJPNMCAwEAAQ==" + "\n"+
		             "-----END PUBLIC KEY-----";

		let key = RSA.getPublicKey(keystr);

		return RSA.encrypt(text, key);
	}
</script>
~~~