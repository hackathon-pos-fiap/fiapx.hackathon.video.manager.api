using Amazon.S3;
using Amazon.S3.Model;
using Core.Gateways.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Gateways.BucketS3
{
    [ExcludeFromCodeCoverage]
    public class BucketS3Gateway : IBucketGateway
    {
        private const string BUCKET_NAME = "fiapx-video-worker-lgrando";
        private readonly AmazonS3Client _s3Client;

        public BucketS3Gateway()
        {
            var bucketAccessKeyId = Environment.GetEnvironmentVariable("BUCKET_ACCESS_KEY_ID");
            var bucketSecretKey = Environment.GetEnvironmentVariable("BUCKET_SECRET_KEY");

            _s3Client = new AmazonS3Client(bucketAccessKeyId, bucketSecretKey, Amazon.RegionEndpoint.USEast1);
        }

        public string GenerateDownloadUrl(string fileName, CancellationToken cancellationToken)
        {
            var downloadRequest = new GetPreSignedUrlRequest
            {
                BucketName = BUCKET_NAME,
                Key = fileName,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            return _s3Client.GetPreSignedURL(downloadRequest);
        }

        public string GenerateUploadUrl(string fileName, CancellationToken cancellationToken)
        {
            var uploadRequest = new GetPreSignedUrlRequest
            {
                BucketName = BUCKET_NAME,
                Key = fileName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = "video/mp4"
            };

            return _s3Client.GetPreSignedURL(uploadRequest);
        }
    }
}
