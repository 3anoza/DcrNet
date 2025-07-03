using System.Text;

namespace DcrNet;

public class Constants
{
    /// <summary>
    ///    Size of each section tag in bytes. For example: ...meta#bgn...
    /// </summary>
    public const byte FILE_SECTION_SIZE = 8;
    /// <summary>
    ///     Header for each HighCriteria file with .DCR extension. May be different for legacy files with version lower than 840
    /// </summary>
    public const string FILE_HEADER = "HGCRLCRS";

    /// <summary>
    ///     Metadata section in DCR file
    /// </summary>
    public static Section Meta = new()
        { Begin = Encoding.UTF8.GetBytes("meta#bgn"), End = Encoding.UTF8.GetBytes("meta#end") };

    /// <summary>
    ///     Data section in DCR file
    /// </summary>
    public static Section Data = new()
        { Begin = Encoding.UTF8.GetBytes("data#bgn"), End = Encoding.UTF8.GetBytes("data#end") };

    /// <summary>
    ///     Attributes section in DCR file
    /// </summary>
    public static Section Attx = new()
        { Begin = Encoding.UTF8.GetBytes("attx#bgn"), End = Encoding.UTF8.GetBytes("attx#end") };

    /// <summary>
    ///     Video frame section in DCR file
    /// </summary>
    public static Section Cadr = new()
        { Begin = Encoding.UTF8.GetBytes("cadr#bgn"), End = Encoding.UTF8.GetBytes("cadr#end") };

    /// <summary>
    ///     Audio pitch section in DCR file
    /// </summary>
    public static Section Pitch = new()
        { Begin = Encoding.UTF8.GetBytes("ptch#str"), End = Encoding.UTF8.GetBytes("ptch#stp") };

    /// <summary>
    ///     Audio bookmark section in DCR file
    /// </summary>
    public static Section AudiMrk = new() { Begin = Encoding.UTF8.GetBytes("audi#mrk") };

    /// <summary>
    ///     Audio frames pointer section in DCR file
    /// </summary>
    public static Section AudiPtr = new() { Begin = Encoding.UTF8.GetBytes("audi#ptr") };

    /// <summary>
    ///     Waveformat section in DCR file. Deprecated, supported only for files with equal to/below 840 version
    /// </summary>
    public static Section Wfmt = new()
        { Begin = Encoding.UTF8.GetBytes("wfmt#bgn"), End = Encoding.UTF8.GetBytes("wfmt#end") };

    /// <summary>
    ///     Cryptography section in DCR file
    /// </summary>
    public static Section Crypt = new()
        { Begin = Encoding.UTF8.GetBytes("crpt#bgn"), End = Encoding.UTF8.GetBytes("crpt#end") };

    /// <summary>
    ///     Channel section in DCR file
    /// </summary>
    public static Section Channel = new()
        { Begin = Encoding.UTF8.GetBytes("chnl#bgn"), End = Encoding.UTF8.GetBytes("chnl#end") };

    /// <summary>
    ///     Bookmarks section in DCR file
    /// </summary>
    public static Section Mark = new()
        { Begin = Encoding.UTF8.GetBytes("mark#bgn"), End = Encoding.UTF8.GetBytes("mark#end") };

    /// <summary>
    ///     Unknown section.
    /// </summary>
    public static Section Bist = new()
        { Begin = Encoding.UTF8.GetBytes("bist#bgn"), End = Encoding.UTF8.GetBytes("bist#end") };

    /// <summary>
    ///     Unknown section
    /// </summary>
    public static Section Tabl = new()
        { Begin = Encoding.UTF8.GetBytes("tabl#bgn"), End = Encoding.UTF8.GetBytes("tabl#end") };

    /// <summary>
    ///     Recordings info section in DCR file
    /// </summary>
    public static Section Rinf = new()
        { Begin = Encoding.UTF8.GetBytes("rinf#bgn"), End = Encoding.UTF8.GetBytes("rinf#end") };

    /// <summary>
    ///     Info section in DCR file
    /// </summary>
    public static Section Info = new()
        { Begin = Encoding.UTF8.GetBytes("info#bgn"), End = Encoding.UTF8.GetBytes("info#end") };

    /// <summary>
    ///     Head section in DCR file
    /// </summary>
    public static Section Head = new() { Begin = Encoding.UTF8.GetBytes("head") };

    /// <summary>
    ///     Represents collection of masks for bookmark type and index parsing
    /// </summary>
    public class Bookmarks
    {
        public const uint TypeMask = 0xF0000000;
        public const uint IndexMask = 0x0FFFFFFF;
    }
}

public class Section
{
    public byte[] Begin { get; set; }
    public byte[] End { get; set; }
}