namespace IAE.Microservice.Application.Common.File
{
    public interface IFileResult
    {
        byte[] Contents { get; }

        string ContentType { get; }

        string DownloadName { get; }
    }
}