using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BTGChallange.Domain.Interfaces;
using BTGChallange.Repository.DynamoDb;
using BTGChallange.Repository.DynamoDbConf;
using BTGChallange.Service.Interfaces;
using BTGChallange.Service.Servicos;
using BTGChallange.Service.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integrador.Ioc.DependecyInjector
{
    public class DependecyInjector
    {

        public static void Register(IServiceCollection service, IConfiguration configuration)
        {
            AddRepository(service, configuration);
            AddService(service);
            AddValidators(service);
        }

        /// <summary>
        /// Injeções de dependencia relacionadas ao Service
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddService(IServiceCollection service)
        {
            service.AddScoped<IServicoLimiteConta, ServicoLimiteConta>();
            service.AddScoped<IServicoTransacaoPix, ServicoTransacaoPix>();

        }


        /// <summary>
        /// Injeções de dependencia relacionadas aos validators
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddValidators(IServiceCollection service)
        {
            service.AddFluentValidationAutoValidation()
                   .AddFluentValidationClientsideAdapters();


            service.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var response = new
                    {
                        ErrorMessage = new Dictionary<string, string[]>(),
                        Success = false,
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                    
                    foreach (var (key, value) in context.ModelState)
                    {
                        if (value.Errors.Any())
                        {
                            response.ErrorMessage.Add(key, value.Errors.Select(e => e.ErrorMessage).ToArray());
                        }
                    }

                    return new UnprocessableEntityObjectResult(response);
                };
            });

            service.AddValidatorsFromAssemblyContaining<CadastrarLimiteDtoValidator>();
            service.AddValidatorsFromAssemblyContaining<AtualizarLimiteDtoValidator>();

        }


        /// <summary>
        /// Injeções de dependencia relacionadas ao Repository
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddRepository(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAmazonDynamoDB>(_ =>
            {
                var config = new AmazonDynamoDBConfig();

                // Configuração da região
                var region = configuration["AWS:Region"] ?? "us-east-1";
                config.RegionEndpoint = RegionEndpoint.GetBySystemName(region);

                var serviceUrl = Environment.GetEnvironmentVariable("AWS_ServiceURL") ?? configuration["DynamoDB:ServiceURL"];
                              
                // Verifica se é DynamoDB Local
                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    config.ServiceURL = serviceUrl;

                    // Para DynamoDB Local, pode usar credenciais fake
                    var accessKey = configuration["AWS:AccessKey"] ?? "fake";
                    var secretKey = configuration["AWS:SecretKey"] ?? "fake";
                    var credentials = new BasicAWSCredentials(accessKey, secretKey);
                    var client = new AmazonDynamoDBClient(credentials, config);
                    
                    new ManagerTablesDynamoDb(client); //cria as tabelas caso não existam

                    return client;
                }

                // Para produção, verifica se tem credenciais no appsettings
                var prodAccessKey = configuration["AWS:AccessKey"];
                var prodSecretKey = configuration["AWS:SecretKey"];

                if (!string.IsNullOrEmpty(prodAccessKey) && !string.IsNullOrEmpty(prodSecretKey))
                {
                    var credentials = new BasicAWSCredentials(prodAccessKey, prodSecretKey);
                    return new AmazonDynamoDBClient(credentials, config);
                }

                // Usa detecção automática de credenciais (variáveis de ambiente, IAM roles, etc.)
                return new AmazonDynamoDBClient(config);
            });

            services.AddScoped<IRepositorioLimiteConta, RepositorioLimiteContaDynamoDb>();
            services.AddScoped<IRepositorioTransacaoPix, RepositorioTransacaoPixDynamoDb>();

        }
    }
}
