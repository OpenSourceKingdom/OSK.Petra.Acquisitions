using System;
using System.Linq;

namespace OSK.Petra.Acquisitions.Models;

public readonly struct HubSearchOptions
{
    #region Variables

    public readonly int[]? OwnerIds { get; init; }

    public readonly Guid[]? HubGroupIds { get; init; }

    #endregion

    #region Api

    public HubSearchOptions WithOwnerId(int ownerId)
        => new()
        {
            OwnerIds = OwnerIds is null ? [ownerId] : [.. OwnerIds.Append(ownerId).Distinct()],
            HubGroupIds = HubGroupIds
        };

    public HubSearchOptions WithGroupId(Guid hubGroupId)
        => new()
        {
            OwnerIds = OwnerIds,
            HubGroupIds = HubGroupIds is null ? [hubGroupId] : [.. HubGroupIds.Append(hubGroupId).Distinct()],
        };

    #endregion
}
