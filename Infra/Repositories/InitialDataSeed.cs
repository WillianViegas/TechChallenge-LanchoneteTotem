using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infra.Configurations.Database;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infra.Repositories
{
    public class InitialDataSeed : IInitialDataSeed
    {
        private readonly IMongoCollection<Usuario> _collectionUsuario;
        private readonly IMongoCollection<Categoria> _collectionCategoria;
        private readonly IMongoCollection<Produto> _collectionProduto;
        private readonly ILogger _log;

        public InitialDataSeed(IDatabaseConfig databaseConfig, ILogger<InitialDataSeed> log)
        {
            _log = log;
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collectionUsuario = database.GetCollection<Usuario>("Usuario");
            _collectionCategoria = database.GetCollection<Categoria>("Categoria");
            _collectionProduto = database.GetCollection<Produto>("Produto");
        }

        public async Task CarregarDadosIniciais()
        {
            var usuarios = await _collectionUsuario.Find(_ => true).ToListAsync();

            if (!usuarios.Any())
            {
                var usuariosList = MockarUsuarios();
                await _collectionUsuario.InsertManyAsync(usuariosList);
            }

            var categorias = await _collectionCategoria.Find(_ => true).ToListAsync();

            if (!categorias.Any())
            {
                var categoriasList = MockarCategorias();
                await _collectionCategoria.InsertManyAsync(categoriasList);
            }


            var produtos = await _collectionProduto.Find(_ => true).ToListAsync();

            if (!produtos.Any())
            {
                var produtosList = await MockarProdutos();
                await _collectionProduto.InsertManyAsync(produtosList);
            }

        }

        public List<Usuario> MockarUsuarios()
        {
            var newUsuarioList = new List<Usuario>();

            var newUsuarioCliente = new Usuario()
            {
                Nome = new NomeVO("Marcos"),
                CPF = new CpfVO("65139370000"),
                Email = new EmailVO("marcao@gmail.com"),
                Tipo = "C"
            };

            var newUsuarioAtendente = new Usuario()
            {
                Nome = new NomeVO("Admin"),
                CPF = new CpfVO("82390019048"),
                Email = new EmailVO("adm@gmail.com"),
                Tipo = "A",
                Senha = "admin!123"
            };

            newUsuarioList.Add(newUsuarioCliente);
            newUsuarioList.Add(newUsuarioAtendente);

            return newUsuarioList;
        }

        public List<Categoria> MockarCategorias()
        {
            var newCategoriaList = new List<Categoria>();

            var newCategoria1 = new Categoria()
            {
                Nome = "Lanche",
                Ativa = true
            };

            var newCategoria2 = new Categoria()
            {
                Nome = "Acompanhamento",
                Ativa = true
            };

            var newCategoria3 = new Categoria()
            {
                Nome = "Bebida",
                Ativa = true
            };

            newCategoriaList.Add(newCategoria1);
            newCategoriaList.Add(newCategoria2);
            newCategoriaList.Add(newCategoria3);

            return newCategoriaList;
        }

        public async Task<List<Produto>> MockarProdutos()
        {
            var newProdutoList = new List<Produto>();

            var categoriaLanche = await _collectionCategoria.Find(x => x.Nome == "Lanche").FirstOrDefaultAsync();
            var newProduto1Lanche = new Produto()
            {
                Nome = "Hamburguer especial da casa",
                Descricao = "Hamburguer artesanal da casa com maionese caseira e molho secreto",
                Preco = 35.99m,
                CategoriaId = categoriaLanche != null ? categoriaLanche.Id : ""
            };

            var newProduto2Lanche = new Produto()
            {
                Nome = "Hamburguer vulcão cheddar",
                Descricao = "Hamburguer no estilo vulcão, acompanhado de um bocado de cheddar",
                Preco = 39.99m,
                CategoriaId = categoriaLanche != null ? categoriaLanche.Id : ""
            };


            var categoriaAcompanhamento = await _collectionCategoria.Find(x => x.Nome == "Acompanhamento").FirstOrDefaultAsync();
            var newProduto1Acomp = new Produto()
            {
                Nome = "Batata frita com cheddar e bacon",
                Descricao = "Porção de batata frita com cheddar e bacon, serve 2 pessoas",
                Preco = 25.99m,
                CategoriaId = categoriaAcompanhamento != null ? categoriaAcompanhamento.Id : ""
            };

            var newProduto2Acomp = new Produto()
            {
                Nome = "Anéis de cebola",
                Descricao = "Anéis de cebola servidos com molho especial, serve 2 pessoas",
                Preco = 25.99m,
                CategoriaId = categoriaAcompanhamento != null ? categoriaAcompanhamento.Id : ""
            };


            var categoriaBebida = await _collectionCategoria.Find(x => x.Nome == "Bebida").FirstOrDefaultAsync();
            var newProduto1Bebida = new Produto()
            {
                Nome = "MilkShake",
                Descricao = "Sabores: nutella, morango, paçoca e baunilha. Tamanho: 500ml",
                Preco = 17.99m,
                CategoriaId = categoriaBebida != null ? categoriaBebida.Id : ""
            };

            var newProduto2Bebida = new Produto()
            {
                Nome = "Suco natural",
                Descricao = "Sabores: laranja, goiaba, manga e limão. Tamanho: 500ml",
                Preco = 17.99m,
                CategoriaId = categoriaBebida != null ? categoriaBebida.Id : ""
            };

            newProdutoList.Add(newProduto1Lanche);
            newProdutoList.Add(newProduto2Lanche);
            newProdutoList.Add(newProduto1Acomp);
            newProdutoList.Add(newProduto2Acomp);
            newProdutoList.Add(newProduto1Bebida);
            newProdutoList.Add(newProduto2Bebida);

            return newProdutoList;
        }
    }
}
