using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.ValueObjects
{
    public class CpfVO
    {

        public string Numero { get; private set; }

        public CpfVO(string numero)
        {
            if (numero == null) throw new ArgumentNullException("CPF inválido.");
           
            Numero = numero;

            if(numero != "")
                Validar();
        }

        public override string ToString()
        {
            return Numero;
        }

        public bool Validar()
        {
            if (Numero.Length > 11)
                throw new Exception("CPF maior que 11 caracteres.");

            while (Numero.Length != 11)
                Numero = '0' + Numero;

            var igual = true;
            for (var i = 1; i < 11 && igual; i++)
                if (Numero[i] != Numero[0])
                    igual = false;

            if (igual || Numero == "12345678909")
                throw new Exception("Numeros no CPF são inguais.");

            var numeros = new int[11];

            for (var i = 0; i < 11; i++)
                numeros[i] = int.Parse(Numero[i].ToString());

            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            var resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    throw new Exception("CPF inválido.");
            }
            else if (numeros[9] != 11 - resultado)
                throw new Exception("CPF inválido.");

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    throw new ArgumentNullException("CPF inválido.");
            }
            else if (numeros[10] != 11 - resultado)
                throw new ArgumentNullException("CPF inválido.");

            return true;
        }
    }
}
