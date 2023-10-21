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
            var produto = await _produtoRepository.GetProdutoById(idProduto);

            if (produto is null) return null;


            var carrinho = new Carrinho();

            if (string.IsNullOrEmpty(idCarrinho))
            {
                var produtoLista = new List<Produto>();
                //revisar essa parte dps

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

            UpdateCarrinho(idCarrinho, carrinho);

            return carrinho;
        }

        public async Task<Carrinho> CreateCarrinho(Carrinho carrinho)
        {
            //verifica se o produto existe
            foreach (var p in carrinho.Produtos)
            {
                var produto = await _produtoRepository.GetProdutoById(p.Id);

                if (produto is null) return null;
            }

            carrinho.Ativo = true;

            return await _carrinhoRepository.CreateCarrinho(carrinho);
        }

        public async Task DeleteCarrinho(string id)
        {
            if (await GetCarrinhoById(id) is Carrinho carrinho)
            {
                await _carrinhoRepository.DeleteCarrinho(id);
            }

            //tratar retorno 
        }

        public Task<Carrinho> GetCarrinhoById(string id)
        {
            return _carrinhoRepository.GetCarrinhoById(id);
        }

        public async Task<Carrinho> RemoveProdutoCarrinho(string idProduto, string idCarrinho, int quantidade = 1)
        {
            var produto = await _produtoRepository.GetProdutoById(idProduto);
            var carrinho = await GetCarrinhoById(idCarrinho);

            if (produto is null) return null;
            if (carrinho is null) return null; //validar dps 

            if (!carrinho.Produtos.Any(x => x.Id.ToString() == idProduto)) return null;


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

        public async Task UpdateCarrinho(string id, Carrinho carrinho)
        {
            var carrinhoOriginal = await _carrinhoRepository.GetCarrinhoById(id);

            //if (carrinhoOriginal is null) return null; //validar isso na useCase dps

            carrinhoOriginal.Produtos = carrinho.Produtos; //validar remoção da quantidade;
            carrinhoOriginal.Total = carrinho.Produtos.Sum(x => x.Preco);

            await _carrinhoRepository.UpdateCarrinho(id, carrinhoOriginal);
        }
    }
}
