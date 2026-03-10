using Adapters.Gateways.MongoDbs.Entities;
using Adapters.Gateways.MongoDbs.Interfaces;
using Core.Entities.Enums;
using Core.Exceptions;
using Infrastructure.DataAccess.MongoAdapter;
using Infrastructure.DataAccess.MongoAdapter.Contexts.Interfaces;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Gateways.MongoDbs
{
    [ExcludeFromCodeCoverage]
    public class VideoMongoDbGateway(IMongoContext mongoContext)
        : MongoGatewayBase<VideoMongoDb>(mongoContext), IVideoMongoDbGateway
    {
        public async Task<IEnumerable<VideoMongoDb>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken)
        {
            var builder = Builders<VideoMongoDb>.Filter;
            var filters = new List<FilterDefinition<VideoMongoDb>>
            {
                builder.Eq(e => e.UserId, userId)
            };

            if (status is not null && status != VideoStatus.None)
            {
                filters.Add(builder.Eq(e => e.Status, status.Value));
            }

            var finalFilter = builder.And(filters);

            var query = _collection
                .Find(finalFilter)
                .Skip(skip)
                .Limit(limit);

            var cursor = await query.ToCursorAsync(cancellationToken);
            return cursor.ToEnumerable(cancellationToken: cancellationToken);
        }

        public async Task<VideoMongoDb> GetByIdAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var builder = Builders<VideoMongoDb>.Filter;
            var filters = new List<FilterDefinition<VideoMongoDb>>
            {
                builder.Eq(e => e.Id, id),
                builder.Eq(e => e.UserId, userId)
            };

            var finalFilter = builder.And(filters);

            var video = await _collection
                .Find(finalFilter)
                .FirstOrDefaultAsync(cancellationToken);

            return video;
        }

        public async Task<VideoMongoDb> InsertAsync(VideoMongoDb video, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(video, cancellationToken: cancellationToken);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new DuplicateItemException($"Video with name '{video.FileName}' already exists.");
            }

            return video;
        }

        public async Task<VideoMongoDb> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken)
        {
            var builder = Builders<VideoMongoDb>.Filter;
            var filters = new List<FilterDefinition<VideoMongoDb>>
            {
                builder.Eq(e => e.Id, id),
                builder.Eq(e => e.UserId, userId)
            };

            var finalFilter = builder.And(filters);

            var update = Builders<VideoMongoDb>.Update
                .Set(e => e.Status, status);

            var options = new FindOneAndUpdateOptions<VideoMongoDb>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _collection.FindOneAndUpdateAsync(finalFilter, update, options, cancellationToken);
        }
    }
}
