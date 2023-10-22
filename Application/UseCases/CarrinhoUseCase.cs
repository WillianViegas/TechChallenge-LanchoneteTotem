using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;

namespace Application.UseCases
{
    public class CarrinhoUseCase : ICarrinhoUseCase
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;

        public CarrinhoUseCase(ICarrinhoRepository carrinhoRepository, IProdutoRepository produtoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task<Carrinho> AddProdutoCarrinho(string idProduto, string idCarrinho, int quantidade = 1)
        {
            try
            {
                var produto = await _produtoRepository.GetProdutoById(idProduto);
                if (produto is null) throw new Exception("Produto não existe");

                var carrinho = new Carrinho();

                if (string.IsNullOrEmpty(idCarrinho))
                {
                    var produtoLista = new List<Produto>();

                    for (int i = 0; i < quantidade; i++)
                    {
                        produtoLista.Add(produto);
                    }

                    carrinho = new Carrinho()
                    {
                        Produtos = produtoLista,
                        Total = produtoLista.Sum(x => x.Preco),
                        Ativo = true
                    };

                    await _carrinhoRepository.CreateCarrinho(carrinho);
                    return carrinho;
                }

                carrinho = await GetCarrinhoById(idCarrinho);

                for (int i = 0; i < quantidade; i++)
                {
                    carrinho.Produtos.Add(produto);
                }

                carrinho.Total = carrinho.Produtos.Sum(x => x.Preco);
                await UpdateCarrinho(idCarrinho, carrinho);

                return carrinho;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Carrinho> CreateCarrinho(Carrinho carrinho)
        {
            try
            {
                foreach (var p in carrinho.Produtos)
                {
                    var produto = await _produtoRepository.GetProdutoById(p.Id);
                    if (produto is null) throw new Exception("Produto não existe");
                }

                carrinho.Ativo = true;

                return await _carrinhoRepository.CreateCarrinho(carrinho);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteCarrinho(string id)
        {
            try
            {
                await _carrinhoRepository.DeleteCarrinho(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Carrinho> GetCarrinhoById(string id)
        {
            try
            {
                return await _carrinhoRepository.GetCarrinhoById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Carrinho> RemoveProdutoCarrinho(string idProduto, string idCarrinho, int quantidade = 1)
        {
            try
            {
                var produto = await _produtoRepository.GetProdutoById(idProduto);
                if (produto is null) throw new Exception("Produto não existe");

                var carrinho = await GetCarrinhoById(idCarrinho);
                if (carrinho is null) throw new Exception("Carrinho não existe");


                if (!carrinho.Produtos.Any(x => x.Id.ToString() == idProduto)) throw new Exception("O produto não existe no carrinho");


                for (int i = 0; i < quantidade; i++)
                {
                    var produtoCarrinho = carrinho.Produtos.FirstOrDefault(x => x.Id.ToString() == idProduto);

                    if (produtoCarrinho is null) break;

                    carrinho.Produtos.Remove(produtoCarrinho);
                }

                carrinho.Total = carrinho.Produtos.Sum(x => x.Preco);
                await UpdateCarrinho(idCarrinho, carrinho);

                return carrinho;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCarrinho(string id, Carrinho carrinho)
        {
            try
            {
                var carrinhoOriginal = await _carrinhoRepository.GetCarrinhoById(id);
                if (carrinhoOriginal is null) throw new Exception("Carrinho não encontrado");

                carrinhoOriginal.Produtos = carrinho.Produtos;
                carrinhoOriginal.Total = carrinho.Produtos.Sum(x => x.Preco);

                await _carrinhoRepository.UpdateCarrinho(id, carrinhoOriginal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
