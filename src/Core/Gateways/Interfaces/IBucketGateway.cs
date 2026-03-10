namespace Core.Gateways.Interfaces
{
    public interface IBucketGateway
    {
        string GenerateUploadUrl(string fileName, CancellationToken cancellationToken);
        string GenerateDownloadUrl(string fileName, CancellationToken cancellationToken);
    }
}
