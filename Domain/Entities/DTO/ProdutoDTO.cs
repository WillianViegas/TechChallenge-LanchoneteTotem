namespace Domain.Entities.DTO
{
    public class ProdutoDTO
    {
        public string Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public string? CategoriaId { get; set; }

        public ProdutoDTO() { }

        public ProdutoDTO(Produto produto) =>
            (Id, Nome, Descricao, Preco, CategoriaId) = (produto.Id.ToString(), produto.Nome, produto.Descricao, produto.Preco, produto.CategoriaId);
    }
}
