using hb29.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using hb29.API.Interfaces;

namespace hb29.API.Services
{
    public class SendEmail : ISendEmail
    {
        private IConfiguration _configuration;
        private readonly ILogger<SendEmail> _logger;
        public SendEmail(IConfiguration Configuration, ILogger<SendEmail> logger)
        {
            _configuration = Configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailSmtpAsync(string email, string subject, string message)
        {
            try
            {
                /*SEND MAIL TO USER*/
                MailParametersDTO objReturnToEmail = new MailParametersDTO();
                objReturnToEmail.Email = email;
                objReturnToEmail.Assunto = subject;
                objReturnToEmail.Mensagem = message;

                //get config send mail
                string endpointUrl = _configuration.GetValue<string>("SharedApi:UrlSmptMail");

                var httpClient = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpointUrl),
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(objReturnToEmail), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
                };
                //add default headers and token
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
                if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                //send mail
                var result = await httpClient.SendAsync(request);
                await result.Content.ReadAsStringAsync();

                return true;
            }
            catch (WebException ex)
            {
                var remoteErrorCode = ((HttpWebResponse)ex.Response).StatusCode;
                _logger.LogWarning($"Send Email - StatusCode Error: {remoteErrorCode}, An error just happened");
                
                return false;
                     
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"Send Email - StatusCode Error: {ex.Message}, An error just happened");
                return false;
            }
        }

        public async Task<bool> SendEmailGraphAsync(string email, string subject, string message)
        {
            try
            {
                /*SEND MAIL TO USER*/
                MailParametersDTO objReturnToEmail = new MailParametersDTO();
                objReturnToEmail.Email = email;
                objReturnToEmail.Assunto = subject;
                objReturnToEmail.Mensagem = message;

                //get config send mail
                string endpointUrl = _configuration.GetValue<string>("SharedApi:UrlGraphMail");

                var httpClient = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpointUrl),
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(objReturnToEmail), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
                };
                //add default headers and token
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
                if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                //send mail
                var result = await httpClient.SendAsync(request);
                await result.Content.ReadAsStringAsync();

                return true;
            }
            catch (WebException ex)
            {
                var remoteErrorCode = ((HttpWebResponse)ex.Response).StatusCode;
                _logger.LogWarning($"Send Email - StatusCode Error: {remoteErrorCode}, An error just happened");

                return false;

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Send Email - StatusCode Error: {ex.Message}, An error just happened");
                return false;
            }
        }
    }
}
