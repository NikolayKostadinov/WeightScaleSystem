namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IArchivingService
    {
        IEnumerable<FileInfo> ArchiveFilesInFolder(string targetPath, string resultFileName);
    }
}
