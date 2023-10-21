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
    public interface IProdutoUseCase
    {
        public IList<Produto> GetAllProdutos();
        public Task<ProdutoDTO> GetProdutoById(string id);
        public Task<ProdutoDTO> CreateProduto(Produto produto);
        public void UpdateProduto(string id, Produto produto);
        public void DeleteProduto(string id);
        public Task<IList<Produto>> GetAllProdutosPorCategoria(string id);
    }
}
