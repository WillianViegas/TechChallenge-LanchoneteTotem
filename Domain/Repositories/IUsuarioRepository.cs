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
    public interface IUsuarioRepository
    {
        public Task<IList<Usuario>> GetAllUsuarios();
        public Task<Usuario> GetUsuarioById(string id);
        public  Task<Usuario> GetUsuarioByCPF(string cpf);
        public  Task<Usuario> GetUsuarioByEmail(string email);
        public  Task<Usuario> CreateUsuario(Usuario usuario);
        public  Task UpdateUsuario(string id, Usuario usuario);
        public Task DeleteUsuario(string id);
    }
}
