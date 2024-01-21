namespace Notifier.Model
{
    public class MessageBody
    {
        public string IdTransacao { get; set; }
        public string idPedido { get; set; }
        public string Status { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}
