using Microsoft.AspNetCore.Http.HttpResults;
using Sempi5.Domain.PatientEntity;

/// <summary>
/// @author Simão Lopes
/// @date 1/12/2024
/// This background service is responsible for periodically processing scheduled patient account deletions.
/// It runs in the background and invokes the PatientService to handle the deletion logic every 30 minutes.
/// The service will continue running as long as the application is active, executing the deletion process
/// at the specified interval. Errors during processing are caught and thrown with a descriptive message.
/// </summary>
public class AccountDeletionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;


    public AccountDeletionBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var patientDeletionService = scope.ServiceProvider.GetRequiredService<PatientService>();
                try
                {
                    await patientDeletionService.ProcessScheduledDeletions();
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while processing your request." + ex.Message);
                }
            }

            int refreshSeconds = 60 * 30;

            await Task.Delay(TimeSpan.FromSeconds(refreshSeconds), stoppingToken);
        }
    }
}