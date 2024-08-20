using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SharpCompress.Archives.Rar;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace SimpleShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : BaseApiController
    {
        private ImportManager _importService;
        private readonly string _imageDirectory = "/app/import_files";
        public ImportController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, ImportManager importService) : base(repository, logger, mapper)
        {
            _importService = importService;
        }
        [HttpPost]
        public async Task<IActionResult> ImportProductData(IFormFile importXml,  IFormFile offersXml)
        {
            if (importXml == null || offersXml == null)
                return BadRequest("No file uploaded");

            using (var reader = new StreamReader(importXml.OpenReadStream()))
            {
                var importXmlData = await reader.ReadToEndAsync();
                await _importService.ImportXml(importXmlData);
                
            }

            using (var reader = new StreamReader(offersXml.OpenReadStream()))
            {
                var offersXmlData = await reader.ReadToEndAsync();
                await _importService.OffersXml(offersXmlData);
               
            }
            return Ok();

        }

       

    }
}
