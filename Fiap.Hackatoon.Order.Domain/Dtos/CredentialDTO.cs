using System.Text.Json.Serialization;

namespace FIAP.TechChallenge.ContactsInsertProducer.Domain.DTOs.Application
{
    public record CredentialDTO
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
