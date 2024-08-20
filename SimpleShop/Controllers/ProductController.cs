using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        public ProductController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : base(repository, logger, mapper)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetProduct([FromQuery] CatalogFilterDto filter) 
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProduct([FromQuery] ProductFilterDto filter)
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [HttpGet("search-catalog")]
        public async Task<IActionResult> SearchCatalog([FromQuery] CatalogFilterDto filter)
        {
            var products = _mapper.Map<List<ProductDto>>(await _repositoryManager.ProductRepository.GetProducts(filter, false));
            return Ok(products);
        }
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteProduct(string article)
        {
            var product = await _repositoryManager.ProductRepository.GetProduct(article);
            await _repositoryManager.ProductRepository.DeleteProduct(product);
            await _repositoryManager.SaveAsync();
            return Ok();
        }
    }
}
