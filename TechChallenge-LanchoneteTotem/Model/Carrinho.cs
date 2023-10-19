using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TechChallenge_LanchoneteTotem.Model
{
    public class Carrinho
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<Produto> Produtos { get; set; }
        public decimal Total { get; set; }
        public bool Ativo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
