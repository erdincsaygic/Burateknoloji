using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StilPay.Utility.Worker
{
    public class tAES3
    {

        public readonly static tAES3 Instance = new tAES3();

        private const string _registryKey = "vg-.2022.-se";

        private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

        private static object _lockConfig = new object();

        public string Decrypt()
        {
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_registryKey, _salt);

                string DecryptText = null;
                byte[] bytes = Convert.FromBase64String(GetEncryptText());
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    RijndaelManaged aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            DecryptText = srDecrypt.ReadToEnd();
                    }


                }

                DecryptText = DecryptText.Replace("vg-.2022.-se", "sp21322-aplsxl_?.");
                DecryptText = DecryptText.Replace("89.252.138.233", "192.168.25.5");
                DecryptText = DecryptText.Replace("DEV", "DEV_TEST2");
                DecryptText = DecryptText.Replace("STILPAY", "MSSQLSERVER2019");

                string[] InfoConfig = DecryptText.Split('\n');
                return "Data Source=" + InfoConfig[0] + "; Initial Catalog=" + InfoConfig[1] + "; User Id=" + InfoConfig[2] + "; Password=" + InfoConfig[3] + "; Min Pool Size=0; Max Pool Size=100; Pooling=true; MultipleActiveResultSets=true;";
            }
            catch (Exception Ex)
            {
            }

            return null;
        }

        private byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new Exception("AES-ReadByteArray");

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException("AES-ReadByteArray");

            return buffer;
        }

        private string GetEncryptText()
        {
            lock (_lockConfig)
            {
                using (FileStream fs = new FileStream("WebApp.config", FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

    }
}