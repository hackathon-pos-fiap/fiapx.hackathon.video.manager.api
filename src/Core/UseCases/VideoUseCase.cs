using Core.Entities;
using Core.Entities.Enums;
using Core.Gateways.Interfaces;
using Core.Providers.Interfaces;
using Core.UseCases.Interfaces;

namespace Core.UseCases
{
    public class VideoUseCase : IVideoUseCase
    {
        private readonly IVideoGateway _videoGateway;
        private readonly IBucketGateway _bucketGateway;
        private readonly IUserProvider _userProvider;

        public VideoUseCase(IVideoGateway videoGateway, IBucketGateway bucketGateway, IUserProvider userProvider)
        {
            _videoGateway = videoGateway;
            _bucketGateway = bucketGateway;
            _userProvider = userProvider;
        }

        public async Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, int skip, int limit, CancellationToken cancellationToken)
        {
            var videos = await _videoGateway.GetAllAsync(status, _userProvider.Id, skip, limit, cancellationToken);

            foreach (var video in videos)
            {
                SetDownloadUrlIfVideoIsCompleted(video, cancellationToken);
            }

            return videos;
        }

        public async Task<Video> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var video = await _videoGateway.GetByIdAsync(id, _userProvider.Id, cancellationToken);

            SetDownloadUrlIfVideoIsCompleted(video, cancellationToken);

            return video;
        }

        public async Task<Video> RequestUploadAsync(string fileName, CancellationToken cancellationToken)
        {
            var bucketUploadUrl = _bucketGateway.GenerateUploadUrl(fileName, cancellationToken);

            var video = new Video
            {
                UserId = _userProvider.Id,
                FileName = fileName,
                Status = VideoStatus.WaitingUpload,
                UploadUrl = bucketUploadUrl
            };

            return await _videoGateway.InsertAsync(video, cancellationToken);
        }

        public async Task<Video> UpdateStatusAsync(string id, VideoStatus status, CancellationToken cancellationToken)
        {
            var video = await _videoGateway.UpdateStatusAsync(id, _userProvider.Id, status, cancellationToken);
            
            SetDownloadUrlIfVideoIsCompleted(video, cancellationToken);

            return video;
        }

        private void SetDownloadUrlIfVideoIsCompleted(Video video, CancellationToken cancellationToken)
        {
            if (video.Status == VideoStatus.Completed)
            {
                video.DownloadUrl = _bucketGateway.GenerateDownloadUrl(video.FileName, cancellationToken);
            }
        }
    }
}
