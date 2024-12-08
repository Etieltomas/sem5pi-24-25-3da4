using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.PatientEntity;
using Sempi5.Infrastructure.PatientRepository;

namespace Sempi5.Domain.MedicalRecordEntity
{
    public class MedicalRecordService
    {
        private readonly IConfiguration _configuration;
        private readonly string base_url;
        private readonly HttpClient _httpClient;
        private IPatientRepository _patientRepository;

        // Constructor with HttpClient dependency injection
        public MedicalRecordService(IPatientRepository patientRepository, IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            base_url = _configuration["IpAddresses:BackEnd2"] ?? "http://localhost:3000";
            _patientRepository = patientRepository;
        }

        // Method to create a new medical record
        public async Task<MedicalRecordDTO> AddMedicalRecord(PatientDTO patientDTO)
        {
            var medicalRecordDTO = new MedicalRecordDTO
            {
                Patient = patientDTO.MedicalRecordNumber,
                Allergies = patientDTO.Allergies,
                Conditions = patientDTO.Conditions
            };
            
            var url = base_url+"/api/medicalRecord";

            // Make the POST request
            var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(medicalRecordDTO), Encoding.UTF8, "application/json"));

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
 
            return JsonSerializer.Deserialize<MedicalRecordDTO>(json);
        }

        // Method to get all medical recorsd as a list of DTOs
        public async Task<List<MedicalRecordDTO>> GetAllMedicalRecords()
        {
            var url = base_url+"/api/medicalRecord";

            // Make the GET request
            var response = await _httpClient.GetAsync(url);

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
            var medicalRecords = JsonSerializer.Deserialize<List<MedicalRecordDTO>>(json);

            return medicalRecords;
        }

        public async Task<List<MedicalRecordDTO>> Search(string? allergy, string? condition, int page, int pageSize)
        {
            var queryParams = new List<string>();

            // Add query parameters only if they are not null or empty
            if (!string.IsNullOrEmpty(allergy)) queryParams.Add($"allergy={Uri.EscapeDataString(allergy)}");
            if (!string.IsNullOrEmpty(condition)) queryParams.Add($"condition={Uri.EscapeDataString(condition)}");
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

            foreach (var medicalRecord in medicalRecords)
            {
                // Get the patient details
                var patient = await _patientRepository.GetPatientById(new PatientID(medicalRecord.Patient));
                if (patient != null)
                {
                    medicalRecord.Patient = patient.Name + " ("+ patient.Email+ ")" ;
                } else
                {
                    medicalRecord.Patient = "Unknown";
                }
            }
            return medicalRecords;
        }

        public async Task<MedicalRecordDTO> GetMedicalRecord(string patientId)
        {
            var url = base_url+"/api/medicalRecord/"+patientId;

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

        public async Task<MedicalRecordDTO> UpdateRecord(string patientId, List<string> conditions, List<string> allergies)
        {
            var url = base_url+"/api/medicalRecord/edit/"+patientId;

            MedicalRecordDTO record;

            if (conditions == null && allergies == null)
            {
                return null;
            } else if (conditions == null)
            {
                record = new MedicalRecordDTO
                {
                    Allergies = allergies
                };
            } else if (allergies == null)
            {
                record = new MedicalRecordDTO
                {
                    Conditions = conditions
                };
            } else
            {
                record = new MedicalRecordDTO
                {
                    Conditions = conditions,
                    Allergies = allergies
                };
                
            }

            // Make the PATCH request
            var response = await _httpClient.PutAsync(url, new StringContent(JsonSerializer.Serialize(record), Encoding.UTF8, "application/json"));

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();
            var medicalRecord = JsonSerializer.Deserialize<MedicalRecordDTO>(json);

            return medicalRecord;
        }

        // Convert domain model to DTO
        private MedicalRecordDTO ConvertToDTO(MedicalRecord medicalRecord)
        {
            return new MedicalRecordDTO
            {
                Patient = medicalRecord.Patient.ToString(),
                Allergies = medicalRecord.Allergies.Select(a => a.ToString()).ToList(),
                Conditions = medicalRecord.Conditions.Select(c => c.ToString()).ToList()
            };
        }
    }
}
