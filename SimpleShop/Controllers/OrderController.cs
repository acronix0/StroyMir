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
                orderDetails.AppendLine($"<p><strong>- Артикль:</strong> {product.ProductArticle}<br>" +
                    $"<strong>Название:</strong> {product.ProductName}<br>" +
                    $"<strong>Количество:</strong> {product.Count}<br>" +
                    $"<strong>Стоимость:</strong> {(product.ProductPrice * product.Count):C}</p>");
            }

            orderDetails.AppendLine($"<p><strong>Общая стоимость:</strong> {order.TotalPrice:C}</p>");
            orderDetails.AppendLine($"<p><strong>Тип доставки:</strong> {order.DeliveryType}</p>");
            orderDetails.AppendLine($"<p><strong>Адрес доставки:</strong> {order.Address}</p>");
            orderDetails.AppendLine($"<p><strong>Дата заказа:</strong> {order.OrderDate}</p>");
            orderDetails.AppendLine($"<p><strong>Комментарий:</strong> {order.Comment}</p>");


            var email = _configuration.GetSection("EmailSettings").GetValue<string>("OrderRecipient");
            await _mailManager.SendMail("Новый заказ", orderDetails.ToString(), email);
            return Ok();
        }
    }
}
