using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Sempi5.Domain.MedicalConditionEntity
{
    public class MedicalConditionService
    {
        private readonly IConfiguration _configuration;
        private readonly string base_url;
        private readonly HttpClient _httpClient;

        // Constructor with HttpClient dependency injection
        public MedicalConditionService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            base_url = _configuration["IpAddresses:BackEnd2"] ?? "http://localhost:3001";
        }

        // Method to add a new medical condition using DTO
        public async Task<string> AddMedicalCondition([FromBody] MedicalConditionDTO medicalConditionDTO)
        {
            var url = base_url + "/api/medical-conditions";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Create the JSON payload
            var content = new StringContent(JsonSerializer.Serialize(medicalConditionDTO, options), Encoding.UTF8, "application/json");

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

        // Method to get all medical conditions as a list of DTOs
        public async Task<List<MedicalConditionDTO>> GetAllMedicalConditions()
        {
            var url = base_url + "/api/medical-conditions";

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Return an empty list if the request was not successful
                return new List<MedicalConditionDTO>();
            }

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                // Extract the _value property
                var valueElement = doc.RootElement.GetProperty("_value");
            
                 // Deserialize the _value property to a list of AllergyDTO
                var medicalConditions = JsonSerializer.Deserialize<List<MedicalConditionDTO>>(valueElement.GetRawText(), options);
                return medicalConditions;
            }
        }
    }
}