using DcrNet.Models.Liberty.Bookmarks;

namespace DcrNet.Models;

/// <summary>
///    Representation of Liberty file information
/// </summary>
public class LibertyFileDescriptor
{
    /// <summary>
    ///     Audio information
    /// </summary>
    public WaveFormat WaveFormat { get; set; } = null!;

    /// <summary>
    ///     Stored bookmarks information
    /// </summary>
    public List<BookmarkData> Bookmarks { get; set; } = new();
}