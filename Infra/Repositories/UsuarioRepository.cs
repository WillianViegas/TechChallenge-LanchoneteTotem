using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Domain.ValueObjects;
using Infra.Configurations;
using MongoDB.Driver;
using System;

namespace Infra.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _collection;

        public UsuarioRepository(IDatabaseConfig databaseConfig)
        {
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collection = database.GetCollection<Usuario>("Usuario");
        }

        //ver como lançar um await
        public async Task<IList<Usuario>> GetAllUsuarios()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Usuario> GetUsuarioById(string id)
        {
            return await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetUsuarioByCPF(string cpf)
        {
            return await _collection.Find(x => x.CPF == new CpfVO(cpf)).FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            return await _collection.Find(x => x.Email == new EmailVO(email)).FirstOrDefaultAsync();
        }

        public async Task<Usuario> CreateUsuario(Usuario usuario)
        {
            await _collection.InsertOneAsync(usuario);
            return usuario;
        }

        public async Task UpdateUsuario(string id, Usuario usuario)
        {
            await _collection.ReplaceOneAsync(x => x.Id.ToString() == id, usuario);
        }

        public async Task DeleteUsuario(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
        }
    }
}
