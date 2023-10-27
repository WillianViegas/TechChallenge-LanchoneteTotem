using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Usuario() { }

        public Usuario(NomeVO nome, CpfVO cpf, EmailVO email, string tipo, string senha)
        {
            Nome = nome;
            CPF = cpf;
            Email = email;
            Tipo = tipo;
            Senha = senha;
        }

        public NomeVO Nome { get; set; }
        public EmailVO Email { get; set; }
        public CpfVO CPF { get; set; } 
        public string? Tipo { get; set; } = null;
        public string? Senha { get; set; } = null;
    }
}
