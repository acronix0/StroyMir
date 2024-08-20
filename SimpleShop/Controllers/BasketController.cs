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
        public BasketController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, BasketManager basketManager) : base(repository, logger, mapper)
        {
            _userManager = userManager;
            _basketManager = basketManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            var basket = await _basketManager.GetBasket(user);
            if (basket.BasketProducts.Count == 0)
            {
                return Ok(new { Message = "Basket empty" });
            }
            var result = new List<BasketDto>();
            foreach (var item in basket.BasketProducts)
            {
                result.Add(new BasketDto
                {
                    Count = item.Count,
                    Image = item.Product.Image,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    ProductArticle = item.Product.Article
                });
            }
            return Ok(result);
        }
        [HttpPost("add-product")]
        [Authorize]
        public async Task<IActionResult> AddProductToBasket(string productArticle, int count)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            await _basketManager.AddProductToBasket(user, productArticle, count);
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
        [HttpPost("change-product-count")]
        [Authorize]
        public async Task<IActionResult> ChangeProductCount([FromBody] string productArticle,  int count)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized();
            await _basketManager.ChangeProductCount(user, productArticle, count);
            return Ok();
        }
    }
}
