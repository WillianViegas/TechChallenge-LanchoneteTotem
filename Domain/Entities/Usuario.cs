using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Nome { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? CPF { get; set; } = null;
        public string? Tipo { get; set; } = null;
        public string? Senha { get; set; } = null;
    }
}
