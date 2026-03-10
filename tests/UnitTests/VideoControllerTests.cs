using Adapters.Controllers;
using Adapters.Presenters;
using Core.Entities;
using Core.Entities.Enums;
using Core.UseCases.Interfaces;

namespace UnitTests
{
    public class VideoControllerTests
    {
        private class FakeUseCase : IVideoUseCase
        {
            public Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, int skip, int limit, CancellationToken cancellationToken)
            {
                var videos = new List<Video>
                {
                    new Video { Id = "1", UserId = "u1", FileName = "a.mp4", Status = VideoStatus.Completed, DownloadUrl = "d1" },
                    new Video { Id = "2", UserId = "u1", FileName = "b.mp4", Status = VideoStatus.WaitingUpload }
                };
                return Task.FromResult<IEnumerable<Video>>(videos);
            }

            public Task<Video> GetByIdAsync(string id, CancellationToken cancellationToken)
            {
                if (id == "notfound") return Task.FromResult<Video?>(null!);
                return Task.FromResult(new Video { Id = id, UserId = "u1", FileName = "file.mp4", Status = VideoStatus.Completed, DownloadUrl = "d" });
            }

            public Task<Video> RequestUploadAsync(string fileName, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Video { Id = "ins", FileName = fileName, UploadUrl = "up" });
            }

            public Task<Video> UpdateStatusAsync(string id, VideoStatus status, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Video { Id = id, UserId = "u1", FileName = "file.mp4", Status = status, DownloadUrl = status == VideoStatus.Completed ? "d" : null });
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedResponses()
        {
            var controller = new VideoController(new FakeUseCase());

            var responses = (await controller.GetAllAsync(new VideoFilter(null, 0, 10), CancellationToken.None)).ToList();

            Assert.Equal(2, responses.Count);
            Assert.Contains(responses, r => r.FileName == "a.mp4" && r.DownloadUrl == "d1");
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsOnEmptyId()
        {
            var controller = new VideoController(new FakeUseCase());

            await Assert.ThrowsAsync<System.ArgumentNullException>(() => controller.GetByIdAsync("", CancellationToken.None));
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsWhenNotFound()
        {
            var controller = new VideoController(new FakeUseCase());

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(() => controller.GetByIdAsync("notfound", CancellationToken.None));
        }

        [Fact]
        public async Task RequestUploadAsync_ReturnsUploadResponse()
        {
            var controller = new VideoController(new FakeUseCase());

            var resp = await controller.RequestUploadAsync(new VideoUploadRequest("file.mp4"), CancellationToken.None);

            Assert.Equal("ins", resp.VideoId);
            Assert.Equal("up", resp.UploadUrl);
        }

        [Fact]
        public async Task UpdateStatusAsync_ValidatesInputs()
        {
            var controller = new VideoController(new FakeUseCase());

            await Assert.ThrowsAsync<System.ArgumentNullException>(() => controller.UpdateStatusAsync("", new VideoUpdateStatusRequest(VideoStatus.Completed), CancellationToken.None));

            var nullResult = await controller.UpdateStatusAsync("1", new VideoUpdateStatusRequest(VideoStatus.None), CancellationToken.None);
            Assert.Null(nullResult);
        }
    }
}
