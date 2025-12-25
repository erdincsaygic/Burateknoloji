using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StilPay.Utility.Worker
{
    public class RandomPasswordGenerator
    {

        public static string GenerateRandomPassword()
        {
            // Karakter setlerini tanımla
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            // Karakter setlerini birleştir
            const string allChars = upperCaseChars + lowerCaseChars + digits;

            Random random = new Random();

            // Şifre uzunluğunu belirle (4 ile 16 arasında)
            int length = random.Next(10, 17); // 17 çünkü üst sınır dahil değil

            // Şifreyi oluştur
            StringBuilder password = new StringBuilder(length);

            // En az bir büyük harf, küçük harf, rakam ve özel karakter ekleyin
            password.Append(upperCaseChars[random.Next(upperCaseChars.Length)]);
            password.Append(lowerCaseChars[random.Next(lowerCaseChars.Length)]);
            password.Append(digits[random.Next(digits.Length)]);

            // Kalan karakterler için rastgele seçim yapın
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Karakterlerin rastgele sıralanmasını sağla
            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }
    }
}
