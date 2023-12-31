﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Domain.Enum;
using Domain.Entities.DTO;

namespace Domain.Entities
{
    public class Pedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int Numero { get; set; }
        public List<Produto> Produtos { get; set; }
        public UsuarioDTO Usuario { get; set; }
        public decimal Total { get; set; }
        public EPedidoStatus Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public string IdCarrinho { get; set; }
        public Pagamento Pagamento { get; set; }
    }
}
