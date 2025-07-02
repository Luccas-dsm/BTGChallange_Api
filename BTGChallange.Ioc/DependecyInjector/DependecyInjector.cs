using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BTGChallange.Domain.Interfaces;
using BTGChallange.Repository.DynamoDb;
using BTGChallange.Service.Interfaces;
using BTGChallange.Service.Servicos;
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
            // AddValidators(service);
        }

        /// <summary>
        /// Injeções de dependencia relacionadas ao Service
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddService(IServiceCollection service)
        {
            service.AddScoped<IServicoLimiteConta, ServicoLimiteConta>();
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

                    // Iterando sobre os erros do ModelState
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


        }

        /// <summary>
        /// Injeções de dependencia relacionadas ao Repository
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddRepository(IServiceCollection service, IConfiguration configuration)
        {

            service.AddSingleton<IAmazonDynamoDB>(provider =>
            {
                var environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = environment == "Development";

                if (isDevelopment)
                {
                    // Desenvolvimento: DynamoDB Local
                    var localConfig = new AmazonDynamoDBConfig
                    {
                        ServiceURL = "http://localhost:8000",
                        RegionEndpoint = RegionEndpoint.USEast1
                    };
                    var fakeCredentials = new BasicAWSCredentials("fake", "fake");
                    return new AmazonDynamoDBClient(fakeCredentials, localConfig);
                }
                else
                {
                    // Produção: AWS Real (usa credenciais automáticas)
                    var prodConfig = new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(
                            configuration.GetValue<string>("AWS:Region", "us-east-1")
                        )
                    };

                    // O SDK automaticamente procura credenciais em:
                    // 1. Variáveis de ambiente
                    // 2. Arquivo ~/.aws/credentials
                    // 3. IAM Roles (se rodando na AWS)
                    return new AmazonDynamoDBClient(prodConfig);
                }
            });

            // Repositórios
            service.AddScoped<IRepositorioLimiteConta, RepositorioLimiteContaDynamoDb>();
        }
    }
}
