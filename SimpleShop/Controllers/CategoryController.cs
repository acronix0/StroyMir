using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;

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
            var catigories = await _repositoryManager.CategoryRepository.GetCategories();
            return Ok( catigories);
           
        }
    }
}
