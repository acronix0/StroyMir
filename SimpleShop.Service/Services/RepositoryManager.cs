using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private ICategoryRepository _CategoryRepository;
        private IProductRepository _ProductRepository;
        private IBasketRepository _BasketRepository;
        private IUserAuthenticationRepository _userAuthenticationRepository;
        private IBasketProductRepository _BasketProductRepository;

        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;
        private IConfiguration _configuration;
        
        public RepositoryManager(RepositoryContext repositoryContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration)
        {

            _repositoryContext = repositoryContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IBasketProductRepository BasketProductRepository
        {
            get
            {
                if (_BasketProductRepository is null)
                    _BasketProductRepository = new BasketProductRepository(_repositoryContext);
                return _BasketProductRepository;
            }
        } 
        public IBasketRepository BasketRepository
        {
            get
            {
                if (_BasketRepository is null)
                    _BasketRepository = new BasketRepository(_repositoryContext);
                return _BasketRepository;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_CategoryRepository is null)
                    _CategoryRepository = new CategoryRepository(_repositoryContext);
                return _CategoryRepository;
            }
        }
        public IProductRepository ProductRepository
        {
            get
            {
                if (_ProductRepository is null)
                    _ProductRepository = new ProductRepository(_repositoryContext);
                return _ProductRepository;
            }
        }
        public IUserAuthenticationRepository UserAuthentication
        {
            get
            {
                if(_userAuthenticationRepository is null)
                    _userAuthenticationRepository = new UserAuthenticationRepository(_userManager, _roleManager, _configuration, _mapper);
                return _userAuthenticationRepository;
            }
        }

        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
    }
}
