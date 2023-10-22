using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;

namespace Application.UseCases
{
    public class ProdutoUseCase : IProdutoUseCase
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoUseCase(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<ProdutoDTO> CreateProduto(Produto produto)
        {
            return new ProdutoDTO( await _produtoRepository.CreateProduto(produto));
        }

        public async Task DeleteProduto(string id)
        {
            await _produtoRepository.DeleteProduto(id);
        }

        public async Task<IList<Produto>> GetAllProdutos()
        {
            return await _produtoRepository.GetAllProdutos();
        }

        public async Task<ProdutoDTO> GetProdutoById(string id)
        {
            var produto = await _produtoRepository.GetProdutoById(id);

            if (produto == null) return new ProdutoDTO();

            return new ProdutoDTO(await _produtoRepository.GetProdutoById(id));
        }

        public async Task<IList<Produto>> GetAllProdutosPorCategoria(string id)
        {
            return await _produtoRepository.GetAllProdutosPorCategoria(id);
        }

        public async Task<ProdutoDTO> GetProdutoByNome(string nome)
        {
            var produto = await _produtoRepository.GetProdutoByNome(nome);

            if (produto == null) return new ProdutoDTO();

            return new ProdutoDTO(produto);
        }

        public async Task UpdateProduto(string id, Produto produtoInput)
        {
            var produto = await _produtoRepository.GetProdutoById(id);

            if (produto != null)
            {
                produto.Nome = produtoInput.Nome;
                produto.Descricao = produtoInput.Descricao;
                produto.Preco = produtoInput.Preco;
                produto.CategoriaId = produtoInput.CategoriaId is null ? produto.CategoriaId : produtoInput.CategoriaId;

                await _produtoRepository.UpdateProduto(id, produto);
            }
        }
    }
}
