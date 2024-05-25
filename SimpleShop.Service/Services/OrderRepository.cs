using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    public class OrderRepository : BaseEntityRepository<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void foo()
        {
            throw new NotImplementedException();
        }
    }
    class ASd {
        public ASd()
        {
           
        }
    }
}
