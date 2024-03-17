using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class ProdutoUseCase : IProdutoUseCase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ILogger _log;

        public ProdutoUseCase(IProdutoRepository produtoRepository, ILogger<ProdutoUseCase> log)
        {
            _produtoRepository = produtoRepository;
            _log = log;
        }

        public async Task<ProdutoDTO> CreateProduto(Produto produto)
        {
            try
            {
                return new ProdutoDTO(await _produtoRepository.CreateProduto(produto));
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteProduto(string id)
        {
            try
            {
                await _produtoRepository.DeleteProduto(id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Produto>> GetAllProdutos()
        {
            try
            {
                return await _produtoRepository.GetAllProdutos();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProdutoDTO> GetProdutoById(string id)
        {
            try
            {
                var produto = await _produtoRepository.GetProdutoById(id);
                if (produto == null) return new ProdutoDTO();

                return new ProdutoDTO(await _produtoRepository.GetProdutoById(id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Produto>> GetAllProdutosPorCategoria(string id)
        {
            try
            {
                return await _produtoRepository.GetAllProdutosPorCategoria(id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProdutoDTO> GetProdutoByNome(string nome)
        {
            try
            {
                var produto = await _produtoRepository.GetProdutoByNome(nome);
                if (produto == null) return new ProdutoDTO();

                return new ProdutoDTO(produto);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateProduto(string id, Produto produtoInput)
        {
            try
            {
                var produto = await _produtoRepository.GetProdutoById(id);
                if (produto is null) throw new Exception("Produto não encontrado");

                produto.Nome = produtoInput.Nome;
                produto.Descricao = produtoInput.Descricao;
                produto.Preco = produtoInput.Preco;
                produto.CategoriaId = produtoInput.CategoriaId is null ? produto.CategoriaId : produtoInput.CategoriaId;

                await _produtoRepository.UpdateProduto(id, produto);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
