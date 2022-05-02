using GloboTicket.Services.ShoppingBasket.Extensions;
using GloboTicket.Services.ShoppingBasket.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace GloboTicket.Services.ShoppingBasket.Services
{
    public class DiscountService: IDiscountService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor httpContextAccessor;
        private string accessToken;

        public DiscountService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client; 
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetToken()
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }

            // Retrieve the OIDC IDP discovery doc
            var idpDoc = await client.GetDiscoveryDocumentAsync("https://localhost:5010/");
            if(idpDoc.IsError)
            {
                throw new Exception(idpDoc.Error);
            }

            // Get subject token (current access token)
            var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            // Build a dictionary of your custom parameters
            var parameters = new Dictionary<string, string>
            {
                { "subject_token", token },
                { "subject_token_type", "urn:ietf:params:oauth:token_type:access_token" },
                { "scope", "openid profile discount.fullaccess" }
            };
            var result = await client.RequestTokenAsync(new TokenRequest
            {
                Address = idpDoc.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant_type:token_exchange",
                Parameters = parameters,
                ClientId = "shoppingbaskettodownstreamtokenexchangeclient",
                ClientSecret = "adb80e88-78bc-4512-a382-857c688d78d5"
            });
            if(result.IsError)
            {
                throw new Exception(result.Error);
            }

            accessToken = result.AccessToken;
            return accessToken;
        }

        public async Task<Coupon> GetCoupon(Guid userId)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/discount/user/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.ReadContentAs<Coupon>();
        }
    }
}
