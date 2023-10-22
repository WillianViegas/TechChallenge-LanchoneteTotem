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
    public interface IPedidoUseCase
    {
        public Task<IList<Pedido>> GetAllPedidosAtivos();
        public Task<IList<Pedido>> GetAllPedidos();
        public Task<Pedido> GetPedidoById(string id);
        public Task<Pedido> CreatePedido(string carrinhoId);
        public Task<Pedido> ConfirmarPedido(string id);
        public Task UpdatePedido(string id, Pedido pedidoInput);
        public Task UpdateStatusPedido(string id, int status);
        public Task DeletePedido(string id);
    }
}
