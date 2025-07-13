using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.Integrations;
using FIAP.TechChallenge.ContactsInsertProducer.Domain.DTOs.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fiap.Hackatoon.Order.Integrations.v1
{
    public class ProductConsultManager(ILogger<ProductConsultManager> logger, IConfiguration configuration) : IProductConsultManager
    {
        private readonly ILogger<ProductConsultManager> _logger = logger;

        private readonly IConfiguration _configuration = configuration;

        public async Task<Product> GetProductById(string id, string token)
        {
            try
            {
                var url = $"{_configuration.GetSection("Integrations:ProductConsult")["BasePath"]}products/{id}";

                HttpClient cliente = new HttpClient();
                cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var resultado = await cliente.GetAsync(url);

                if (resultado != null && resultado.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = JsonConvert.DeserializeObject<Product>(await resultado.Content.ReadAsStringAsync());
                    return responseString;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Um erro aconteceu ao obter o Contato. Erro: {e.Message}.", e);
                return null; 
            }
        }

        public async Task<string> GetToken()
        {
            try
            {
                var url = _configuration.GetSection("Integrations:ProductConsult")["BasePath"]+"api/token";
                var body = new CredentialDTO
                {
                    Username = _configuration.GetSection("Credentials")["Username"],
                    Password = _configuration.GetSection("Credentials")["Password"]
                };

                HttpClient cliente = new();
                var json = JsonConvert.SerializeObject(body);
                StringContent httpContent = new(json, System.Text.Encoding.UTF8, "application/json");

                var resultado = await cliente.PostAsync(url, httpContent);

                if (resultado != null && resultado.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await resultado.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                _logger.LogError($"Um erro aconteceu ao obter o token. Erro: {e.Message}.", e);
                return string.Empty;
            }
        }
    }
}
