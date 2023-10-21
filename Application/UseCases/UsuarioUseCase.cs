using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;

namespace Application.UseCases
{
    public class UsuarioUseCase : IUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Task<UsuarioDTO> CreateUsuario(UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                CPF = usuarioDTO.CPF,
                Email = usuarioDTO.Email
            };

            return _usuarioRepository.CreateUsuario(usuario);
        }

        public void DeleteUsuario(string id)
        {
            _usuarioRepository.DeleteUsuario(id);
        }

        public IList<Usuario> GetAllUsuarios()
        {
            return _usuarioRepository.GetAllUsuarios();
        }

        public Task<UsuarioDTO> GetUsuarioByCPF(string cpf)
        {
            return _usuarioRepository.GetUsuarioByCPF(cpf);
        }

        public Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            return _usuarioRepository.GetUsuarioByEmail(email);
        }

        public Task<UsuarioDTO> GetUsuarioById(string id)
        {
            return _usuarioRepository.GetUsuarioById(id);
        }

        public void UpdateUsuario(string id, UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                CPF = usuarioDTO.CPF,
                Email = usuarioDTO.Email
            };

            _usuarioRepository.UpdateUsuario(id, usuario);
        }
    }
}
