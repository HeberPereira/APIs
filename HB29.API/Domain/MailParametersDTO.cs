using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.DTOs
{
    public class MailParametersDTO
    {
        public string Email { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public Guid? ConcurrencyToken { get; set; }
    }
}
