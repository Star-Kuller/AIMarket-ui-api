namespace IAE.Microservice.Application.Common.File
{
    public abstract class FileQuery
    {
        public FileType Type { get; }

        protected FileQuery(FileType type)
        {
            Type = type;
        }
    }
}