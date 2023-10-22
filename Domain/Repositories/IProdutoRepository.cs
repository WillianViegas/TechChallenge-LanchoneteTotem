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
    public interface IProdutoRepository
    {
        public Task<IList<Produto>> GetAllProdutos();
        public Task<Produto> GetProdutoById(string id);
        public Task<Produto> GetProdutoByNome(string nome);
        public  Task<Produto> CreateProduto(Produto produto);
        public  Task UpdateProduto(string id, Produto produto);
        public Task DeleteProduto(string id);
        public Task<IList<Produto>> GetAllProdutosPorCategoria(string id);
    }
}
