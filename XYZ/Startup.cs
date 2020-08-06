using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using XYZ.Data;



namespace XYZ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            //services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddServerSideBlazor();
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect("Auth0", options =>
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
            });

            services.AddSingleton<Need4Service>();
            services.AddSingleton<Models.Need4Context>();
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


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{ 
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}{id?}");
            //});

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
