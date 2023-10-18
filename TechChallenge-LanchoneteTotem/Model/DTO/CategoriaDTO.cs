namespace TechChallenge_LanchoneteTotem.Model.DTO
{
    public class CategoriaDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }


        public CategoriaDTO() { }

        public CategoriaDTO(Categoria categoria) =>
            (Id, Nome) = (categoria.Id, categoria.Nome);
    }
}
