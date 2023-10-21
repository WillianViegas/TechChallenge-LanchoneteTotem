using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using MongoDB.Driver;
using System;

namespace Infra.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IMongoCollection<Pedido> _collection;

        public PedidoRepository(IDatabaseConfig databaseConfig)
        {
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collection = database.GetCollection<Pedido>("Pedido");
        }

        //ver como lançar um await
        public async Task<IList<Pedido>> GetAllPedidos()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            var pedido = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
            return pedido;
        }

        public async Task<Pedido> CreatePedido(Pedido pedido)
        {
            await _collection.InsertOneAsync(pedido);

            return pedido;
        }

        public void UpdatePedido(string id, Pedido pedidoInput)
        {
            _collection.ReplaceOneAsync(x => x.Id.ToString() == id, pedidoInput);
        }

        public void DeletePedido(string id)
        {
            _collection.DeleteOneAsync(x => x.Id.ToString() == id);
        }
    }
}
