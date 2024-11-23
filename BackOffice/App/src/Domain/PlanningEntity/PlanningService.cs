using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Sempi5.Domain.StaffEntity;

public class PlanningService
{
    private readonly HttpClient _httpClient;

    public PlanningService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StaffDTO>> GetAvailableStaffAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:5012/api/Staff/Available");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<StaffDTO>>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<StaffDTO>();
    }

    public async Task<List<OperationRequestDto>> GetScheduledOperationsAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:5012/api/OperationRequest");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var operations = JsonSerializer.Deserialize<List<OperationRequestDto>>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return operations?.Where(o => o.Status.Equals("scheduled", StringComparison.OrdinalIgnoreCase)).ToList()
               ?? new List<OperationRequestDto>();
    }

    public async Task<PlanningDto> GetPlanningAsync()
    {
        var availableStaffTask = GetAvailableStaffAsync();
        var scheduledOperationsTask = GetScheduledOperationsAsync();

        await Task.WhenAll(availableStaffTask, scheduledOperationsTask);

        return new PlanningDto
        {
            AvailableStaffs = availableStaffTask.Result ?? new List<StaffDTO>(), 
            ScheduledOperations = scheduledOperationsTask.Result ?? new List<OperationRequestDto>() 
        };
    }
}