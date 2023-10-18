using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TechChallenge_LanchoneteTotem.Model
{
    public class Categoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
    }
}
