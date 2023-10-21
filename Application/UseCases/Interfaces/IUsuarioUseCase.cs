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
    public interface IUsuarioUseCase
    {
        public IList<Usuario> GetAllUsuarios();
        public Task<UsuarioDTO> GetUsuarioById(string id);
        public Task<UsuarioDTO> GetUsuarioByCPF(string cpf);
        public Task<UsuarioDTO> GetUsuarioByEmail(string email);
        public Task<UsuarioDTO> CreateUsuario(UsuarioDTO usuarioDTO);
        public void UpdateUsuario(string id, UsuarioDTO usuarioDTO);
        public void DeleteUsuario(string id);
    }
}
