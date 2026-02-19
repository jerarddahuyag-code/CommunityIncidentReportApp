using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incidents.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IMinioClient minioClient, Stream fileContentStream, string fileName, CancellationToken cancellationToken);
}

public class FileService : IFileService
{
    public async Task<string> UploadFileAsync(IMinioClient minioClient, Stream fileContentStream, string fileName, CancellationToken cancellationToken)
    {
        var args = new PutObjectArgs()
            .WithBucket("incidents")
            .WithObject(fileName)   
            .WithStreamData(fileContentStream)
            .WithObjectSize(fileContentStream.Length)
            .WithContentType("application/octet-stream");

        await minioClient.PutObjectAsync(args, cancellationToken).ConfigureAwait(false);

        return fileName;
    }
}
