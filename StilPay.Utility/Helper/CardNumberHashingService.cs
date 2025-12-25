using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StilPay.Utility.Helper
{
    public class CardNumberHashingService
    {
        // Sabit bir salt değeri. Bu değeri veritabanından alabilirsiniz veya uygulamanın sabit bir kısmında tutabilirsiniz.
        private static readonly string FixedSalt = tSQLBankManager.GetSystemSettingValues("CardNumberHashSalt")[0].ParamVal;

        // Salt ile birlikte SHA-256 kullanarak hash oluşturma
        public static string Hash(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            string saltedInput = input + FixedSalt;
            byte[] saltedInputBytes = Encoding.UTF8.GetBytes(saltedInput);
            byte[] hashBytes = sha256.ComputeHash(saltedInputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        // Hash'i doğrulamak için fonksiyon
        public static bool VerifyHashWithFixedSalt(string input, string storedHash)
        {
            string computedHash = Hash(input);
            return computedHash == storedHash;
        }
    }
}
