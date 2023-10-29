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
            if (email == null) throw new ArgumentNullException("E-mail inválido.");

            Email = email;

            if (Email != "")
                Validar();
        }

        public override string ToString()
        {
            return Email;
        }

        public bool Validar()
        {
            var trimmedEmail = Email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
