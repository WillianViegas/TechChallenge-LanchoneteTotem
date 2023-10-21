using Domain.Entities;
using Domain.Entities.DTO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ICarrinhoRepository
    {
        public Task<Carrinho> GetCarrinhoById(string id);
        public  Task<Carrinho> CreateCarrinho(Carrinho carrinho);
        public  Task UpdateCarrinho(string id, Carrinho carrinho);
        public Task DeleteCarrinho(string id);
    }
}
