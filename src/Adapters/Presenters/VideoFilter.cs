using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]

    public record VideoFilter(
        VideoStatus? Status,
        int Skip,
        int Limit);
}
