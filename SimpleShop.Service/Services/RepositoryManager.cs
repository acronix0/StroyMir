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
        //private IEntityRepository _baseEntityRepository;
       
        private ICategoryRepository _CategoryRepository;

        //private UserManager<ApplicationUser> _userManager;
        //private RoleManager<IdentityRole> _roleManager;
        //private IMapper _mapper;
        private IConfiguration _configuration;
        
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            
            _repositoryContext = repositoryContext;
           
            //_configuration = configuration;
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

        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
    }
}
