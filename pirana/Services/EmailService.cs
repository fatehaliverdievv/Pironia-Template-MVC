using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using pirana.Abstraction;
using System.Net;
using System.Net.Mail;

namespace pirana.Services
{
    public class EmailService:IEmailService
    {
        IConfiguration _configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string mailto,string subject,string body, bool isbodyhtml =false)
        {
            using SmtpClient smtpClient = new SmtpClient { Host = _configuration["Email:Host"], Port = Convert.ToInt32(_configuration["Email:Port"])};
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_configuration["Email:Login"], _configuration["Email:Password"]);
            MailAddress from = new MailAddress(_configuration["Email:Login"], "Pironia");
            MailAddress to = new MailAddress(mailto); //"tu7l7qsfn@code.edu.az";
            using MailMessage message = new MailMessage();
            message.Subject = subject;  //"Salam dostlar";
            message.Body = body; //"Men yene geldim";
            message.IsBodyHtml = isbodyhtml;
            smtpClient.Send(message);
        }
    }
}
