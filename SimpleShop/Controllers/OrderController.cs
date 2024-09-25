using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;
using System.Text;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private MailManager _mailManager;
        private IConfiguration _configuration;
        public OrderController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, MailManager mailManager, IConfiguration configuration) : base(repository, logger, mapper)
        {
            _userManager = userManager;
            _configuration= configuration;
            _mailManager = mailManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            var orders = await _repositoryManager.OrderRepository.GetUserOrders(user.Id);
            return Ok(_mapper.Map<List<OrderDto>>(orders));
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrders([FromBody] OrderDto orderDto)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            var order = new Order()
            {
                Address = orderDto.Address,
                Comment = orderDto.Comment,
                DeliveryType = orderDto.DeliveryType,
                OrderDate = DateTime.Now,
                OrderProducts = new List<OrderProduct>(),
                RecipientEmail = orderDto.RecipientEmail,
                RecipientName = orderDto.RecipientName,
                RecipientPhone = orderDto.RecipientPhone,
                TotalPrice = orderDto.TotalPrice,
                User = user
            };
            foreach (var item in orderDto.OrderProducts)
            {
                order.OrderProducts.Add(new OrderProduct()
                {
                    Order = order,
                    
                    ProductId = item.ProductId,
                    Count = item.Count,
                    TotalPrice = item.ProductPrice * item.Count
                });
                var product = await _repositoryManager.ProductRepository.GetProduct(item.ProductId);
                await _repositoryManager.ProductRepository.UpdateProduct(product);
                product.Count -= item.Count;
            }

            await _repositoryManager.OrderRepository.AddOrder(order);
            
            await _repositoryManager.SaveAsync();


            var orderDetails = new StringBuilder();

            orderDetails.AppendLine($"<p><strong>Имя заказчика:</strong> {order.RecipientName}</p>");
            orderDetails.AppendLine($"<p><strong>Email заказчика:</strong> {order.RecipientEmail}</p>");
            orderDetails.AppendLine($"<p><strong>Телефон заказчика:</strong> {order.RecipientPhone}</p>");
            orderDetails.AppendLine("<p><strong>Товары в заказе:</strong></p>");

            foreach (var product in orderDto.OrderProducts)
            {
                
                decimal amount2 = product.ProductPrice * product.Count;
                // Округляем до двух знаков после запятой
                amount2 = Math.Round(amount2, 2, MidpointRounding.AwayFromZero);
                // Получаем абсолютное значение для корректной обработки отрицательных чисел
                decimal absoluteAmount2 = Math.Abs(amount2);
                // Разделяем целую и дробную части
                int integerPart2 = (int)absoluteAmount2;
                int fractionalPart2 = (int)((absoluteAmount2 - integerPart2) * 100);
                string integerPartWithSpaces2 = AddThousandSeparators(integerPart2);
                string formattedNumber2 = $"{integerPartWithSpaces2},{fractionalPart2:D2}";
                orderDetails.AppendLine($"<p><strong>- Артикль:</strong> {product.ProductArticle}<br>" +
                    $"<strong>Название:</strong> {product.ProductName}<br>" +
                    $"<strong>Количество:</strong> {product.Count}<br>" +
                    $"<strong>Стоимость:</strong> {formattedNumber2} руб.</p>");

            }
            decimal amount = order.TotalPrice;

            // Округляем до двух знаков после запятой
            amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

            // Получаем абсолютное значение для корректной обработки отрицательных чисел
            decimal absoluteAmount = Math.Abs(amount);

            // Разделяем на целую и дробную части
            int integerPart = (int)absoluteAmount;
            int fractionalPart = (int)((absoluteAmount - integerPart) * 100);

            // Форматируем целую часть с разделителями тысяч
            string integerPartWithSpaces = AddThousandSeparators(integerPart);

            // Собираем итоговую строку
            string formattedNumber = $"{integerPartWithSpaces},{fractionalPart:D2}";

            orderDetails.AppendLine($"<p><strong>Общая стоимость:</strong> {formattedNumber} руб.</p>");
            orderDetails.AppendLine($"<p><strong>Тип доставки:</strong> {order.DeliveryType}</p>");
            orderDetails.AppendLine($"<p><strong>Адрес доставки:</strong> {order.Address}</p>");
            orderDetails.AppendLine($"<p><strong>Дата заказа:</strong> {order.OrderDate}</p>");
            orderDetails.AppendLine($"<p><strong>Комментарий:</strong> {order.Comment}</p>");


            var email = _configuration.GetSection("EmailSettings").GetValue<string>("OrderRecipient");
            await _mailManager.SendMail("Новый заказ", orderDetails.ToString(), email);
            return Ok();
        }
        string AddThousandSeparators(int number)
        {
            string numberStr = number.ToString();
            StringBuilder sb = new StringBuilder();

            int counter = 0;
            for (int i = numberStr.Length - 1; i >= 0; i--)
            {
                sb.Insert(0, numberStr[i]);
                counter++;
                if (counter == 3 && i != 0)
                {
                    sb.Insert(0, ' ');
                    counter = 0;
                }
            }
            return sb.ToString();
        }
    }
}
