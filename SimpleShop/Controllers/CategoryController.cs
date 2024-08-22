using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using System.Collections;
using System.Net.Mail;
using System.Net;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseApiController
    {
        public CategoryController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : base(repository, logger, mapper)
        {
        }
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _repositoryManager.CategoryRepository.GetCategories();
            return Ok( _mapper.Map<List<CategoryDto>>(categories.ToList()));
        }
        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            // Настройки для отправки почты
            string smtpServer = "mail.hosting.reg.ru";
            int smtpPort = 587; // Порт для SSL
            string smtpUser = "support@dm-trade.pro"; // Укажите ваш почтовый адрес
            string smtpPass = "UJ9-EF8-9VP-ae6"; // Укажите пароль почтового ящика

            string fromEmail = smtpUser; // Email отправителя
            string toEmail = "ggghfhhv@yandex.ru"; // Email получателя

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "Тестовое письмо";
            message.Body = "Это тестовое письмо, отправленное с использованием SSL/TLS через SMTP-сервер.";

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                //client.EnableSsl = true; // Включаем SSL
                client.UseDefaultCredentials = false;
                try
                {
                    await client.SendMailAsync(message);
                    return Ok("Письмо успешно отправлено.");
                }
                catch (SmtpException ex)
                {
                    return StatusCode(500, $"Ошибка при отправке письма: {ex.Message}");
                }
            }
        }
    }
    
}
