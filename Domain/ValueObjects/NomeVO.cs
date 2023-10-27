using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class NomeVO
    {
        public string Nome { get; }

        public NomeVO(string nome) 
        {
            if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome inválido.");

            Nome =  nome;
        }

        public override string ToString()
        {
            return Nome;
        }
    }

   
}
