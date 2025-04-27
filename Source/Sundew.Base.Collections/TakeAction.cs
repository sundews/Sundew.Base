namespace Sundew.Base.Collections;

/// <summary>
/// Enum describing taking or skip items in a list.
/// </summary>
[DiscriminatedUnions.DiscriminatedUnion]
public enum TakeAction
{
    /// <summary>Take the item.</summary>
    Take,

    /// <summary>Skip the item.</summary>
    Skip,

    /// <summary>Ends taking items.</summary>
    End,

    /// <summary>Takes the item and ends.</summary>
    TakeAndEnd,
}