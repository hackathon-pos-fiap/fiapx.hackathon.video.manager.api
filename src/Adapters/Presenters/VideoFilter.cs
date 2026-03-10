using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]

    public record VideoFilter(
        VideoStatus? Status,
        int Skip = 0,
        int Limit = 10);
}
