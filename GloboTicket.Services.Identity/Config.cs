// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace GloboTicket.Services.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[] 
        {
            new ApiResource("eventcatalog", "Event Catalog API")
            {
                Scopes = { "eventcatalog.fullaccess", "eventcatalog.read", "eventcatalog.write" }
            },
            new ApiResource("shoppingbasket", "Shopping Basket API")
            {
                Scopes = { "shoppingbasket.fullaccess" }
            },
            new ApiResource("discount", "Discount API")
            {
                Scopes = { "discount.fullaccess" }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("eventcatalog.fullaccess"),
                new ApiScope("eventcatalog.read"),
                new ApiScope("eventcatalog.write"),
                new ApiScope("shoppingbasket.fullaccess"),
                new ApiScope("discount.fullaccess")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                //new Client
                //{
                //    ClientName = "GloboTicket Machine 2 Machine Client",
                //    ClientId = "globoticketm2m",
                //    ClientSecrets = { new Secret("09bd6e3e-20fb-4c0e-bce7-a7dfece50276".Sha256()) },
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    AllowedScopes = { "eventcatalog.fullaccess" }
                //},
                //new Client
                //{
                //    ClientName = "GloboTicket Interactive Client",
                //    ClientId = "globoticketinteractive",
                //    ClientSecrets = { new Secret("1d33dac1-e23b-4273-b255-56349da6e9e7".Sha256()) },
                //    AllowedGrantTypes = GrantTypes.Code,
                //    RedirectUris = { "https://localhost:5000/signin-oidc" },
                //    PostLogoutRedirectUris = { "https://localhost:5000/signout-oidc" },
                //    AllowedScopes = { "openid", "profile", "shoppingbasket.fullaccess" }
                //},
                new Client
                {
                    ClientName = "GloboTicket Client",
                    ClientId = "globoticket",
                    ClientSecrets = { new Secret("1d33dac1-e23b-4273-b255-56349da6e9e7".Sha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-oidc" },
                    AllowedScopes = { "openid", "profile", "shoppingbasket.fullaccess", "eventcatalog.read", "eventcatalog.write" }
                },
                new Client
                {
                    ClientName = "Shopping Basket Token Exchange Client",
                    ClientId = "shoppingbaskettodownstreamtokenexchangeclient",
                    ClientSecrets = { new Secret("adb80e88-78bc-4512-a382-857c688d78d5".Sha256()) },
                    AllowedGrantTypes = new string[] { "urn:ietf:params:oauth:grant_type:token_exchange" },
                    AllowedScopes = { "openid", "profile", "discount.fullaccess" }
                }
            };
    }
}