using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;

namespace SimpleShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BasketController : BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private BasketManager _basketManager;
        private IConfiguration _configuration;
        private TelegramBotManager _telegramBotManager;
        public BasketController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, BasketManager basketManager, TelegramBotManager telegramBotManager, IConfiguration configuration) : base(repository, logger, mapper)
        {
            _userManager = userManager;
            _basketManager = basketManager;
            _telegramBotManager = telegramBotManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            try
            {
                Basket? basket = null;
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                    return Unauthorized();
                basket = await _basketManager.GetBasket(user);


                var result = new CheckoutDto()
                {
                    BasketProducts = new List<BasketProductDto>()
                };
                if (basket == null)
                {
                    return Ok(result);
                }
                result.OrderInfo = new OrderInfo
                {
                    RecipientName = basket.RecipientName,
                    Comment = basket.Comment,
                    Address = basket.Address,
                    RecipientEmail = basket.RecipientEmail,
                    RecipientPhone = basket.RecipientPhone,
                    DeliveryType = basket.DeliveryType,
                };
                if (basket.BasketProducts == null)
                {
                    basket.BasketProducts = new List<BasketProduct>();
                }
                foreach (var item in basket.BasketProducts)
                {
                    var product = await _repositoryManager.ProductRepository.GetProduct(item.ProductId);
                    int inStock = 0;
                    if (product != null)
                    {
                        inStock = product.Count;
                    }
                    result.BasketProducts.Add(new BasketProductDto
                    {
                        Count = item.Count,
                        Image = item.Product.Image,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        ProductArticle = item.Product.Article,
                        InStock = inStock,
                    });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("Ошибка получения корзины");
            }
        }

        [HttpPost("add-product")]
        [Authorize]
        public async Task<IActionResult> AddProductToBasket([FromBody] AddProductDto request)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var err = await _basketManager.AddProductToBasket(user, request.ProductArticle, request.Count);
            if (err != "")
            {
                return BadRequest(err);
            }
            return Ok();
        }

        [HttpPost("remove-product")]
        [Authorize]
        public async Task<IActionResult> RemoveProductFromBasket([FromBody] string productArticle)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            await _basketManager.RemoveProductFromBasket(user, productArticle);
            return Ok();
        }

        [HttpGet("clear-basket")]
        [Authorize]
        public async Task<IActionResult> ClearBasket()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            await _basketManager.ClearBasket(user);
            return Ok();
        }
        public class ChangeProductCountRequest
        {
            public string ProductArticle { get; set; }
            public int Count { get; set; }
        }

        [HttpPost("change-product-count")]
        [Authorize]
        public async Task<IActionResult> ChangeProductCount([FromBody] ChangeProductCountRequest request)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var err = await _basketManager.ChangeProductCount(user, request.ProductArticle, request.Count);
            if (!string.IsNullOrEmpty(err))
                return BadRequest(err);

            return Ok();
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrderFromBasket([FromBody] OrderInfo orderInfo)
        {
            if (orderInfo == null)
                return BadRequest("Некорректные входные данные.");

            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                    return Unauthorized();

                var basket = await _basketManager.GetBasket(user);
                if (basket == null || !basket.BasketProducts.Any())
                    return BadRequest("Корзина пуста.");

                var order = new Order
                {
                    Address = orderInfo.Address,
                    Comment = orderInfo.Comment,
                    DeliveryType = orderInfo.DeliveryType,
                    OrderDate = DateTime.Now,
                    OrderProducts = new List<OrderProduct>(),
                    RecipientEmail = orderInfo.RecipientEmail,
                    RecipientName = orderInfo.RecipientName,
                    RecipientPhone = orderInfo.RecipientPhone,
                    TotalPrice = basket.BasketProducts.Sum(p => p.Count * p.Product.Price),
                    User = user
                };

                var insufficientMessages = new List<string>();

                foreach (var item in basket.BasketProducts)
                {
                    var product = await _repositoryManager.ProductRepository.GetProduct(item.ProductId);
                    if (product == null)
                    {
                        return NotFound($"Товар с ID {item.ProductId} не найден.");
                    }

                    if (product.Count < item.Count)
                    {
                        insufficientMessages.Add($"Недостаточно товара (Артикул: {product.Article}). В наличии: {product.Count}, требуется: {item.Count}.");
                        continue;
                    }

                    order.OrderProducts.Add(new OrderProduct
                    {
                        Order = order,
                        ProductId = product.Id,
                        Count = item.Count,
                        TotalPrice = product.Price
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

                // Очистить корзину после заказа
                await _repositoryManager.BasketProductRepository.ClearBasketProducts(basket.Id);
                basket.BasketProducts.Clear();
                basket.RecipientEmail = orderInfo.RecipientEmail;
                basket.Address = orderInfo.Address;
                basket.RecipientPhone = orderInfo.RecipientPhone;
                basket.DeliveryType = orderInfo.DeliveryType;
                basket.RecipientName = orderInfo.RecipientName;
                await _repositoryManager.BasketRepository.UpdateBasket(basket);
                await _repositoryManager.SaveAsync();

                // Уведомление
                try
                {
                    await _telegramBotManager.SendNewOrderAsync(new OrderDto
                    {
                        Id = order.Id,
                        Address = order.Address,
                        Comment = order.Comment,
                        DeliveryType = order.DeliveryType,
                        OrderDate = order.OrderDate,
                        RecipientEmail = order.RecipientEmail,
                        RecipientName = order.RecipientName,
                        RecipientPhone = order.RecipientPhone,
                        TotalPrice = order.TotalPrice,
                        OrderProducts = order.OrderProducts.Select(op => new OrderProductDto
                        {
                            ProductId = op.ProductId,
                            ProductArticle = op.Product.Article,
                            ProductName = op.Product.Name,
                            ProductPrice = op.TotalPrice,
                            Count = op.Count,
                            Image = op.Product.Image
                        }).ToList()
                    }, _configuration.GetValue<string>("BaseUrl"));
                }
                catch
                {
                    try
                    {
                        await _telegramBotManager.SendSimpleOrder(order.Id, _configuration.GetValue<string>("BaseUrl"));
                    }
                    catch { }
                }

                return Ok(new { Message = "Заказ успешно создан из корзины", OrderId = order.Id });
            }
            catch (Exception ex)
            {
                try { await _telegramBotManager.SendSerg(0, "server ex: " + ex.Message); } catch { }
                return StatusCode(500, "Произошла внутренняя ошибка сервера.");
            }
        }

        [HttpPost("set-delivery-type")]
        [Authorize]
        public async Task<IActionResult> SetDeliveryType([FromBody] string deliveryType)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var err = await _basketManager.SetDeliveryType(user, deliveryType);
            if (!string.IsNullOrEmpty(err))
                return BadRequest(err);

            return Ok();
        }

        [HttpPost("set-comment")]
        [Authorize]
        public async Task<IActionResult> SetComment([FromBody] string comment)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var err = await _basketManager.SetComment(user, comment);
            if (!string.IsNullOrEmpty(err))
                return BadRequest(err);

            return Ok();
        }
    }
}
