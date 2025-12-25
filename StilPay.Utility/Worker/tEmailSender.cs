using System.Net;
using System.Net.Mail;

namespace StilPay.Utility.Worker
{
    public static class tEmailSender
    {
        public static bool SendEmail(string SmtpServer, int SmtpPortNr, bool EnableSSL, string FromEmail, string FromPassword, string DisplayName, string ToEmail, string Subject, string Content, params Attachment[] Files)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPortNr);
                smtp.EnableSsl = EnableSSL;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, FromPassword);

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(FromEmail, DisplayName);
                    mailMessage.Priority = MailPriority.High;
                    mailMessage.To.Add(ToEmail);
                    mailMessage.Subject = Subject;
                    mailMessage.Body = Content;
                    mailMessage.IsBodyHtml = true;
                    if (Files != null)
                        foreach (var file in Files)
                        {
                            mailMessage.Attachments.Add(file);
                        };

                    smtp.Send(mailMessage);
                }

                return true;
            }
            catch { }

            return false;
        }
    }
}
