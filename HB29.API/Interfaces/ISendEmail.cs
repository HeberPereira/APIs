using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.Interfaces
{
    public interface ISendEmail
    {
        Task<bool> SendEmailSmtpAsync(string email, string subject, string message);

        Task<bool> SendEmailGraphAsync(string email, string subject, string message);
    }
}
