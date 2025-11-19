using System.Text;   
using ISLE.Interfaces;
using System.Data;
using Microsoft.Extensions.Configuration; 
using System.Net.Mail;
using System.Threading.Tasks;

namespace ISLE.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendMailAsync(string to, string subject, string body)
        {
            try
            {
                var from = _config["Smtp:From"];
                var smtpHost = _config["Smtp:Host"];
                var smtpPort = int.Parse(_config["Smtp:Port"]);
                var smtpUser = _config["Smtp:User"];
                var smtpPass = _config["Smtp:Pass"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;

                    var mail = new MailMessage(from, to, subject, body);
                    mail.IsBodyHtml = true;
                    await client.SendMailAsync(mail);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}