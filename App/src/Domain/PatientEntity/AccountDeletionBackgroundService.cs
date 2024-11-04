using Microsoft.AspNetCore.Http.HttpResults;
using Sempi5.Domain.PatientEntity;

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