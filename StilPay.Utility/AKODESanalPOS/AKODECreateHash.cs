using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODECreateHash
    {
        public static string CreateHash(string apiPass, string clientId, string apiUser, string rnd, string timeSpan)
        {
            var hashString = apiPass + clientId + apiUser + rnd + timeSpan;
            System.Security.Cryptography.SHA512 sha = new System.Security.Cryptography.SHA512CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(hashString);
            byte[] hashingbytes = sha.ComputeHash(bytes);
            var hash = Convert.ToBase64String(hashingbytes);
            return hash;
        }
    }
}
