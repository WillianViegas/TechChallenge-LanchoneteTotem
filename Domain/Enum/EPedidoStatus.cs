using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum EPedidoStatus
    {
        Novo = 0,
        PendentePagamento = 1,
        Pago = 2,
        Confirmado = 3,
        EmPreparo = 4,
        Pronto = 5,
        Finalizado = 6
    }
}
