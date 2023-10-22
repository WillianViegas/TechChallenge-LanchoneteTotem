using Application.UseCases.Interfaces;
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

        public async Task<UsuarioDTO> CreateUsuario(UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                CPF = usuarioDTO.CPF,
                Email = usuarioDTO.Email
            };

            return await _usuarioRepository.CreateUsuario(usuario);
        }

        public async Task DeleteUsuario(string id)
        {
            await _usuarioRepository.DeleteUsuario(id);
        }

        public async Task<IList<Usuario>> GetAllUsuarios()
        {
            return await _usuarioRepository.GetAllUsuarios();
        }

        public async Task<UsuarioDTO> GetUsuarioByCPF(string cpf)
        {
            return await _usuarioRepository.GetUsuarioByCPF(cpf);
        }

        public async Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            return await _usuarioRepository.GetUsuarioByEmail(email);
        }

        public async Task<UsuarioDTO> GetUsuarioById(string id)
        {
            return await _usuarioRepository.GetUsuarioById(id);
        }

        public async Task UpdateUsuario(string id, UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                CPF = usuarioDTO.CPF,
                Email = usuarioDTO.Email
            };

            await _usuarioRepository.UpdateUsuario(id, usuario);
        }
    }
}
