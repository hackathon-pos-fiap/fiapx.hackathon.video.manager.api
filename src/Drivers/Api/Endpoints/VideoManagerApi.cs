using Adapters.Controllers.Interfaces;
using Adapters.Presenters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Api.Endpoints
{
    [ApiController]
    [ExcludeFromCodeCoverage]
    [Route("video")]
    public class VideoManagerApi : ControllerBase
    {
        private readonly IVideoController _videoController;

        public VideoManagerApi(IVideoController videoController)
        {
            _videoController = videoController;
        }

        [HttpPost]
        public async Task<ActionResult<VideoUploadResponse>> RequestUploadAsync(
            [FromBody] VideoUploadRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _videoController.RequestUploadAsync(request, cancellationToken);
            return Created(Request.Path, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoResponse>>> GetAllAsync(
            [FromQuery] VideoFilter filter,
            CancellationToken cancellationToken)
        {
            var response = await _videoController.GetAllAsync(filter, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<VideoResponse>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var response = await _videoController.GetByIdAsync(id, cancellationToken);
            return Ok(response);
        }

        [HttpGet("filename/{filename}", Name = "GetByFilename")]
        public async Task<ActionResult<VideoResponse>> GetByFilenameAsync(string filename, CancellationToken cancellationToken)
        {
            var response = await _videoController.GetByFilenameAsync(filename, cancellationToken);
            return Ok(response);
        }

        [HttpPatch("{id}/status", Name = "UpdateStatusAsync")]
        public async Task<ActionResult<VideoResponse>> UpdateStatusAsync(
            string id,
            [FromBody] VideoUpdateStatusRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _videoController.UpdateStatusAsync(id, request, cancellationToken);
            return Ok(response);
        }
    }
}
