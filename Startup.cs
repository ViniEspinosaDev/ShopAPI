using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Shop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // env: tudo sobre o ambiente de desenvolvimento está nesse cara
            // app: todas as informações sobre a aplicação em si está nesse cara

            // se está em desenvolvimento
            if (env.IsDevelopment())
            {
                // Habilita uma página de exceção mais detalhada (com info que não podem mostrar em produção)
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop v1"));
            }

            /* 
                Força a API a responder sobre https. Primeiro passo de segurança é a API responder
                requisições https 
            */
            app.UseHttpsRedirection();

            // Padrão de rotas do ASP.Net MVC
            app.UseRouting();

            app.UseAuthorization();

            // Mapeamento das URLs
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
