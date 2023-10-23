using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Domain.Enum;

namespace Domain.Entities
{
    public class Pagamento
    {
        public ETipoPagamento Tipo { get; set; }
        public string QRCodeUrl { get; set; }
        public string Bandeira { get; set; }
        public string OrdemDePagamento { get; set; }
    }
}
