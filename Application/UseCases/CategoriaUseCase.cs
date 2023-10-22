using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using System.Runtime.InteropServices;

namespace Application.UseCases
{
    public class CategoriaUseCase : ICategoriaUseCase
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaUseCase(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            categoria.Ativa = true;
            return await _categoriaRepository.CreateCategoria(categoria);
        }

        public async Task DeleteCategoria(string id)
        {
            await _categoriaRepository.DeleteCategoria(id);
        }

        public async Task<IList<Categoria>> GetAllCategorias()
        {
            return await _categoriaRepository.GetAllCategorias();
        }

        public async Task<CategoriaDTO> GetCategoriaById(string id)
        {
            var categoria = await _categoriaRepository.GetCategoriaById(id);

            if (categoria is null) return new CategoriaDTO();

            return new CategoriaDTO(categoria);
        }

        public async Task<CategoriaDTO> GetCategoriaByNome(string nome)
        {
            var categoria = await _categoriaRepository.GetCategoriaByNome(nome);

            if (categoria is null) return new CategoriaDTO();

            return new CategoriaDTO(categoria);
        }

        public async Task UpdateCategoria(string id, Categoria categoria)
        {
            await _categoriaRepository.UpdateCategoria(id, categoria);
        }
    }
}
