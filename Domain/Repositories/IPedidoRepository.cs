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
    public interface IPedidoRepository
    {
        public Task<IList<Pedido>> GetAllPedidos();
        public Task<Pedido> GetPedidoById(string id);
        public Task<Pedido> CreatePedido(Pedido pedido);
        public void UpdatePedido(string id, Pedido pedidoInput);
        public void DeletePedido(string id);
    }
}
