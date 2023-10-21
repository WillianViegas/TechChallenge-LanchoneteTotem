using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
    public class Pedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int Numero { get; set; }

        public List<Produto> Produtos { get; set; }
        public Usuario Usuario { get; set; }
        public decimal Total { get; set; }
        public PedidoStatus Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public string IdCarrinho { get; set; }
        public Pagamento Pagamento { get; set; }

        public enum PedidoStatus
        {
            Novo = 0,
            Pago = 1,
            Confirmado = 2,
            EmPreparo = 3,
            Pronto = 4,
            Finalizado = 5
        };
    }
}
