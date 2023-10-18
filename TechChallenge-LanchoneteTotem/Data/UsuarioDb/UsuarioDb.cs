using Microsoft.EntityFrameworkCore;
using TechChallenge_LanchoneteTotem.Model.Usuario;

namespace TechChallenge_LanchoneteTotem.Data.UsuarioDb
{
    class UsuarioDb : DbContext
    {
        public UsuarioDb(DbContextOptions<UsuarioDb> options)
        : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
    }
}
