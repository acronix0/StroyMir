using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;
using System.Text;
using Telegram.Bot.Types;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private MailManager _mailManager;
        private IConfiguration _configuration;
        private TelegramBotManager _telegramBotManager;
        public OrderController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, MailManager mailManager,TelegramBotManager telegramBotManager, IConfiguration configuration) : base(repository, logger, mapper)
        {
            _userManager = userManager;
            _configuration= configuration;
            _mailManager = mailManager;
            _telegramBotManager = telegramBotManager;
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
        [HttpGet("reports")]
        public async Task<IActionResult> GetOrders3days()
        {
            var orders = await _repositoryManager.OrderRepository.GetOrders();
            var data = _mapper.Map<List<OrderDto>>(orders.Where(o => o.OrderDate >= DateTime.Now.AddDays(-1)));
            await _telegramBotManager.SendOrdersReportToTelegramAsync(data);
            return Ok();
        }


        [HttpGet("{orderId}")]
        public async Task<ContentResult> GetOrderPage(int orderId)
        {
            // Получаем данные заказа из репозитория
            var query = await _repositoryManager.OrderRepository.GetOrder(orderId);
            var order = _mapper.Map<OrderDto>(query);

            if (order == null)
            {
                // Если заказ не найден
                return new ContentResult
                {
                    Content = "<h1>Заказ не найден</h1>",
                    ContentType = "text/html",
                    StatusCode = 404
                };
            }

            // Формируем HTML для отображения заказа
            string htmlContent = $@"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Заказ №{order.Id}</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #5f5f5f;
            color: #333;
            font-size: 14px;
        }}
        .container {{
            max-width: 960px;
            margin: auto;
            padding: 20px;
        }}
        .order-details, .order-products {{
            background: #fff;
            color: #333;
            padding: 15px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 20px;
        }}
        .order-products table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        .order-products th, .order-products td {{
            border: 1px solid #ddd;
            padding: 10px;
            text-align: left;
        }}
        .order-products th {{
            background-color: #f28322;
            color: #fff;
            font-size: 14px;
        }}
        .order-products td {{
            background-color: #fff;
            color: #333;
            font-size: 14px;
        }}
        h1 {{
            color: #f28322;
            font-size: 24px;
            text-align: center;
        }}
        h2 {{
            color: #f28322;
            font-size: 18px;
            margin-top: 0;
        }}
        p {{
            margin: 5px 0;
            font-size: 14px;
        }}

        /* Мобильные устройства */
        @media (max-width: 768px) {{
            .container {{
                padding: 10px;
            }}
            h1 {{
                font-size: 20px;
            }}
            h2 {{
                font-size: 16px;
            }}
            .order-products th, .order-products td {{
                font-size: 12px;
                padding: 8px;
            }}
        }}

        /* Очень маленькие устройства */
        @media (max-width: 480px) {{
            .order-products th, .order-products td {{
                font-size: 10px;
                padding: 5px;
            }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Информация о заказе</h1>
        <div class='order-details'>
            <h2>Данные заказчика и доставки</h2>
            <p><strong>Имя получателя:</strong> {order.RecipientName}</p>
            <p><strong>Телефон:</strong> {order.RecipientPhone}</p>
            <p><strong>Электронная почта:</strong> {order.RecipientEmail}</p>
            <p><strong>Адрес доставки:</strong> {order.Address}</p>
            <p><strong>Тип доставки:</strong> {order.DeliveryType}</p>
            <p><strong>Дата заказа:</strong> {order.OrderDate:yyyy-MM-dd}</p>
            <p><strong>Комментарий:</strong> {order.Comment}</p>
            <p><strong>Общая стоимость:</strong> {FormatPriceDecimal(order.TotalPrice)}</p>
        </div>
        
        <div class='order-products'>
            <h2>Состав заказа</h2>
            <table>
                <thead>
                    <tr>
                        <th>Артикул</th>
                        <th>Название</th>
                        <th>Количество</th>
                        <th>Стоимость</th>
                    </tr>
                </thead>
                <tbody>
                    {string.Join("", order.OrderProducts.Select(product => $@"
                        <tr>
                            <td>{product.ProductArticle}</td>
                            <td>{product.ProductName}</td>
                            <td>{product.Count}</td>
                            <td>{FormatPriceDecimal(product.ProductPrice)}</td>
                        </tr>
                    "))}
                </tbody>
            </table>
        </div>
    </div>
</body>
</html>";


            // Возвращаем HTML-контент
            return new ContentResult
            {
                Content = htmlContent,
                ContentType = "text/html",
                StatusCode = 200
            };
        }

        private string FormatPriceDecimal(decimal price)
        {
            price = Math.Round(price, 2, MidpointRounding.AwayFromZero);

            // Форматируем число с использованием инвариантной культуры
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:N2}", price)
                         .Replace(",", " ") // Разделитель тысяч — пробел
                         .Replace(".", ",") + " руб."; // Десятичный разделитель — запятая
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrders([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.OrderProducts == null || !orderDto.OrderProducts.Any())
            {
                return BadRequest("Некорректные входные данные.");
            }

            try
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

                var insufficientMessages = new List<string>();

                foreach (var item in orderDto.OrderProducts)
                {
                    var product = await _repositoryManager.ProductRepository.GetProduct(item.ProductId);
                    if (product == null)
                    {
                        return NotFound($"Товар с Артикулом {item.ProductArticle} не найден.");
                    }

                    if (product.Count < item.Count)
                    {
                        insufficientMessages.Add($"Недостаточное количество товара (Артикул: {item.ProductArticle}). Доступно: {product.Count}, запрошено: {item.Count}.");
                        continue;
                    }

                    order.OrderProducts.Add(new OrderProduct()
                    {
                        Order = order,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        TotalPrice = item.ProductPrice
                    });

                    product.Count -= item.Count;
                    await _repositoryManager.ProductRepository.UpdateProduct(product);
                }

                if (insufficientMessages.Any())
                {
                    return BadRequest(string.Join(" ", insufficientMessages));
                }

                await _repositoryManager.OrderRepository.AddOrder(order);
                await _repositoryManager.SaveAsync();

                orderDto.Id = order.Id;
            }
            catch (DbUpdateException dbEx)
            {
                try { await _telegramBotManager.SendSerg(orderDto.Id, "db ex:  " + dbEx.Message); } catch { }
                return StatusCode(500, "Произошла внутренняя ошибка сервера.");
            }
            catch (Exception ex)
            {
                try { await _telegramBotManager.SendSerg(orderDto.Id, "server ex:  " + ex.Message); } catch { }
                return StatusCode(500, "Произошла внутренняя ошибка сервера.");
            }

            try
            {
                try
                {
                    await _telegramBotManager.SendNewOrderAsync(orderDto, _configuration.GetValue<string>("BaseUrl"));
                }
                catch { }
            }
            catch
            {
                try
                {
                    await _telegramBotManager.SendSimpleOrder(orderDto.Id, _configuration.GetValue<string>("BaseUrl"));
                }
                catch { }
            }

            return Ok(new { Message = "Заказ успешно создан", OrderId = orderDto.Id });
        }
        
        [Authorize]
        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckProductAvailability([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return BadRequest("Список ID товаров пуст или не передан.");
            }

            var result = new Dictionary<int, int>();

            foreach (var id in productIds)
            {
                Product? product = null;
                try
                {
                    product = await _repositoryManager.ProductRepository.GetProduct(id);
                }
                catch 
                {
                }
                if (product != null)
                {
                    result[id] = product.Count;
                }
                else
                {
                    result[id] = -1; // Или можно вообще не добавлять, если товар не найден
                }
            }

            return Ok(result);
        }
    }
}
