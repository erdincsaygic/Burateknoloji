using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace StilPay.Utility.Worker
{
    public static class MailSender
    {
        public static string SendEmail(string recipient, string subject, string body)
        {
            var sc = new SmtpClient();
            sc.Port = 587;
            sc.Host = "smtp.yandex.com.tr";
            sc.EnableSsl = true;

            sc.Credentials = new NetworkCredential("noreply@stilpay.com", "15976226Aa__??");

            var mailA = new MailMessage();
            mailA.From = new MailAddress("noreply@stilpay.com", "STİLPAY");

            mailA.To.Add(recipient);

            mailA.Subject = subject;
            mailA.IsBodyHtml = true;
            mailA.Body = body;

            try
            {
                sc.Send(mailA);
                return ("OK");

            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

        }
    }
}
