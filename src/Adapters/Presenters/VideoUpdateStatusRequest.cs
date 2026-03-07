using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoUpdateStatusRequest(
        VideoStatus Status);
}
