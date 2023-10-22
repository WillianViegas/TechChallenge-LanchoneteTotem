using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using static Domain.Entities.Pedido;

namespace Application.UseCases
{
    public class PedidoUseCase : IPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;

        public PedidoUseCase(IPedidoRepository pedidoRepository, ICarrinhoRepository carrinhoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
        }

        public async Task<Pedido> ConfirmarPedido(string id)
        {
            var pedido = await GetPedidoById(id);

            if (pedido is null) throw new Exception("Pedido não existe");

            //fazer solicitação do QRCode para pagamento(antes ou durante essa chamada)
            //passando status pra pago por enquanto (ver como funciona na api do mercado pago)
            pedido.Status = PedidoStatus.Pago;
            pedido.Pagamento = new Pagamento()
            {
                Tipo = Pagamento.TipoPagamento.QRCode,
                QRCodeUrl = "www.usdfhosdfsdhfosdfhsdofhdsfds.com.br",
            };

            await _pedidoRepository.UpdatePedido(id, pedido);


            //desativa o carrinho (pensar se futuramente n é melhor excluir)
            var carrinho = await _carrinhoRepository.GetCarrinhoById(id);
            if (carrinho != null)
            {
                carrinho.Ativo = false;
                await _carrinhoRepository.UpdateCarrinho(carrinho.Id, carrinho);
            }

            return pedido;
        }

        public async Task<Pedido> CreatePedidoFromCarrinho(Carrinho carrinho)
        {
            var numeroPedido = await _pedidoRepository.GetAllPedidos();

            var pedido = new Pedido
            {
                Produtos = carrinho.Produtos,
                Total = carrinho.Total,
                Status = 0,
                DataCriacao = DateTime.Now,
                Numero = numeroPedido.Count + 1,
                Usuario = carrinho.Usuario,
                IdCarrinho = carrinho.Id
            };

            await _pedidoRepository.CreatePedido(pedido);

            return pedido;
        }

        public async Task<Pedido> CreatePedido(Pedido pedido)
        {
            var numeroPedido = await _pedidoRepository.GetAllPedidos();

            var novoPedido = new Pedido
            {
                Produtos = pedido.Produtos,
                Total = pedido.Produtos.Sum(x => x.Preco),
                Status = 0,
                DataCriacao = DateTime.Now,
                Numero = numeroPedido.Count + 1,
                Usuario = pedido.Usuario,
                IdCarrinho = pedido.IdCarrinho
            };

            await _pedidoRepository.CreatePedido(novoPedido);

            return novoPedido;
        }

        public async Task DeletePedido(string id)
        {
            await _pedidoRepository.DeletePedido(id);
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            return await _pedidoRepository.GetAllPedidos();
        }

        public async Task<IList<Pedido>> GetAllPedidosAtivos()
        {
            var listaPedidosAtivos = await _pedidoRepository.GetAllPedidos();
            return listaPedidosAtivos.Where(x => x.Status != Pedido.PedidoStatus.Finalizado).ToList();
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            return await _pedidoRepository.GetPedidoById(id);
        }

        public async Task UpdatePedido(string id, Pedido pedidoInput)
        {
            var pedido = await _pedidoRepository.GetPedidoById(id);
            pedido.Produtos = pedidoInput.Produtos;
            pedido.Total = pedido.Produtos.Sum(x => x.Preco);
            pedido.Usuario = pedidoInput.Usuario;

            await _pedidoRepository.UpdatePedido(id, pedido);
        }

        public async Task UpdateStatusPedido(string id, int status)
        {
            var pedido = await _pedidoRepository.GetPedidoById(id);
            pedido.Status = (Pedido.PedidoStatus)status;
            await _pedidoRepository.UpdatePedido(id, pedido);
        }
    }
}
