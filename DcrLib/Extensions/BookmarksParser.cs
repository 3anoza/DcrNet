using DcrNet.Models.Liberty.Bookmarks;

namespace DcrNet.Extensions;

public class BookmarksParser
{
    /// <summary>
    ///     Extracts from raw IDMARK field bookmark type and true index
    /// </summary>
    /// <param name="rawBookmarkId">IDMARK value</param>
    /// <returns>Bookmark type and index</returns>
    public static (BookmarkType type, int index) GetBookmarkType(long rawBookmarkId)
    {
        var rawType = (uint)rawBookmarkId & Constants.Bookmarks.TypeMask;

        var type = Enum.IsDefined(typeof(BookmarkType), (int)rawType)
            ? (BookmarkType)rawType
            : BookmarkType.Unknown;

        var index = (int)(rawBookmarkId & Constants.Bookmarks.IndexMask);

        return (type, index);
    }
}