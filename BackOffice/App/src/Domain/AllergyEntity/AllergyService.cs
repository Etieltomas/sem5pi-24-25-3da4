using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Sempi5.Domain.AllergyEntity
{
    public class AllergyService
    {
        private readonly IConfiguration _configuration;
        private readonly string base_url;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AllergyService> _logger;

        // Constructor with HttpClient and ILogger dependency injection
        public AllergyService(IConfiguration configuration, HttpClient httpClient, ILogger<AllergyService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
            base_url = _configuration["IpAddresses:BackEnd2"] ?? "http://localhost:3001";
        }

        // Method to add a new allergy using DTO
        public async Task<string> AddAllergy([FromBody] AllergyDTO allergyDTO)
        {
            var url = base_url+"/api/allergies";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Create the JSON payload
            var content = new StringContent(JsonSerializer.Serialize(allergyDTO, options), Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                // Return null if the request was not successful
                return null;
            }
            // Return the response body as a string
            return await response.Content.ReadAsStringAsync();
        }

        // Method to get all allergies as a list of DTOs
        public async Task<List<AllergyDTO>> GetAllAllergies()
        {
            var url = base_url + "/api/allergies"; // Certifique-se de que a URL está correta

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Log the error status code
                _logger.LogError($"Failed to get allergies. Status code: {response.StatusCode}");
                // Return null if the request was not successful
                return null;
            }

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();

            // Log the JSON content for debugging
            _logger.LogInformation($"JSON content: {json}");

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                // Parse the JSON document
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    // Extract the _value property
                    var valueElement = doc.RootElement.GetProperty("_value");

                    // Deserialize the _value property to a list of AllergyDTO
                    var allergies = JsonSerializer.Deserialize<List<AllergyDTO>>(valueElement.GetRawText(), options);
                    return allergies;
                }
            }
            catch (JsonException ex)
            {
                // Log the exception and the JSON content for debugging
                _logger.LogError($"Error deserializing JSON: {ex.Message}");
                _logger.LogError($"JSON content: {json}");
                return null;
            }
        }
    }
}