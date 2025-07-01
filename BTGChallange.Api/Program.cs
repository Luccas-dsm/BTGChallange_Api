using System.Text;
using System.Text.Json;
using Integrador.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Integrador
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureAutehntication(builder);
            ConfigureServices(builder);
            AddConnections();
            ConfigureApplication(builder.Build(), builder.Configuration);
        }

        /// <summary>
        /// Adiciona as licensas e connections strings aos projetos
        /// </summary>
        private static void AddConnections()
        {

        }

        /// <summary>
        /// Retorna o conteudo de um json especifico, normalmente appSettinhs.json
        /// </summary>
        /// <param name="pathDirectory">Caminho do Diretorio</param>
        /// <param name="jsonNome">Nome do arquivo json</param>
        /// <param name="section">Nome do valor que deseja buscar</param>
        /// <returns></returns>
        private static string GetConfigurationBuilder(string pathDirectory, string jsonNome, string section)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                                .SetBasePath(pathDirectory)
                                .AddJsonFile(jsonNome)
                                .Build();

            return Configuration[section];
        }

        /// <summary>
        /// Adiciona os serviços ao builder
        /// </summary>
        /// <param name="builder"></param>
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Configuração do CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin() // Permitir todas as origens
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });


            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Adiciona as configurações do swagger
            builder.Services.AddSwaggerConfiguration();

            //Injeções de dependencia do projeto IOC
            builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);

            //Injeções do auto mapper
            builder.Services.AddDAutoMapperInjectionConfiguration();

        }

        public static void ConfigureAutehntication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(a =>
            {
                a.RequireHttpsMetadata = true;
                a.SaveToken = false;
                a.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8
                        .GetBytes(builder.Configuration["jwt:key"]!)
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromMinutes(60)
                };

                a.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new { message = "Token invalido." });
                        return context.Response.WriteAsync(result);

                    },

                };

            });
        }


        /// <summary>
        /// Configuração do webApplication
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureApplication(WebApplication app, IConfiguration configuration)
        {
            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //}
            app.UseSwaggerSetup(configuration);

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
