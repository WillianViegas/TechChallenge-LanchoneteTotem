using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class EmailVO
    {
        public string Email { get; private set; }

        public EmailVO(string email) 
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("E-mail inválido.");

            Email = email;
        }

        public override string ToString()
        {
            return Email;
        }
    }
}
