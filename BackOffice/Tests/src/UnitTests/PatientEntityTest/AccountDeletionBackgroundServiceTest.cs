using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Sempi5;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class AccountDeletionBackgroundServiceTests
{

    private readonly Mock<IPatientRepository> _mockPatientRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<EmailService> _mockEmailService;
    private readonly Mock<Serilog.ILogger> _mockLogger;
    private readonly Mock<PatientService> _mockpatientService;

    public AccountDeletionBackgroundServiceTests()
    {
        _mockPatientRepo = new Mock<IPatientRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmailService = new Mock<EmailService>();
        _mockLogger = new Mock<Serilog.ILogger>();
        _mockLogger.Setup(logger => logger.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(_mockLogger.Object);

        _mockpatientService = new Mock<PatientService>(
            _mockPatientRepo.Object,
            _mockUserRepo.Object,
            _mockUnitOfWork.Object,
            _mockEmailService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsExceptionOnProcessError()
    {
        // Arrange
        _mockpatientService.Setup(service => service.ProcessScheduledDeletions())
                          .ThrowsAsync(new Exception("Test exception"));

        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceProviderMock.Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
                           .Returns(serviceScopeFactoryMock.Object);
        serviceScopeFactoryMock.Setup(factory => factory.CreateScope())
                               .Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(scope => scope.ServiceProvider)
                        .Returns(serviceProviderMock.Object);

        serviceProviderMock.Setup(provider => provider.GetService(typeof(PatientService)))
                           .Returns(_mockpatientService.Object);

        var backgroundService = new AccountDeletionBackgroundService(serviceProviderMock.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => backgroundService.StartAsync(cts.Token));
    }

    [Fact]
    public async Task ExecuteAsync_CallsProcessScheduledDeletionsPeriodically()
    {
        // Arrange
        var callCount = 0;
        _mockpatientService.Setup(service => service.ProcessScheduledDeletions())
            .Callback(() => callCount++)
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceProviderMock.Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
                           .Returns(serviceScopeFactoryMock.Object);
        serviceScopeFactoryMock.Setup(factory => factory.CreateScope())
                               .Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(scope => scope.ServiceProvider)
                        .Returns(serviceProviderMock.Object);

        serviceProviderMock.Setup(provider => provider.GetService(typeof(PatientService)))
                           .Returns(_mockpatientService.Object);

        var backgroundService = new AccountDeletionBackgroundService(serviceProviderMock.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)); // give enough time for periodic calls

        // Act
        await backgroundService.StartAsync(cts.Token);
        
        // Wait to let the loop run
        await Task.Delay(TimeSpan.FromSeconds(2));
        
        // Assert
        Assert.True(callCount > 0, "ProcessScheduledDeletions should be called at least once.");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationTokenStopsExecutionGracefully()
    {
        // Arrange
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceProviderMock.Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
                           .Returns(serviceScopeFactoryMock.Object);
        serviceScopeFactoryMock.Setup(factory => factory.CreateScope())
                               .Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(scope => scope.ServiceProvider)
                        .Returns(serviceProviderMock.Object);

        // Ensuring PatientService is mocked
        serviceProviderMock.Setup(provider => provider.GetService(typeof(PatientService)))
                           .Returns(_mockpatientService.Object);

        var backgroundService = new AccountDeletionBackgroundService(serviceProviderMock.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act
        await backgroundService.StartAsync(cts.Token);
        
        // Assert - should not throw exception
        Assert.True(true);
    }

}