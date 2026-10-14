namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Specifies the type of path a given acquisition path is
/// </summary>
public enum AcquisitionPathType
{
    /// <summary>
    /// The path is a single path, containing multiple choices, but there are no branches. e.g. armor level 1, 2, 3
    /// </summary>
    Single,

    /// <summary>
    /// The path is a copmlex path, containing multiple choices that potentially branch into many other paths. e.g. tech tree
    /// </summary>
    Multi
}
