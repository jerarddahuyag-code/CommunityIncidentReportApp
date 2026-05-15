using Common.Mediator;
using Incidents.Domain;
using Incidents.Requests;
using Incidents.Services;
using Minio;

namespace Incidents.RequestHandlers;
public class CreateIncidentRequestHandler(IMinioClientFactory clientFactory, IFileService fileService, IIncidentService incidentService) : IRequestHandler<CreateIncidentRequest, Guid>
{
    public async Task<Guid> Handle(CreateIncidentRequest request, CancellationToken cancellationToken)
    {
        string? fileUrl = null;
        if (request.MediaFile is not null) {
            var client = clientFactory.CreateClient();
            var fileName = Guid.NewGuid().ToString() + "_" + request.MediaFile.FileName;
            fileUrl = await fileService.UploadFileAsync(client, request.MediaFile.OpenReadStream(), fileName, request.MediaFile.ContentType, cancellationToken);
        }

        var id = await incidentService.CreateIncidentAsync(new IncidentDomain
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = IncidentStatus.Reported,
            ImageUrl = fileUrl,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);

        return id;
    }
}
