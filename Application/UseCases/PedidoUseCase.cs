﻿using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Enum;
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
            try
            {
                var pedido = await GetPedidoById(id);
                if (pedido is null) throw new Exception("Pedido não existe");

                //fazer solicitação do QRCode para pagamento(antes ou durante essa chamada)
                //passando status pra pago por enquanto (ver como funciona na api do mercado pago)
                pedido.Status = EPedidoStatus.Pago;
                pedido.Pagamento = new Pagamento()
                {
                    Tipo = ETipoPagamento.QRCode,
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                var listaPedidosFiltrados = listaPedidos.Where(x => x.Status == EPedidoStatus.EmPreparo || x.Status == EPedidoStatus.Pronto || x.Status == EPedidoStatus.Confirmado)
                    .OrderBy(n => n.Status == EPedidoStatus.Pronto)
                    .ThenBy(n => n.Status == EPedidoStatus.EmPreparo)
                    .ThenBy(n => n.Status == EPedidoStatus.Confirmado).ToList();

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
                return listaPedidosAtivos.Where(x => x.Status != EPedidoStatus.Novo && x.Status != EPedidoStatus.Pago && x.Status != EPedidoStatus.Finalizado).ToList();
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
