using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using System.Collections;

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
            string[] people = { "Tom", "Sam", "Bob" };
            
            IEnumerator peopleEnumerator = people.GetEnumerator(); // получаем IEnumerator
            while (peopleEnumerator.MoveNext())   // пока не будет возвращено false
            {
                string item = (string)peopleEnumerator.Current; // получаем элемент на текущей позиции
                Console.WriteLine(item);
            }
            peopleEnumerator.Reset();



            var catigories = await _repositoryManager.CategoryRepository.GetCategories();
            return Ok( catigories);
           
        }
    }
}
