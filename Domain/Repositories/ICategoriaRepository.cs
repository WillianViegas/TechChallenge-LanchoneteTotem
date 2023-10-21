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
    public interface ICategoriaRepository
    {
        public IList<Categoria> GetAllCategorias();
        public Task<Categoria> GetCategoriaById(string id);
        public  Task<Categoria> CreateCategoria(Categoria usuario);
        public  Task UpdateCategoria(string id, Categoria usuario);
        public Task DeleteCategoria(string id);
    }
}
