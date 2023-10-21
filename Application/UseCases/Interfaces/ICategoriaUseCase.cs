using Domain.Entities;
using Domain.Entities.DTO;

namespace Application.UseCases.Interfaces
{
    public interface ICategoriaUseCase
    {
        public IList<Categoria> GetAllCategorias();
        public Task<CategoriaDTO> GetCategoriaById(string id);
        public Task<Categoria> CreateCategoria(Categoria categoria);
        public void UpdateCategoria(string id, Categoria categoria);
        public void DeleteCategoria(string id);
    }
}
