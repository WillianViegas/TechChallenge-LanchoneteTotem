using Domain.Entities;
using Domain.Entities.DTO;

namespace Application.UseCases.Interfaces
{
    public interface ICategoriaUseCase
    {
        public Task<IList<Categoria>> GetAllCategorias();
        public Task<CategoriaDTO> GetCategoriaById(string id);
        public Task<CategoriaDTO> GetCategoriaByNome(string id);
        public Task<Categoria> CreateCategoria(Categoria categoria);
        public Task UpdateCategoria(string id, Categoria categoria);
        public Task DeleteCategoria(string id);
    }
}
