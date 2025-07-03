using DcrNet.Models;
using DcrNet.Providers;
using DcrNet.Services;

// ReSharper disable InconsistentNaming

namespace DcrNet.Files;

/// <summary>
///     One of the main types in DcrNet library. Represents .DCR liberty file
/// </summary>
public class LibertyFile
{
    private readonly LibertyAudioDataReader _audioDataReader;
    private readonly FileStream _fileStream;
    private readonly SpeexConverter _converter;
    private List<string>? _decodedPcmFiles;

    public LibertyFileDescriptor FileDescriptor;

    /// <summary>
    ///     Creates new instance of Liberty file
    /// </summary>
    /// <param name="filePath">path to liberty file</param>
    /// <exception cref="FileNotFoundException">throws exception if specified file not found</exception>
    public LibertyFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found. Path: {filePath}");

        _fileStream = File.OpenRead(filePath);

        var sectionsProvider = new XmlSectionProvider();
        var dcrFileDescriptorEjector = new LibertyFileDescriptorEjector(_fileStream, sectionsProvider);
        FileDescriptor = dcrFileDescriptorEjector.GetFileDescriptor();

        _audioDataReader = new LibertyAudioDataReader(_fileStream, FileDescriptor.WaveFormat);
        _converter = new SpeexConverter();
    }

    public List<string> GetPcmChannelFiles()
    {
        if (_decodedPcmFiles != null)
            return _decodedPcmFiles;

        var framesFile = _audioDataReader.ReadFramesToFile();
        var pcmFiles = _decodedPcmFiles =
            _converter.ExtractPcmAudioForEachChannel(framesFile, FileDescriptor.WaveFormat);

        if (File.Exists(framesFile))
            File.Delete(framesFile);

        Console.WriteLine("[DCR_LIB]: Cleaned up temp file with speex data");

        return pcmFiles;
    }


    ~LibertyFile()
    {
        _fileStream.Position = 0;
        _fileStream.Flush(false);
        _fileStream.Close();
    }
}