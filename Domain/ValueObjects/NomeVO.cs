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
            if (nome == null) throw new Exception("Nome inválido.");

            Nome =  nome;

            if (Nome != "")
                Validar();
        }

        public override string ToString()
        {
            return Nome;
        }

        public bool Validar()
        {
            if(Nome.Length <= 3)
            {
                throw new Exception("Nome inválido. O nome deve ter mais de 3 caracteres");
            }

            if (Nome.Length >= 30)
            {
                throw new Exception("Nome inválido. O nome deve ter menos de 30 caracteres");
            }

            return true;
        } 
    }

   
}
