namespace DcrNet.Providers;

public interface IXmlSectionProvider
{
    /// <summary>
    ///     Returns specified object from xml string
    /// </summary>
    /// <typeparam name="TResult">type of data object</typeparam>
    /// <param name="xmlContent">xml string</param>
    /// <returns>object with specified type</returns>
    TResult GetSectionData<TResult>(string xmlContent);
}