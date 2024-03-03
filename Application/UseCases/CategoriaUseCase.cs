using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Application.UseCases
{
    public class CategoriaUseCase : ICategoriaUseCase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger _log;

        public CategoriaUseCase(ICategoriaRepository categoriaRepository, ILogger<CategoriaUseCase> log)
        {
            _categoriaRepository = categoriaRepository;
            _log = log;
        }

        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            try
            {
                categoria.Ativa = true;
                return await _categoriaRepository.CreateCategoria(categoria);
            }
            catch(Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteCategoria(string id)
        {
            try
            {
                await _categoriaRepository.DeleteCategoria(id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<Categoria>> GetAllCategorias()
        {
            try
            {
                return await _categoriaRepository.GetAllCategorias();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoriaDTO> GetCategoriaById(string id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetCategoriaById(id);
                if (categoria is null) return new CategoriaDTO();

                return new CategoriaDTO(categoria);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoriaDTO> GetCategoriaByNome(string nome)
        {
            try
            {
                var categoria = await _categoriaRepository.GetCategoriaByNome(nome);
                if (categoria is null) return new CategoriaDTO();

                return new CategoriaDTO(categoria);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCategoria(string id, Categoria categoria)
        {
            try
            {
                var categoriaOriginal = await _categoriaRepository.GetCategoriaById(id);
                if (categoriaOriginal is null) throw new Exception("Categoria não encontrada");

                categoriaOriginal.Nome = categoria.Nome;
                categoriaOriginal.Ativa = categoria.Ativa;
                await _categoriaRepository.UpdateCategoria(id, categoriaOriginal);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
