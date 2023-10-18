﻿namespace TechChallenge_LanchoneteTotem.Model.DTO
{
    public class ProdutoDTO
    {
        public string Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }

        public ProdutoDTO() { }

        public ProdutoDTO(Produto produto) =>
            (Id, Nome, Descricao, Preco) = (produto.Id, produto.Nome, produto.Descricao, produto.Preco);
    }
}
