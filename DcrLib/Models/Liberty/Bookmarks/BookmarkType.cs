namespace DcrNet.Models.Liberty.Bookmarks;

public enum BookmarkType
{
    UserBookmark = 0x00000000,
    StartRecording = 0x10000000,
    ResumeRecording = 0x20000000,
    StopRecording = 0x30000000,
    PauseRecording = 0x40000000,
    DocketBookmark = 0x50000000,
    Unknown = 0xF000000
}