using Incidents.Domain;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Moq;

namespace Unit_Tests.Handlers;

[TestClass]
public class UpdateIncidentStatusRequestHandlerTests
{
    [TestMethod]
    public async Task Handle_ReturnsGuidFromService()
    {
        // Arrange
        Guid expected = Guid.NewGuid();
        var incidentServiceMock = new Mock<IIncidentService>();
        incidentServiceMock
            .Setup(s => s.UpdateIncidentStatus(It.IsAny<Guid>(), It.IsAny<IncidentStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new UpdateIncidentStatusRequestHandler(incidentServiceMock.Object);
        var request = new UpdateIncidentStatusRequest
        {
            Id = Guid.NewGuid(),
            NewStatus = IncidentStatus.Resolved
        };

        // Act
        Guid result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expected, result);
        incidentServiceMock.Verify(s => s.UpdateIncidentStatus(It.IsAny<Guid>(), It.IsAny<IncidentStatus>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_ForwardsParametersToService_And_PassesCancellationToken()
    {
        // Arrange
        Guid expectedResult = Guid.NewGuid();
        var incidentServiceMock = new Mock<IIncidentService>();

        Guid capturedId = Guid.Empty;
        IncidentStatus capturedStatus = default;
        CancellationToken capturedToken = default;

        incidentServiceMock
            .Setup(s => s.UpdateIncidentStatus(It.IsAny<Guid>(), It.IsAny<IncidentStatus>(), It.IsAny<CancellationToken>()))
            .Callback((Guid id, IncidentStatus status, CancellationToken ct) =>
            {
                capturedId = id;
                capturedStatus = status;
                capturedToken = ct;
            })
            .ReturnsAsync(expectedResult);

        var handler = new UpdateIncidentStatusRequestHandler(incidentServiceMock.Object);

        Guid incidentId = Guid.NewGuid();
        IncidentStatus newStatus = IncidentStatus.Denied;
        var request = new UpdateIncidentStatusRequest
        {
            Id = incidentId,
            NewStatus = newStatus
        };

        var cts = new CancellationTokenSource();

        // Act
        Guid result = await handler.Handle(request, cts.Token);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(incidentId, capturedId);
        Assert.AreEqual(newStatus, capturedStatus);
        Assert.AreEqual(cts.Token, capturedToken);

        incidentServiceMock.Verify(s => s.UpdateIncidentStatus(incidentId, newStatus, It.IsAny<CancellationToken>()), Times.Once);
    }
}
