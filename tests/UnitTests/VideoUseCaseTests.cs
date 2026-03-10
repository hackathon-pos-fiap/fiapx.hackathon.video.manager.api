using Core.Entities;
using Core.Entities.Enums;
using Core.Gateways.Interfaces;
using Core.Providers.Interfaces;
using Core.UseCases;
using Core.UseCases.Interfaces;

namespace UnitTests
{
    public class VideoUseCaseTests
    {
        private class FakeVideoGateway : IVideoGateway
        {
            public string? LastUserIdForGetAll;
            public string? LastInsertedId;

            public Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken)
            {
                LastUserIdForGetAll = userId;

                var videos = new List<Video>
                {
                    new Video { Id = "1", UserId = userId, FileName = "completed.mp4", Status = VideoStatus.Completed },
                    new Video { Id = "2", UserId = userId, FileName = "waiting.mp4", Status = VideoStatus.WaitingUpload }
                };

                return Task.FromResult<IEnumerable<Video>>(videos);
            }

            public Task<Video> GetByIdAsync(string id, string userId, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Video { Id = id, UserId = userId, FileName = "file.mp4", Status = VideoStatus.Completed });
            }

            public Task<Video> InsertAsync(Video video, CancellationToken cancellationToken)
            {
                video.Id = "inserted-id";
                LastInsertedId = video.Id;
                return Task.FromResult(video);
            }

            public Task<Video> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Video { Id = id, UserId = userId, FileName = "file.mp4", Status = status });
            }
        }

        internal class FakeBucketGateway : IBucketGateway
        {
            public string GenerateDownloadUrl(string fileName, CancellationToken cancellationToken) => $"download://{fileName}";
            public string GenerateUploadUrl(string fileName, CancellationToken cancellationToken) => $"upload://{fileName}";
        }

        private class FakeUserProvider : IUserProvider
        {
            public string Id { get; set; } = "user-1";
        }

        [Fact]
        public async Task GetAllAsync_SetsDownloadUrlOnlyForCompletedVideos()
        {
            var videoGateway = new FakeVideoGateway();
            var bucket = new FakeBucketGateway();
            var user = new FakeUserProvider();

            IVideoUseCase useCase = new VideoUseCase(videoGateway, bucket, user);

            var videos = (await useCase.GetAllAsync(null, 0, 10, CancellationToken.None)).ToList();

            Assert.Equal(2, videos.Count);
            var completed = videos.Single(v => v.FileName == "completed.mp4");
            var waiting = videos.Single(v => v.FileName == "waiting.mp4");

            Assert.Equal(VideoStatus.Completed, completed.Status);
            Assert.Equal("download://completed.mp4", completed.DownloadUrl);
            Assert.Null(waiting.DownloadUrl);
        }

        [Fact]
        public async Task GetByIdAsync_SetsDownloadUrlWhenCompleted()
        {
            var videoGateway = new FakeVideoGateway();
            var bucket = new FakeBucketGateway();
            var user = new FakeUserProvider();

            IVideoUseCase useCase = new VideoUseCase(videoGateway, bucket, user);

            var video = await useCase.GetByIdAsync("id-1", CancellationToken.None);

            Assert.Equal("download://file.mp4", video.DownloadUrl);
        }

        [Fact]
        public async Task RequestUploadAsync_CallsBucketAndInsertsVideo()
        {
            var videoGateway = new FakeVideoGateway();
            var bucket = new FakeBucketGateway();
            var user = new FakeUserProvider();

            IVideoUseCase useCase = new VideoUseCase(videoGateway, bucket, user);

            var video = await useCase.RequestUploadAsync("newfile.mp4", CancellationToken.None);

            Assert.Equal("inserted-id", video.Id);
            Assert.Equal(user.Id, video.UserId);
            Assert.Equal(VideoStatus.WaitingUpload, video.Status);
            Assert.Equal("upload://newfile.mp4", video.UploadUrl);
        }

        [Fact]
        public async Task UpdateStatusAsync_SetsDownloadUrlWhenCompleted()
        {
            var videoGateway = new FakeVideoGateway();
            var bucket = new FakeBucketGateway();
            var user = new FakeUserProvider();

            IVideoUseCase useCase = new VideoUseCase(videoGateway, bucket, user);

            var video = await useCase.UpdateStatusAsync("id-2", VideoStatus.Completed, CancellationToken.None);

            Assert.Equal("download://file.mp4", video.DownloadUrl);
            Assert.Equal(VideoStatus.Completed, video.Status);
        }
    }
}
