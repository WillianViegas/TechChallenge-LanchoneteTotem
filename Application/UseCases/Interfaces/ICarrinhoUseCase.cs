using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Interfaces
{
    public interface ICarrinhoUseCase
    {
        public Task<Carrinho> GetCarrinhoById(string id);
        public Task<Carrinho> CreateCarrinho(Carrinho carrinho);
        public Task UpdateCarrinho(string id, Carrinho carrinho);
        public Task DeleteCarrinho(string id);
        public Task<Carrinho> AddProdutoCarrinho(string idProduto, string idCarrinho, int quantidade = 1);
        public Task<Carrinho> RemoveProdutoCarrinho(string idProduto, string idCarrinho, int quantidade = 1);
    }
}
