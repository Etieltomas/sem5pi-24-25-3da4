using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.PatientEntity;
using Sempi5.Infrastructure.PatientRepository;
using System.Text.Json.Serialization;

namespace Sempi5.Domain.MedicalRecordEntity
{
    public class MedicalRecordService
    {
        private readonly IConfiguration _configuration;
        private readonly string base_url;
        private readonly HttpClient _httpClient;
        private IPatientRepository _patientRepository;
        private readonly ILogger<MedicalRecordService> _logger;

        // Constructor with HttpClient dependency injection
        public MedicalRecordService(IPatientRepository patientRepository, IConfiguration configuration, HttpClient httpClient, ILogger<MedicalRecordService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            base_url = _configuration["IpAddresses:BackEnd2"] ?? "http://localhost:3000";
            _patientRepository = patientRepository;
            _logger = logger;
        }

        // Method to create a new medical record
        public async Task<MedicalRecordDTO> AddMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {      
            var url = base_url+"/api/medicalRecord";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Make the POST request
            var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(medicalRecordDTO, options), Encoding.UTF8, "application/json"));

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
 
            return JsonSerializer.Deserialize<MedicalRecordDTO>(json);
        }

        // Method to get all medical records as a list of DTOs
        public async Task<List<MedicalRecordDTO>> GetAllMedicalRecords()
        {
            var url = base_url+"/api/medicalRecord";

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Return null if the request was not successful
                return null;
            }

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();

            return GetMedicalRecordsFromJson(json);
        }

        private List<MedicalRecordDTO> GetMedicalRecordsFromJson(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                // Extract the `_value` property
                var valueElement = doc.RootElement.GetProperty("_value");

                // Deserialize `_value` into a list of MedicalRecordDTO
                var medicalRecords = JsonSerializer.Deserialize<List<MedicalRecordDTO>>(valueElement.GetRawText());

                return medicalRecords;
            }
        }

        public async Task<List<MedicalRecordDTO>> Search(string? filter, string patientEmail, 
                                            int page, int pageSize)
        {
            var queryParams = new List<string>();

            // Add query parameters only if they are not null or empty
            if (!string.IsNullOrEmpty(filter)) queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
            
            queryParams.Add($"patientEmail={Uri.EscapeDataString(patientEmail)}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            // Join all query parameters with '&'
            var queryString = string.Join("&", queryParams);

            // Construct the full URL
            var url = $"{base_url}/api/medicalRecord/search?{queryString}";

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Return null if the request was not successful
                return null;
            }

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();

            var medicalRecords = JsonSerializer.Deserialize<List<MedicalRecordDTO>>(json);

            return medicalRecords;
        }

        public async Task<MedicalRecordDTO> GetMedicalRecord(string patientEmail)
        {
            var url = base_url+"/api/medicalRecord/"+patientEmail;

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Deserialize the JSON response
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            var json = await response.Content.ReadAsStringAsync();

            var medicalRecord = JsonSerializer.Deserialize<MedicalRecordDTO>(json);

            return medicalRecord;
        }
        public async Task<RecordLineDTO> GetEntryMedicalRecord(string idMedicalRecordLine)
        {
            var url = base_url + "/api/medicalRecord/entry/"+idMedicalRecordLine;

            // Fazer a requisição GET
            var response = await _httpClient.GetAsync(url);

            // Verificar se a resposta foi bem-sucedida
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Deserializar a resposta JSON
            var json = await response.Content.ReadAsStringAsync();

            // Deserializar o objeto RecordLineDTO
            var medicalRecordLine = JsonSerializer.Deserialize<RecordLineDTO>(json);

            return medicalRecordLine;
        }

        /*public async Task<RecordLineDTO> Update(RecordLineDTO recordLineDTO, string idMedicalRecordLine)
        {
            var url = base_url + "/api/medicalRecord/entry/" + idMedicalRecordLine + "/update";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _logger.LogInformation(recordLineDTO.ToString());

            // Criar um objeto anônimo com o campo RecordLine como um array
            var payload = new
            {
                RecordLine = new[] { recordLineDTO }
            };

            _logger.LogInformation("----------------" + payload.ToString());

            var content = new StringContent(JsonSerializer.Serialize(payload, options), Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending PATCH request to URL: {url}", url);
            _logger.LogInformation("Request content: {content}", content);

            try
            {
                var response = await _httpClient.PatchAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PATCH request failed with status code: {statusCode}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error response content: {errorContent}", errorContent);
                    return null;
                }

                // Deserializar a resposta JSON
                var json = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Response content: {json}", json);

                // Deserializar o objeto RecordLineDTO
                var recordLine = JsonSerializer.Deserialize<RecordLineDTO>(json);

                return recordLine;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while sending PATCH request: {message}", ex.Message);
                _logger.LogError("Stack Trace: {stackTrace}", ex.StackTrace);
                return null;
            }
        }*/

        public async Task<MedicalRecordDTO> Update(MedicalRecordDTO medicalRecordDTO, string idMedicalRecordLine)
        {
            var url = base_url + "/api/medicalRecord/entry/" + idMedicalRecordLine + "/update";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Make the POST request
            var response = await _httpClient.PatchAsync(url, new StringContent(JsonSerializer.Serialize(medicalRecordDTO, options), Encoding.UTF8, "application/json"));

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
 
            return JsonSerializer.Deserialize<MedicalRecordDTO>(json);
        }

        public async Task<MedicalRecordDTO> Anonymization(MedicalRecordDTO medicalRecordDTO, string patientEmail)
        {
            var url = base_url + "/api/medicalRecord/"+ patientEmail + "/anonimize";

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Make the POST request
            var response = await _httpClient.PatchAsync(url, new StringContent(JsonSerializer.Serialize(medicalRecordDTO, options), Encoding.UTF8, "application/json"));

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
 
            return JsonSerializer.Deserialize<MedicalRecordDTO>(json);
        }
    }
}