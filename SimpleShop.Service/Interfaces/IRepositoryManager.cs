using SimpleShop.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IRepositoryManager
    {
        Task SaveAsync();
        OrderRepository OrderRepository { get; }
    }
}
