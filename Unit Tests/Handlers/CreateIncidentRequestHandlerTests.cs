using System.Text;
using Incidents.Domain;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Minio;

namespace Unit_Tests.Handlers;

[TestClass]
public class CreateIncidentRequestHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenMediaFileIsNull_InvokesCreateIncident_WithNullImageUrl()
    {
        // Arrange
        var clientFactoryMock = new Mock<IMinioClientFactory>(MockBehavior.Strict);
        var fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
        var incidentServiceMock = new Mock<IIncidentService>();

        incidentServiceMock
            .Setup(x => x.CreateIncidentAsync(It.IsAny<IncidentDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncidentDomain d, CancellationToken _) => d.Id);

        var handler = new CreateIncidentRequestHandler(clientFactoryMock.Object, fileServiceMock.Object, incidentServiceMock.Object);

        var request = new CreateIncidentRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Test",
            Description = "Desc",
            Category = IncidentCategory.Other,
            Latitude = 1.23,
            Longitude = 4.56,
            MediaFile = null
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreNotEqual(Guid.Empty, result);
        incidentServiceMock.Verify(x => x.CreateIncidentAsync(It.Is<IncidentDomain>(d =>
            d.UserId == request.UserId &&
            d.Title == request.Title &&
            d.Description == request.Description &&
            d.Category == request.Category &&
            d.Latitude == request.Latitude &&
            d.Longitude == request.Longitude &&
            d.Status == IncidentStatus.Reported &&
            d.ImageUrl == null
        ), It.IsAny<CancellationToken>()), Times.Once);

        // Ensure file upload was not attempted and client factory not used
        fileServiceMock.VerifyNoOtherCalls();
        clientFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task Handle_WhenMediaFileProvided_UploadsFileAndPassesReturnedUrlToCreateIncident()
    {
        // Arrange
        var clientFactoryMock = new Mock<IMinioClientFactory>();
        var fileServiceMock = new Mock<IFileService>();
        var incidentServiceMock = new Mock<IIncidentService>();

        clientFactoryMock.Setup(x => x.CreateClient()).Returns(It.IsAny<MinioClient>());

        var returnedUrl = "https://example.com/uploads/photo.png";

        fileServiceMock
            .Setup(x => x.UploadFileAsync(It.IsAny<MinioClient>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedUrl);

        incidentServiceMock
            .Setup(x => x.CreateIncidentAsync(It.IsAny<IncidentDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncidentDomain d, CancellationToken _) => d.Id);

        var handler = new CreateIncidentRequestHandler(clientFactoryMock.Object, fileServiceMock.Object, incidentServiceMock.Object);

        var content = "file-content";
        var fileName = "photo.png";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns(fileName);
        formFileMock.Setup(f => f.ContentType).Returns("image/png");
        formFileMock.Setup(f => f.OpenReadStream()).Returns(ms);

        var request = new CreateIncidentRequest
        {
            UserId = Guid.NewGuid(),
            Title = "WithMedia",
            Description = "Has media",
            Category = IncidentCategory.Other,
            MediaFile = formFileMock.Object
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreNotEqual(Guid.Empty, result);

        fileServiceMock.Verify(x => x.UploadFileAsync(
            It.IsAny<MinioClient>(),
            It.IsAny<Stream>(),
            It.Is<string>(s => s.Contains("_" + fileName)),
            It.Is<string>(ct => ct == "image/png"),
            It.IsAny<CancellationToken>()), Times.Once);

        incidentServiceMock.Verify(x => x.CreateIncidentAsync(It.Is<IncidentDomain>(d =>
            d.UserId == request.UserId &&
            d.Title == request.Title &&
            d.Description == request.Description &&
            d.Category == request.Category &&
            d.ImageUrl == returnedUrl &&
            d.Status == IncidentStatus.Reported
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
