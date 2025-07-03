namespace DcrNet.Models.Liberty.Bookmarks;

public class BookmarkData
{
    public long Id { get; set; }
    public BookmarkType Type { get; set; }
    public long FilePosition { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime CreatedAtLocal { get; set; }
    public string? NoteText { get; set; }
}