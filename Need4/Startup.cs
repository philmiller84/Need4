using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;

namespace Need4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private Need4Context db { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //    options.Authority = "https://need4.us.auth0.com/";
            //    options.Audience = "https://need4-api";
            //});
            //services.AddAuthorization();
            services.AddGrpc();
            services.AddTransient<Models.Need4Context>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app.UseAuthentication();  //new statement
            //app.UseAuthorization();       //new statement

            app.UseEndpoints(endpoints =>
            {
                //private const int Port = 50051;
                endpoints.MapGrpcService<ItemRepositoryImpl>();
                endpoints.MapGrpcService<TradeServiceImpl>();
                endpoints.MapGrpcService<SaleServiceImpl>();
                endpoints.MapGrpcService<UserServiceImpl>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client." +
                        " To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
