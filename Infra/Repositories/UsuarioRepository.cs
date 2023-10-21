using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
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
        public IList<Usuario> GetAllUsuarios()
        {
            return  _collection.Find(_ => true).ToList();
        }

        public async Task<UsuarioDTO> GetUsuarioById(string id)
        {
            var usuario = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
            return new UsuarioDTO(usuario);
        }

        public async Task<UsuarioDTO> GetUsuarioByCPF(string cpf)
        {
            var usuario = await _collection.Find(x => x.CPF == cpf).FirstOrDefaultAsync();
            return new UsuarioDTO(usuario);
        }

        public async Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            var usuario = await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
            return new UsuarioDTO(usuario);
        }

        public async Task<UsuarioDTO> CreateUsuario(Usuario usuario)
        {
            await _collection.InsertOneAsync(usuario);

            var usuarioDTO = new UsuarioDTO(usuario);

            return usuarioDTO;
        }

        public async Task UpdateUsuario(string id, Usuario usuario)
        {
            var usuarioOriginal = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

            usuarioOriginal.Nome = usuario.Nome;
            usuarioOriginal.Email = usuario.Email;
            usuarioOriginal.CPF = usuario.CPF;

            await _collection.ReplaceOneAsync(x => x.Id.ToString() == id, usuarioOriginal);
        }

        public async Task DeleteUsuario(string id)
        {
            if (await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Usuario usuario)
            {
                await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
            }
        }
    }
}
