using Moq;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Infrastructure;
using Xunit;
using System.Threading.Tasks;
using Sempi5.Domain.Shared;

public class OperationRequestServiceIntegrationTests
{
    private readonly Mock<IOperationRequestRepository> _mockRequestRepo;
    private readonly Mock<IStaffRepository> _mockStaffRepo;
    private readonly Mock<IPatientRepository> _mockPatientRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly OperationRequestService _service;

    public OperationRequestServiceIntegrationTests()
    {
        _mockRequestRepo = new Mock<IOperationRequestRepository>();
        _mockStaffRepo = new Mock<IStaffRepository>();
        _mockPatientRepo = new Mock<IPatientRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new OperationRequestService(_mockRequestRepo.Object, null, _mockStaffRepo.Object, _mockPatientRepo.Object, _mockUnitOfWork.Object, null);
    }

    [Fact]
    public async Task Should_Create_OperationRequest_When_Valid_DTO()
    {
        var dto = new OperationRequestCreateDTO
        {
            StaffId = "1",
            PatientId = "1",
            OperationTypeId = "1",
            Priority = "High",
            Deadline = "01-01-2025"
        };

        _mockRequestRepo.Setup(r => r.AddAsync(It.IsAny<OperationRequest>())).ReturnsAsync(new OperationRequest());

        var result = await _service.CreateOperationRequest(dto);

        Assert.Equal("1", result.StaffId);
        Assert.Equal("High", result.Priority);
    }
}
