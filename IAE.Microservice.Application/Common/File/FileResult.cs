using System.Collections.Generic;

namespace IAE.Microservice.Application.Common.File
{
    public abstract class FileResult : FileQuery, IFileResult
    {
        private new FileType Type { get; }

        private string Name { get; }

        public byte[] Contents { get; }

        public string ContentType => Type.GetContentType();

        public string DownloadName => Name + Type.GetExtension();

        protected FileResult(FileType type, string name, byte[] contents) : base(type)
        {
            Type = type;
            Name = name;
            Contents = contents;
        }
    }
}