﻿using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using MongoDB.Driver;
using System;

namespace Infra.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly IMongoCollection<Produto> _collection;

        public ProdutoRepository(IDatabaseConfig databaseConfig)
        {
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collection = database.GetCollection<Produto>("Produto");
        }

        //ver como lançar um await
        public IList<Produto> GetAllProdutos()
        {
            return  _collection.Find(_ => true).ToList();
        }

        public async Task<Produto> GetProdutoById(string id)
        {
            var produto = await _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

            if (produto == null) return null;

            return produto;
        }

        public async Task<IList<Produto>> GetAllProdutosPorCategoria(string id)
        {
            var produtos = await _collection.Find(x => x.CategoriaId == id).ToListAsync();
            return produtos;
        }

        public async Task<Produto> CreateProduto(Produto produto)
        {
            await _collection.InsertOneAsync(produto);

            return produto;
        }

        public async Task UpdateProduto(string id, Produto produto)
        {
            await _collection.ReplaceOneAsync(x => x.Id.ToString() == id, produto);
        }

        public async Task DeleteProduto(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
        }
    }
}