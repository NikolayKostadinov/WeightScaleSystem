namespace WeightScale.Application.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WeightScale.Domain.Abstract;

    public interface IFileService
    {
        IEnumerable<FileInfo> ArchiveFilesInFolder(string targetPath, string resultFileName);
        IValidationMessageCollection ClearFiles(string targetPath, IEnumerable<string> files);
    }
}
