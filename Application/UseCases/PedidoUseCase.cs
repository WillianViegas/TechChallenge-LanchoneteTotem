using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Enum;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using Microsoft.Extensions.Logging;
using static Domain.Entities.Pedido;

namespace Application.UseCases
{
    public class PedidoUseCase : IPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly ILogger _log;

        public PedidoUseCase(IPedidoRepository pedidoRepository, ICarrinhoRepository carrinhoRepository, ILogger<PedidoUseCase> log)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _log = log;
        }

        public async Task<Pedido> FinalizarPedido(string id)
        {
            try
            {
                var pedido = await GetPedidoById(id);
                if (pedido is null) throw new Exception("Pedido não existe");

                if (pedido.Status != EPedidoStatus.Novo) throw new Exception($"Status do pedido não é válido para confirmação. Status: {pedido.Status}, NumeroPedido: {pedido.Numero}");

                pedido.Status = EPedidoStatus.PendentePagamento;
                pedido.Pagamento = new Pagamento()
                {
                    Tipo = ETipoPagamento.QRCode,
                    QRCodeUrl = "www.usdfhosdfsdhfosdfhsdofhdsfds.com.br",
                    OrdemDePagamento = Guid.NewGuid().ToString()
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> ConfirmarPedido(string id)
        {
            var pedido = new Pedido();
            try
            {
                pedido = await GetPedidoById(id);

                if (pedido is null)
                {
                    _log.LogError("Pedido não encontrado");
                    return pedido;
                }
                if (pedido.Status != EPedidoStatus.PendentePagamento)
                {
                    _log.LogError($"Status do pedido não é válido para confirmação. Status: {pedido.Status}, NumeroPedido: {pedido.Numero}");
                    return pedido;
                }
                
                pedido.Status = EPedidoStatus.Recebido;

                await _pedidoRepository.UpdatePedido(id, pedido);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }

            return pedido;
        }

        public async Task<Pedido> CreatePedidoFromCarrinho(Carrinho carrinho)
        {
            try
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
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> CreatePedido(Pedido pedido)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeletePedido(string id)
        {
            try
            {
                await _pedidoRepository.DeletePedido(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            try
            {
                var listaPedidos = await _pedidoRepository.GetAllPedidos();
                var listaPedidosFiltrados = listaPedidos.Where(x => x.Status == EPedidoStatus.EmPreparo || x.Status == EPedidoStatus.Pronto || x.Status == EPedidoStatus.Recebido)
                    .OrderBy(n => n.Status == EPedidoStatus.Recebido)
                    .ThenBy(n => n.Status == EPedidoStatus.EmPreparo)
                    .ThenBy(n => n.Status == EPedidoStatus.Pronto).ToList();

                return listaPedidosFiltrados;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Pedido>> GetAllPedidosAtivos()
        {
            try
            {
                var listaPedidosAtivos = await _pedidoRepository.GetAllPedidos();
                return listaPedidosAtivos.Where(x => x.Status != EPedidoStatus.Novo && x.Status != EPedidoStatus.Recebido && x.Status != EPedidoStatus.Finalizado).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Pedido>> GetAllPedidosProntosParaRetirada()
        {
            try
            {
                var listaPedidosAtivos = await _pedidoRepository.GetAllPedidos();
                return listaPedidosAtivos.Where(x => x.Status == EPedidoStatus.Pronto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Pedido>> GetAllPedidosFinalizados()
        {
            try
            {
                var listaPedidosAtivos = await _pedidoRepository.GetAllPedidos();
                return listaPedidosAtivos.Where(x => x.Status == EPedidoStatus.Finalizado).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            try
            {
                return await _pedidoRepository.GetPedidoById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdatePedido(string id, Pedido pedidoInput)
        {
            try
            {
                var pedido = await _pedidoRepository.GetPedidoById(id);
                if (pedido is null) throw new Exception("Pedido não encontrado");

                pedido.Produtos = pedidoInput.Produtos;
                pedido.Total = pedido.Produtos.Sum(x => x.Preco);
                pedido.Usuario = pedidoInput.Usuario;

                await _pedidoRepository.UpdatePedido(id, pedido);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateStatusPedido(string id, int status)
        {
            try
            {
                var pedido = await _pedidoRepository.GetPedidoById(id);
                if (pedido is null) throw new Exception("Pedido não encontrado");

                pedido.Status = (EPedidoStatus)status;
                await _pedidoRepository.UpdatePedido(id, pedido);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
