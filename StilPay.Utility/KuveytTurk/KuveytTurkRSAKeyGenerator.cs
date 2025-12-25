using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace StilPay.Utility.KuveytTurk
{
    public class KuveytTurkRSAKeyGenerator
    {
        public static string RSAKeyGenerator(string privateKey , string accessToken, string httpMethod, string postRequestBody, string queryString)
        {
            try
            {
                var privateKeyService = GetClientPrivateKeyFromRest(privateKey);
                var key = privateKeyService.ExportParameters(true);
                string bodyData = httpMethod == "GET" ? string.Concat("", queryString.Split('?').Length <= 1 ? string.Empty : "?" + queryString.Split('?')[1]) : postRequestBody;
                var signedData = SignData(accessToken + bodyData, key);
                return signedData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string SignData(string data, RSAParameters key)
        {
            // Create a UnicodeEncoder to convert between byte array and string.
            var byteConverter = new ASCIIEncoding();
            var originalData = byteConverter.GetBytes(data);

            try
            {
                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.  
                var rsaProvider = new RSACryptoServiceProvider();

                rsaProvider.ImportParameters(key);

                // Hash and sign the data. Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                var signedData = rsaProvider.SignData(originalData, "SHA256");
                return Convert.ToBase64String(signedData);

            }
            catch (CryptographicException e)
            {
                return e.Message;
            }
        }
        private static RSACryptoServiceProvider GetClientPrivateKeyFromRest(string privateKey)
        {
            using (TextReader privateKeyTextReader = new StringReader(privateKey))
            {
                var readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

                var privateKeyParams = ((RsaPrivateCrtKeyParameters)readKeyPair.Private);
                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var parameters = new RSAParameters
                {

                    Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                    P = privateKeyParams.P.ToByteArrayUnsigned(),
                    Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                    DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                    DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                    InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned(),
                    D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                    Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),

                };

                cryptoServiceProvider.ImportParameters(parameters);
                return cryptoServiceProvider;
            }
        }
    }
}
