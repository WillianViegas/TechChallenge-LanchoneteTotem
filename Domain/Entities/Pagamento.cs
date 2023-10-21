using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
    public class Pagamento
    {
        public TipoPagamento Tipo { get; set; }
        public string QRCodeUrl { get; set; }
        public string Bandeira { get; set; }
        public string OrdemDePagamento { get; set; }


        public enum TipoPagamento
        {
            QRCode = 0,
            CartaoCredito = 1,
            CartaoDebito = 2,
            CartaoRefeicao = 3
        }
    }
}
