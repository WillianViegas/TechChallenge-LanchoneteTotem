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
        public Task<UsuarioDTO> GetUsuarioById(string id);
        public  Task<UsuarioDTO> GetUsuarioByCPF(string cpf);
        public  Task<UsuarioDTO> GetUsuarioByEmail(string email);
        public  Task<UsuarioDTO> CreateUsuario(Usuario usuario);
        public  Task UpdateUsuario(string id, Usuario usuario);
        public Task DeleteUsuario(string id);
    }
}
