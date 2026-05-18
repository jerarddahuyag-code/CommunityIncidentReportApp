using Incidents.Domain;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Moq;

namespace Unit_Tests.Handlers;

[TestClass]
public class GetCommentsRequestHandlerTests
{
    [TestMethod]
    public async Task Handle_Should_Map_CommentDomain_To_ResponseItems()
    {
        // Arrange
        var incidentId = Guid.NewGuid();
        var comments = new[]
        {
            new CommentDomain
            {
                Id = Guid.NewGuid(),
                IncidentId = incidentId,
                UserId = Guid.NewGuid(),
                DisplayName = "Alice",
                Content = "First comment",
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            },
            new CommentDomain
            {
                Id = Guid.NewGuid(),
                IncidentId = incidentId,
                UserId = Guid.NewGuid(),
                DisplayName = "Bob",
                Content = "Second comment",
                CreatedAt = DateTime.UtcNow
            }
        };

        var commentServiceMock = new Mock<ICommentService>();
        Guid? capturedIncidentId = null;
        CancellationToken capturedToken = default;
        commentServiceMock
            .Setup(s => s.GetCommentsByIncidentId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback((Guid id, CancellationToken ct) =>
            {
                capturedIncidentId = id;
                capturedToken = ct;
            })
            .ReturnsAsync(comments);

        var handler = new GetCommentsRequestHandler(commentServiceMock.Object);
        var request = new GetCommentsRequest { IncidentId = incidentId };
        var cts = new CancellationTokenSource();

        // Act
        var response = await handler.Handle(request, cts.Token);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(comments.Length, response.Items.Length);

        // Ensure mapping is correct and preserves order
        for (var i = 0; i < comments.Length; i++)
        {
            var src = comments[i];
            var dst = response.Items[i];

            Assert.AreEqual(src.Id, dst.Id);
            Assert.AreEqual(src.IncidentId, dst.IncidentId);
            Assert.AreEqual(src.UserId, dst.UserId);
            Assert.AreEqual(src.DisplayName, dst.Username);
            Assert.AreEqual(src.Content, dst.Content);
            Assert.AreEqual(src.CreatedAt, dst.CreatedAt);
        }

        // Verify service invocation
        Assert.AreEqual(incidentId, capturedIncidentId);
        Assert.AreEqual(cts.Token, capturedToken);
        commentServiceMock.Verify(s => s.GetCommentsByIncidentId(incidentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_Should_Use_Unknown_When_DisplayName_Is_Null()
    {
        // Arrange
        var incidentId = Guid.NewGuid();
        var comment = new CommentDomain
        {
            Id = Guid.NewGuid(),
            IncidentId = incidentId,
            UserId = Guid.NewGuid(),
            DisplayName = null,
            Content = "No display name",
            CreatedAt = DateTime.UtcNow
        };

        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock
            .Setup(s => s.GetCommentsByIncidentId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([comment]);

        var handler = new GetCommentsRequestHandler(commentServiceMock.Object);
        var request = new GetCommentsRequest { IncidentId = incidentId };

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Items.Length);
        Assert.AreEqual("Unknown", response.Items[0].Username);

        commentServiceMock.Verify(s => s.GetCommentsByIncidentId(incidentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
