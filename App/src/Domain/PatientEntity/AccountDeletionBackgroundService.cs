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
                // Registar a exceção
                Console.WriteLine($"Error in ProcessScheduledDeletions: {ex.Message}");
            }
        }

        // Espera 1 segundo até a próxima execução
        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
    }
    }
}