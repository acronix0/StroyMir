using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using System.Collections.Generic;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        public ProductController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : base(repository, logger, mapper)
        {
        }
        [HttpPost]
        public async Task<IActionResult> GetProduct([FromBody] CatalogFilterDto filter) 
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [HttpPost("search")]
        public async Task<IActionResult> SearchProduct([FromBody] ProductFilterDto filter)
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [HttpPost("search-catalog")]
        public async Task<IActionResult> SearchCatalog([FromBody] CatalogFilterDto filter)
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [Authorize]
        [HttpPost("get-basket")]
        public async Task<IActionResult> GetBasket([FromBody] List<int> ids)
        {
            var products = await _repositoryManager.ProductRepository.GetProductsById(ids, false);
            var map = _mapper.Map<List<ProductDto>>(products);
            return Ok(map);
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] string article)
        {
            var product = await _repositoryManager.ProductRepository.GetProduct(article);
            await _repositoryManager.ProductRepository.DeleteProduct(product);
            await _repositoryManager.SaveAsync();
            return Ok();
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                productDto.Image = productDto.Image.Replace('.','_')+".webp";
                var product = _mapper.Map<Product>(productDto);
                await _repositoryManager.ProductRepository.AddProduct(product);

                await _repositoryManager.SaveAsync();
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
          
        }
    }
}
