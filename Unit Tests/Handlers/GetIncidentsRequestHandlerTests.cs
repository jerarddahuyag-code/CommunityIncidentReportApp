using Incidents.Domain;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Moq;

namespace Unit_Tests.Handlers;

[TestClass]
public class GetIncidentsRequestHandlerTests
{
    [TestMethod]
    public async Task Handle_ReturnsMappedItems_WhenServiceReturnsIncidents()
    {
        // Arrange
        var incidents = new[]
        {
            new IncidentDomain
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                DisplayName = "Alice",
                Title = "Pothole",
                Description = "Large pothole on 5th",
                Category = IncidentCategory.Maintenance,
                Latitude = 12.34,
                Longitude = 56.78,
                Status = IncidentStatus.Reported,
                ImageUrl = "http://example.com/image.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new IncidentDomain
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                DisplayName = null, // should map to "Unknown"
                Title = "Graffiti",
                Description = "Graffiti on wall",
                Category = IncidentCategory.Security,
                Latitude = null,
                Longitude = null,
                Status = IncidentStatus.Resolved,
                ImageUrl = null,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        var capturedToken = default(CancellationToken);
        var incidentServiceMock = new Mock<IIncidentService>();
        incidentServiceMock
            .Setup(s => s.GetIncidentsAsync(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken ct) => capturedToken = ct)
            .ReturnsAsync(incidents);

        var handler = new GetIncidentsRequestHandler(incidentServiceMock.Object);
        var cts = new CancellationTokenSource();

        // Act
        var response = await handler.Handle(new GetIncidentsRequest(), cts.Token);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Items.Length);

        var first = response.Items.First(i => i.Id == incidents[0].Id);
        Assert.AreEqual(incidents[0].UserId, first.UserId);
        Assert.AreEqual("Alice", first.Username);
        Assert.AreEqual(incidents[0].Title, first.Title);
        Assert.AreEqual(incidents[0].Description, first.Description);
        Assert.AreEqual(incidents[0].Category, first.Category);
        Assert.AreEqual(incidents[0].Latitude, first.Latitude);
        Assert.AreEqual(incidents[0].Longitude, first.Longitude);
        Assert.AreEqual(incidents[0].Status, first.Status);
        Assert.AreEqual(incidents[0].ImageUrl, first.ImageUrl);
        Assert.AreEqual(incidents[0].CreatedAt, first.CreatedAt);

        var second = response.Items.First(i => i.Id == incidents[1].Id);
        Assert.AreEqual(incidents[1].UserId, second.UserId);
        Assert.AreEqual("Unknown", second.Username);
        Assert.AreEqual(incidents[1].Title, second.Title);
        Assert.AreEqual(incidents[1].Description, second.Description);
        Assert.AreEqual(incidents[1].Category, second.Category);
        Assert.AreEqual(incidents[1].Latitude, second.Latitude);
        Assert.AreEqual(incidents[1].Longitude, second.Longitude);
        Assert.AreEqual(incidents[1].Status, second.Status);
        Assert.AreEqual(incidents[1].ImageUrl, second.ImageUrl);
        Assert.AreEqual(incidents[1].CreatedAt, second.CreatedAt);

        incidentServiceMock.Verify(s => s.GetIncidentsAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.AreEqual(cts.Token, capturedToken);
    }

    [TestMethod]
    public async Task Handle_ReturnsEmpty_WhenServiceReturnsNoIncidents()
    {
        // Arrange
        var incidentServiceMock = new Mock<IIncidentService>();
        incidentServiceMock
            .Setup(s => s.GetIncidentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new GetIncidentsRequestHandler(incidentServiceMock.Object);

        // Act
        var response = await handler.Handle(new GetIncidentsRequest(), CancellationToken.None);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(0, response.Items.Length);

        incidentServiceMock.Verify(s => s.GetIncidentsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
