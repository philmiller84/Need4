﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Helpers
{
    public class Authentication
    {
        public Authentication(IConfiguration configuration) { Configuration = configuration; }
        public IConfiguration Configuration { get; private set; }


        public void SetOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            // Set the authority to your Auth0 domain
            options.Authority = $"https://{Configuration["Auth0:Domain"]}";

            // Configure the Auth0 Client ID and Client Secret
            options.ClientId = Configuration["Auth0:ClientId"];
            options.ClientSecret = Configuration["Auth0:ClientSecret"];

            // Set response type to code
            options.ResponseType = "code";

            // Configure the scope
            options.Scope.Clear();
            options.Scope.Add("profile");
            options.Scope.Add("openid");
            options.Scope.Add("name");
            options.Scope.Add("email");
            options.Scope.Add("picture");

            // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
            // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
            //options.CallbackPath = new PathString("/callback");
            options.CallbackPath = new PathString("/authentication/login-callback");

            // Configure the Claims Issuer to be Auth0
            options.ClaimsIssuer = "Auth0";
            options.Events = new OpenIdConnectEvents
            {
                // handle the logout redirection 
                OnRedirectToIdentityProviderForSignOut = HandleRedirectToIdentityProviderForSignOut,
                OnTicketReceived = OnTicketReceived
            };
        }
        public Task OnTicketReceived(TicketReceivedContext context)
        {
            // Get the ClaimsIdentity
            ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
            if (identity != null)
            {
                // Add the Name ClaimType. This is required if we want User.Identity.Name to actually return something!
                if (!context.Principal.HasClaim(c => c.Type == ClaimTypes.Name) && identity.HasClaim(c => c.Type == "name"))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name, identity.FindFirst("name").Value));
                }

                // Check if token names are stored in Properties
                if (context.Properties.Items.ContainsKey(".TokenNames"))
                {
                    // Token names a semicolon separated
                    string[] tokenNames = context.Properties.Items[".TokenNames"].Split(';');

                    // Add each token value as Claim
                    foreach (string tokenName in tokenNames)
                    {
                        // Tokens are stored in a Dictionary with the Key ".Token.<token name>"
                        string tokenValue = context.Properties.Items[$".Token.{tokenName}"];
                        identity.AddClaim(new Claim(tokenName, tokenValue));
                    }
                }

                Need4Service service = new Need4Service();
                Need4Protocol.UserService.UserServiceClient userService = service.GetUserClient();
                Need4Protocol.PermissionService.PermissionServiceClient permissionService = service.GetPermissionClient();
                string email = Claims.GetEmail(context.Principal.Claims);
                Need4Protocol.User user = new Need4Protocol.User { Email = email };
                Need4Protocol.User response = userService.GetUser(user);
                if (!response.Created)
                {
                    response = userService.CreateUser(user);
                }
                else
                {
                    Need4Protocol.PermissionSet permissions = permissionService.GetAllPermissions(user);
                }
            }

            return Task.CompletedTask;
        }

        public Task HandleRedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            string logoutUri = $"https://{Configuration["Auth0:domain"]}/v2/logout?client_id={Configuration["Auth0:clientId"]}";

            string postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    // transform to absolute
                    HttpRequest request = context.Request;
                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }
                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
            }

            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        }
    }
}
