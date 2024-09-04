using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hb29.API.Helpers
{
    public class GraphHelper
    {
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphClient;

        //private class Tokens
        //{
        //    public string access_token { get; set; }
        //}
        public GraphHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _graphClient = CreateGrahpClient();
        }

        private GraphServiceClient CreateGrahpClient()
        {
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = _configuration["AzureAd:TenantId"];

            // Values from app registration
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            return new GraphServiceClient(clientSecretCredential, scopes);
        }

        public async Task<List<string>> GetUserGroups(string upn)
        {
            List<string> groups = new List<string>();
         
            var claisMemoryCache = Startup.ClaimsMemoryCache;

            //Pega os profiles armazenado no cache durante o login do usuario
            var profiles = claisMemoryCache.Get(upn);

            if (profiles == null || profiles.Count == 0)
            {
                //caso o cache esteja vazio busca no graph
                groups = await this.GetGroupsAsync(upn);
            }
            else
            {
                groups = profiles.Select(s => s.AdGroupId).ToList();
            }
            
            return groups;
        }

        public async Task<List<string>> GetGroupsAsync(string upn)
        {
            List<string> groupIds = new List<string>();

            var response = await _graphClient.Users[upn].GetMemberGroups(false).Request().PostAsync();

            do
            {
                groupIds.AddRange(response.CurrentPage);
            }
            while (response.NextPageRequest != null && (response = await response.NextPageRequest.PostAsync()).Count > 0);

            return groupIds;
        }

        public async Task<List<User>> GetUsersOfGroupAsync(string adGroupId)
        {
            var users = await _graphClient.Groups[adGroupId].Members.Request().Select("displayName,mail,onPremisesSamAccountName,companyName").GetAsync();
            List<User> listUser = new List<User>();
            do
            {
                listUser.AddRange(users.Cast<User>());
            }
            while (users.NextPageRequest != null && (users = await users.NextPageRequest.GetAsync()).Count > 0);

            return listUser;
        }
    }
}
