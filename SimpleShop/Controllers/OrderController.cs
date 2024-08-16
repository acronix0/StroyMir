using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : BaseApiController
    {
        UserManager<ApplicationUser> _userManager;
        public OrderController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager) : base(repository, logger, mapper)
        {
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            var orders = await _repositoryManager.OrderRepository.GetUserOrders(user.Id);
            var x = _mapper.Map<List<OrderDto>>(orders);
            return Ok(x);
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
            }
            await _repositoryManager.OrderRepository.AddOrder(order);
            await _repositoryManager.SaveAsync();
            return Ok();
        }
    }
}
