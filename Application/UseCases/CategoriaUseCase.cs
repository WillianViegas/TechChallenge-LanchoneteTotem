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

        public Task<Categoria> CreateCategoria(Categoria categoria)
        {
            categoria.Ativa = true;
            return _categoriaRepository.CreateCategoria(categoria);
        }

        public void DeleteCategoria(string id)
        {
            _categoriaRepository.DeleteCategoria(id);
        }

        public IList<Categoria> GetAllCategorias()
        {
            return _categoriaRepository.GetAllCategorias();
        }

        public async Task<CategoriaDTO> GetCategoriaById(string id)
        {
            var categoria = await _categoriaRepository.GetCategoriaById(id);

            if (categoria is null) return null;

            return new CategoriaDTO(categoria);
        }

        public void UpdateCategoria(string id, Categoria categoria)
        {
            _categoriaRepository.UpdateCategoria(id, categoria);
        }
    }
}
