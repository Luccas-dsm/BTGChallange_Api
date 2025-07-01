using System.Text.Json;
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


        //IMPORTANTE: OS SERVIÇOS QUE UTILIZAM URL BASE DO CLIENTE HTTP DEVEM SER REGISTRADOS SOMENTE NO ADDHTTPCLIENT E NÃO EM OUTRO TIPO DE INJEÇÃO DE DEPENDENCIA
        //CASO CONTRÁRIO, O CLIENTE HTTP NÃO SERÁ INJETADO CORRETAMENTE
        /// <summary>
        /// Injeções de dependencia relacionadas ao Service
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddService(IServiceCollection service)
        {


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

          //  service.AddValidatorsFromAssemblyContaining<ProdutoValidator>();

        }

        /// <summary>
        /// Injeções de dependencia relacionadas ao Repository
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void AddRepository(IServiceCollection service, IConfiguration configuration)
        {
        }
    }
}
