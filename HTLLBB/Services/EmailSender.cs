using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace HTLLBB.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        SmtpConfig _smtpConfig { get; set; }

        public EmailSender(IOptions<SmtpConfig> smtpConfig) 
        {
            _smtpConfig = smtpConfig.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient smtpClient = new SmtpClient(_smtpConfig.Server, _smtpConfig.Port)
            {
                Credentials = new System.Net.NetworkCredential(_smtpConfig.User, _smtpConfig.Pass),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            MailMessage mail = new MailMessage
            {
                //Setting From , To and CC
                From = new MailAddress("HTLLBB@hightechlowlife.info", "HTLLBB"),
                Subject = subject,
                Body = message
            };
            mail.To.Add(new MailAddress(email));
            mail.IsBodyHtml = true;

            smtpClient.Send(mail);

            return Task.CompletedTask;
        }
    }
}
