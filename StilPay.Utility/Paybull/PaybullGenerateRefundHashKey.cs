using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StilPay.Utility.Paybull
{
    public class PaybullGenerateRefundHashKey
    {
        public static string GenerateRefundHashKey(string amount, string invoice_id, string merchant_key, string app_secret)
        {
            string data = amount + '|' + invoice_id + '|' + merchant_key;
            Random mt_rand = new Random();
            string iv = Sha1Hash(mt_rand.Next().ToString()).Substring(0, 16);
            string password = Sha1Hash(app_secret);
            string salt = Sha1Hash(mt_rand.Next().ToString()).Substring(0, 4);
            string saltWithPassword = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                saltWithPassword = GetHash(sha256Hash, password + salt);
            }
            string encrypted = Encryptor(data, saltWithPassword.Substring(0, 32), iv);

            string msg_encrypted_bundle = iv + ":" + salt + ":" + encrypted;
            msg_encrypted_bundle = msg_encrypted_bundle.Replace("/", "__");

            return msg_encrypted_bundle;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static string Sha1Hash(string password)
        {
            return string.Join("", SHA1CryptoServiceProvider.Create().ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
        }

        private static string Encryptor(string TextToEncrypt, string strKey, string strIV)
        {
            //Turn the plaintext into a byte array.
            byte[] PlainTextBytes = System.Text.Encoding.UTF8.GetBytes(TextToEncrypt);

            //Setup the AES providor for our purposes.
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.BlockSize = 128;
            //MessageBox.Show(System.Text.Encoding.UTF8.GetBytes(strKey).Length.ToString());
            aesProvider.KeySize = 256;
            //My key and iv that i have used in openssl
            aesProvider.Key = System.Text.Encoding.UTF8.GetBytes(strKey);
            aesProvider.IV = System.Text.Encoding.UTF8.GetBytes(strIV);
            aesProvider.Padding = PaddingMode.PKCS7;
            aesProvider.Mode = CipherMode.CBC;

            ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);
            byte[] EncryptedBytes = cryptoTransform.TransformFinalBlock(PlainTextBytes, 0, PlainTextBytes.Length);
            return Convert.ToBase64String(EncryptedBytes);
        }
        private static string Decryptor(string TextToDecrypt, string strKey, string strIV)
        {
            byte[] EncryptedBytes = Convert.FromBase64String(TextToDecrypt);

            //Setup the AES provider for decrypting.            
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            //aesProvider.Key = System.Text.Encoding.ASCII.GetBytes(strKey);
            //aesProvider.IV = System.Text.Encoding.ASCII.GetBytes(strIV);
            aesProvider.BlockSize = 128;
            aesProvider.KeySize = 256;
            //My key and iv that i have used in openssl
            aesProvider.Key = System.Text.Encoding.ASCII.GetBytes(strKey);
            aesProvider.IV = System.Text.Encoding.ASCII.GetBytes(strIV);
            aesProvider.Padding = PaddingMode.PKCS7;
            aesProvider.Mode = CipherMode.CBC;


            ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            byte[] DecryptedBytes = cryptoTransform.TransformFinalBlock(EncryptedBytes, 0, EncryptedBytes.Length);
            return System.Text.Encoding.ASCII.GetString(DecryptedBytes);
        }
        public static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }
    }
}
