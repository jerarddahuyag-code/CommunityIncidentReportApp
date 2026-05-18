using Common.Mediator;
using Incidents.Domain;
using Incidents.RequestHandlers;
using Incidents.Requests;
using Incidents.Services;
using Moq;

namespace Unit_Tests.Handlers;

[TestClass]
public sealed class CreateCommentRequestHandlerTests
{
    [TestMethod]
    public async Task CreateComment_WithProperParameters_ShouldReturnCommentIdAsync()
    {
        // arrange
        var expectedIncidentId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedContent = "This is a comment";
        var comment = new CreateCommentRequest
        {
            IncidentId = expectedIncidentId,
            UserId = expectedUserId,
            Content = expectedContent
        };

        CommentDomain? capturedComment = null;
        var commentServiceStub = DefaultMocks.Create<ICommentService>();
        commentServiceStub.Setup(x => x.CreateCommentAsync(It.IsAny<CommentDomain>(), It.IsAny<CancellationToken>()))
            .Callback((CommentDomain c, CancellationToken _) => capturedComment = c)
            .ReturnsAsync((CommentDomain c, CancellationToken _) => c.Id);

        var sut = new CreateCommentRequestHandler(commentServiceStub.Object);
        // act
        var resultId = await sut.Handle(comment, CancellationToken.None);

        // assert
        commentServiceStub.Verify(x => x.CreateCommentAsync(It.IsAny<CommentDomain>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(capturedComment);
        Assert.AreNotEqual(Guid.Empty, resultId);
        Assert.AreEqual(expectedIncidentId, capturedComment.IncidentId);
        Assert.AreEqual(expectedUserId, capturedComment.UserId);
        Assert.AreEqual(expectedContent, capturedComment.Content);
    }
}
