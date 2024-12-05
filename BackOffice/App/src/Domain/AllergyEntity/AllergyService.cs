using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AllergyEntityEntity;

namespace Sempi5.Domain.AllergyEntity
{
    public class AllergyService
    {
        private readonly string base_url = "http://localhost:3000/api/allergies";
        private readonly HttpClient _httpClient;

        // Constructor with HttpClient dependency injection
        public AllergyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Method to add a new allergy using DTO
        public async Task<string> AddAllergy([FromBody] AllergyDTO allergyDTO)
        {
            var url = base_url;

            // Create the JSON payload
            var content = new StringContent(JsonSerializer.Serialize(allergyDTO), Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(url, content);

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
            var allergies = JsonSerializer.Deserialize<List<Allergy>>(json);

            // Convert to a list of DTOs
            return allergies.Select(a => ConvertToDTO(a)).ToList();
        }

        // Convert domain model to DTO
        private AllergyDTO ConvertToDTO(Allergy allergy)
        {
            return new AllergyDTO
            {
                Code = allergy.Code.ToInt(),
                Name = allergy.Name.ToString()
            };
        }
    }
}
