using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using MongoDB.Driver;
using System;

namespace Infra.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly IMongoCollection<Categoria> _collection;

        public CategoriaRepository(IDatabaseConfig databaseConfig)
        {
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collection = database.GetCollection<Categoria>("Categoria");
        }

        public async Task<IList<Categoria>> GetAllCategorias()
        {
            return  await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Categoria> GetCategoriaById(string id)
        {
            var categoria = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

            return categoria;
        }

        public async Task<Categoria> GetCategoriaByNome(string nome)
        {
            var categoria = await _collection.Find(x => x.Nome.ToUpper() == nome.ToUpper()).FirstOrDefaultAsync();

            return categoria;
        }

        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            await _collection.InsertOneAsync(categoria);

            return categoria;
        }

        public async Task UpdateCategoria(string id, Categoria categoria)
        {
            var categoriaOriginal = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

            categoriaOriginal.Nome = categoria.Nome;
            categoriaOriginal.Ativa = categoria.Ativa;

            await _collection.ReplaceOneAsync(x => x.Id.ToString() == id, categoriaOriginal);
        }

        public async Task DeleteCategoria(string id)
        {
            if (await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Categoria categoria)
            {
                await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
            }
        }
    }
}
