using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adapters.Gateways.MongoDbs;
using Adapters.Gateways.MongoDbs.Converters;
using Adapters.Gateways.MongoDbs.Entities;
using Adapters.Gateways.MongoDbs.Interfaces;
using Core.Entities;
using Core.Entities.Enums;
using Infrastructure.DataAccess.MongoAdapter.Contexts.Interfaces;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace UnitTests
{
    public class VideoGatewayConverterTests
    {
        private class FakeVideoMongoGateway : IVideoMongoDbGateway
        {
            public Task<IEnumerable<VideoMongoDb>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken)
            {
                var list = new List<VideoMongoDb>
                {
                    new VideoMongoDb(new Video { Id = "1", UserId = userId, FileName = "a.mp4", Status = VideoStatus.Finished }),
                    new VideoMongoDb(new Video { Id = "2", UserId = userId, FileName = "b.mp4", Status = VideoStatus.Processing })
                };

                // ensure ids are set
                list[0].Id = "1";
                list[1].Id = "2";

                return Task.FromResult<IEnumerable<VideoMongoDb>>(list);
            }

            public Task<VideoMongoDb> GetByFilenameAsync(string filename, string userId, CancellationToken cancellationToken)
            {
                var v = new VideoMongoDb(new Video { Id = Guid.NewGuid().ToString(), UserId = userId, FileName = filename, Status = VideoStatus.Finished });
                v.FileName = filename;
                return Task.FromResult(v);
            }

            public Task<VideoMongoDb> GetByIdAsync(string id, string userId, CancellationToken cancellationToken)
            {
                var v = new VideoMongoDb(new Video { Id = id, UserId = userId, FileName = "f.mp4", Status = VideoStatus.Finished });
                v.Id = id;
                return Task.FromResult(v);
            }

            public Task<VideoMongoDb> InsertAsync(VideoMongoDb video, CancellationToken cancellationToken)
            {
                video.Id = "inserted";
                return Task.FromResult(video);
            }

            public Task<VideoMongoDb> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken)
            {
                var v = new VideoMongoDb(new Video { Id = id, UserId = userId, FileName = "f.mp4", Status = status });
                v.Id = id;
                return Task.FromResult(v);
            }
        }

        [Fact]
        public async Task Converter_GetAllAsync_ConvertsToCore()
        {
            var fake = new FakeVideoMongoGateway();
            var converter = new VideoGatewayConverter(fake);

            var res = (await converter.GetAllAsync(null, "user1", 0, 10, CancellationToken.None)).ToList();

            Assert.Equal(2, res.Count);
            Assert.Contains(res, v => v.Id == "1" && v.FileName == "a.mp4");
        }

        [Fact]
        public async Task Converter_GetByIdAsync_ConvertsToCore()
        {
            var fake = new FakeVideoMongoGateway();
            var converter = new VideoGatewayConverter(fake);

            var res = await converter.GetByIdAsync("id1", "user1", CancellationToken.None);

            Assert.Equal("id1", res.Id);
            Assert.Equal("f.mp4", res.FileName);
        }

        [Fact]
        public async Task Converter_InsertAsync_ConvertsToCore()
        {
            var fake = new FakeVideoMongoGateway();
            var converter = new VideoGatewayConverter(fake);

            var core = new Video { UserId = "u", FileName = "x.mp4", Status = VideoStatus.WaitingUpload };

            var res = await converter.InsertAsync(core, CancellationToken.None);

            Assert.Equal("inserted", res.Id);
            Assert.Equal("x.mp4", res.FileName);
        }

        [Fact]
        public async Task Converter_UpdateStatusAsync_ConvertsToCore()
        {
            var fake = new FakeVideoMongoGateway();
            var converter = new VideoGatewayConverter(fake);

            var res = await converter.UpdateStatusAsync("id-u", "u", VideoStatus.Finished, CancellationToken.None);

            Assert.Equal("id-u", res.Id);
            Assert.Equal(VideoStatus.Finished, res.Status);
        }
    }

    public class VideoMongoDbGatewayTests
    {
        [Fact]
        public async Task MongoGateway_InsertAsync_InsertsAndReturns()
        {
            var mockCollection = new Mock<IMongoCollection<VideoMongoDb>>();
            mockCollection.Setup(c => c.InsertOneAsync(It.IsAny<VideoMongoDb>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>())).Callback<VideoMongoDb, InsertOneOptions, CancellationToken>((doc, opt, ct) => doc.Id = "generated").Returns(Task.CompletedTask);

            var mockContext = new Mock<IMongoContext>();
            mockContext.Setup(ctx => ctx.GetCollection<VideoMongoDb>()).Returns(mockCollection.Object);

            var gateway = new VideoMongoDbGateway(mockContext.Object);

            var toInsert = new VideoMongoDb(new Video { UserId = "u1", FileName = "n.mp4", Status = VideoStatus.WaitingUpload });

            var res = await gateway.InsertAsync(toInsert, CancellationToken.None);

            Assert.NotNull(res.Id);
            Assert.Equal("n.mp4", res.FileName);
        }

        [Fact]
        public async Task MongoGateway_UpdateStatusAsync_ReturnsUpdated()
        {
            var initial = new VideoMongoDb(new Video { Id = "1", UserId = "u1", FileName = "a.mp4", Status = VideoStatus.Processing }) { Id = "1" };

            var mockCollection = new Mock<IMongoCollection<VideoMongoDb>>();
            mockCollection.Setup(c => c.FindOneAndUpdateAsync(It.IsAny<FilterDefinition<VideoMongoDb>>(), It.IsAny<UpdateDefinition<VideoMongoDb>>(), It.IsAny<FindOneAndUpdateOptions<VideoMongoDb>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    initial.Status = VideoStatus.Finished;
                    return initial;
                });

            var mockContext = new Mock<IMongoContext>();
            mockContext.Setup(ctx => ctx.GetCollection<VideoMongoDb>()).Returns(mockCollection.Object);

            var gateway = new VideoMongoDbGateway(mockContext.Object);

            var res = await gateway.UpdateStatusAsync("1", "u1", VideoStatus.Finished, CancellationToken.None);

            Assert.NotNull(res);
            Assert.Equal(VideoStatus.Finished, res.Status);
        }
    }
}
