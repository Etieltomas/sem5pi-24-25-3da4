using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Sempi5.Domain.AllergyEntity
{
    public class AllergyService
    {
        private readonly IConfiguration _configuration;
        private readonly string base_url;
        private readonly HttpClient _httpClient;

        // Constructor with HttpClient dependency injection
        public AllergyService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
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
            var url = base_url;

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
            var allergies = JsonSerializer.Deserialize<List<AllergyDTO>>(json);

            // Convert to a list of DTOs
            return allergies;
        }
    }
}
