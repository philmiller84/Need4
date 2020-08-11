using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XYZ.Data;

namespace XYZ
{
    public class AuthenticationHelper
    {
        public AuthenticationHelper(IConfiguration configuration) { Configuration = configuration; }
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
            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity != null)
            {
                // Add the Name ClaimType. This is required if we want User.Identity.Name to actually return something!
                if (!context.Principal.HasClaim(c => c.Type == ClaimTypes.Name) && identity.HasClaim(c => c.Type == "name"))
                    identity.AddClaim(new Claim(ClaimTypes.Name, identity.FindFirst("name").Value));

                // Check if token names are stored in Properties
                if (context.Properties.Items.ContainsKey(".TokenNames"))
                {
                    // Token names a semicolon separated
                    string[] tokenNames = context.Properties.Items[".TokenNames"].Split(';');

                    // Add each token value as Claim
                    foreach (var tokenName in tokenNames)
                    {
                        // Tokens are stored in a Dictionary with the Key ".Token.<token name>"
                        string tokenValue = context.Properties.Items[$".Token.{tokenName}"];
                        identity.AddClaim(new Claim(tokenName, tokenValue));
                    }
                }
                var service = new Need4Service();
                var userService = service.GetUserClient();
                var user = new Need4Protocol.User { Id = 0, Name = "phil", Email = "phil.miller84@gmail.com", Created = false };
                var response = userService.GetUser(user);
                if (!response.Created)
                {
                    response = userService.CreateUser(user);
                }
            }

            return Task.CompletedTask;
        }

        public Task HandleRedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            var logoutUri = $"https://{Configuration["Auth0:domain"]}/v2/logout?client_id={Configuration["Auth0:clientId"]}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    // transform to absolute
                    var request = context.Request;
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
