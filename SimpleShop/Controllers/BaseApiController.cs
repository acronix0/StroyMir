using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;

namespace SimpleShop.WebApi.Controllers
{
    
    public class BaseApiController : ControllerBase
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly ILoggerManager _logger;
        protected readonly IMapper _mapper;

        public BaseApiController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = repository;
            _logger = logger;
            _mapper = mapper;
        }
    }
}
