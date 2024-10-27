using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure;
using Xunit;
using System;
using System.Threading.Tasks;

public class OperationRequestRepositoryContainerIntegrationTests
{
    private readonly OperationRequestRepository _repository;
    private readonly DataBaseContext _context;

    public OperationRequestRepositoryContainerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataBaseContext(options);
        _repository = new OperationRequestRepository(_context);
    }

    [Fact]
    public async Task Should_Add_And_Get_OperationRequest()
    {
        var operationRequest = new OperationRequest
        {
            Deadline = new Deadline(DateTime.Now.AddDays(10)),
            Priority = Priority.High,
            Status = Status.Pending
        };

        await _repository.AddAsync(operationRequest);
        await _context.SaveChangesAsync();

        var retrievedRequest = await _repository.GetOperationRequestById(operationRequest.Id);

        Assert.Equal(operationRequest.Id, retrievedRequest.Id);
        Assert.Equal("High", retrievedRequest.Priority.Value);
    }
}
