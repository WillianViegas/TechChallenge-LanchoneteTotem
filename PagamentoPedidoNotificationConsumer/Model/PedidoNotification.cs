namespace PagamentoPedidoNotificationConsumer.Model
{
    public class PedidoNotification
    {
        public Guid IdTransacao { get; set; }
        public string idPedido { get; set; }
        public string Status { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}