using Amazon.Runtime.Internal.Util;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Domain.ValueObjects;
using Infra.Configurations;
using Infra.Repositories;
using System.ComponentModel.DataAnnotations;

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
            try
            {
                var usuario = new Usuario()
                {
                    Nome = new NomeVO(usuarioDTO.Nome),
                    CPF = new CpfVO(usuarioDTO.CPF),
                    Email = new EmailVO(usuarioDTO.Email),
                    Tipo = "C" //cliente
                };

                var usuarioPorCpf = await GetUsuarioByCPF(usuarioDTO.CPF);

                if (usuarioPorCpf != null && !string.IsNullOrEmpty(usuarioPorCpf.Id))
                    throw new ValidationException("CPF já cadastrado");

                return new UsuarioDTO(await _usuarioRepository.CreateUsuario(usuario));
            }
            catch (ValidationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteUsuario(string id)
        {
            try
            {
                await _usuarioRepository.DeleteUsuario(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Usuario>> GetAllUsuarios()
        {
            try
            {
                return await _usuarioRepository.GetAllUsuarios();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioDTO> GetUsuarioByCPF(string cpf)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByCPF(cpf);
                if (usuario == null) return new UsuarioDTO();

                return new UsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByEmail(email);
                if (usuario == null) return new UsuarioDTO();

                return new UsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioDTO> GetUsuarioById(string id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUsuarioById(id);
                if (usuario == null) return new UsuarioDTO();

                return new UsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateUsuario(string id, UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuarioOriginal = await _usuarioRepository.GetUsuarioById(id);
                if (usuarioOriginal is null) throw new Exception("Usuario não encontrado");

                usuarioOriginal.Nome = new NomeVO(usuarioDTO.Nome);
                usuarioOriginal.Email = new EmailVO(usuarioDTO.Email);
                usuarioOriginal.CPF = new CpfVO(usuarioDTO.CPF);

                await _usuarioRepository.UpdateUsuario(id, usuarioOriginal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
