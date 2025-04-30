using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
namespace SimpleShop.Service.Services
{
    public class MailManager
    {
        private string _smtpUser;
        private string _smtpPass;
        private string _smtpServer;
        public MailManager(IConfiguration configuration) 
        {
            var configSection = configuration.GetSection("EmailSettings");
            _smtpUser = configSection.GetValue<string>("SmtpUser");
            _smtpPass = configSection.GetValue<string>("SmtpPass");
            _smtpServer = configSection.GetValue<string>("SmtpServer");
        }
        public async Task<string> SendMail(string subject, string body, string toEmail)
        {
            int smtpPort = 587;
            string fromEmail = _smtpUser;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            // Создание тела сообщения с поддержкой HTML
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = "Ваш клиент не поддерживает HTML. Пожалуйста, используйте другой клиент для просмотра содержимого письма."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                   
                    await client.ConnectAsync(_smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUser, _smtpPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return "";
                }
                catch (Exception ex)
                {
                    // Логирование ошибки ex или другие действия
                    return ex.Message;
                }
            }
        }
    }
}
