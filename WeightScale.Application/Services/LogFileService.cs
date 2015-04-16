namespace WeightScale.LogFileService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ninject;
    using WeightScale.Domain.Abstract;
    using log4net;

    public class LogFileService : WeightScale.Application.Contracts.IFileService
    {
        private readonly ILog logger;
        private readonly IKernel injector;
        public LogFileService(ILog loggerParam, IKernel injectorParam) 
        {
            this.logger = loggerParam;
            this.injector = injectorParam;
        }

        /// <summary>
        /// Archives the files in folder.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <param name="resultFileName">Name of the result file.</param>
        /// <returns>Collection of FileInfo for archived files</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public IEnumerable<FileInfo> ArchiveFilesInFolder(string targetPath, string resultFileName)
        {
            if (File.Exists(resultFileName))
            {
                if (!FileInUse(resultFileName))
                {
                    File.Delete(resultFileName);
                }
                else 
                {
                    throw new InvalidOperationException(
                        string.Format("The file \"{0}\" you are trying to modify is currently in use.", resultFileName));
                }
            }
            using (FileStream zipToOpen = new FileStream(resultFileName, FileMode.OpenOrCreate))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var filesList = GetFilesToArchive(targetPath);

                    if (filesList.Count() > 0)
                    {
                        foreach (var filePath in filesList)
                        {
                            archive.CreateEntryFromFile(filePath.FullName, filePath.Name);
                        } 
                    }

                    return filesList;
                }
            }
        }

        public IValidationMessageCollection ClearFiles(string targetPath, IEnumerable<string> files) 
        {
            var result = injector.Get<IValidationMessageCollection>();
            foreach (var fileName in files)
            {
                var fileFullName = targetPath + @"\" + fileName;
                if (File.Exists(fileFullName))
                {
                    if (!FileInUse(fileFullName))
                    {
                        try
                        {
                            File.Delete(fileFullName);
                            logger.Info(string.Format("File \"{0}\" was deleted successfully.", fileFullName));
                        }
                        catch (Exception ex)
                        {
                            result.AddError(fileFullName, ex.Message + ex.StackTrace);
                            logger.Error(string.Format("The file \"{0}\" couldn't be deleted due to {1}.\n{2}", fileFullName, ex.Message, ex.StackTrace));
                        }
                    }
                    else
                    {
                        result.AddError(fileFullName, string.Format("The file \"{0}\" couldn't be deleted because is in use.", fileFullName));
                        logger.Error(string.Format("The file \"{0}\" couldn't be deleted because is in use.", fileFullName));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the files to archive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetFilesToArchive(string path)
        {
            if (Directory.Exists(path))
            {
                List<FileInfo> files = new List<FileInfo>();

                var fileNames = Directory.GetFiles(path);
                foreach (var fileName in fileNames)
                {
                    if (!this.FileInUse(fileName))
                    {
                        files.Add(new FileInfo(fileName));   
                    }
                }

                return files;
            }
            else 
            {
                throw new DirectoryNotFoundException(string.Format("Directory \"{0}\" not found.", path));
            }
        }

        private bool FileInUse(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    if (fs.CanWrite)
                    {
                        var file = new FileInfo(path);
                        var targetTime = DateTime.Now;
                        var creationTime = file.CreationTime;

                        if (creationTime.Date == targetTime.Date  && creationTime.Hour == targetTime.Hour) 
                        {
                            return true;
                        }

                        return false;
                    }
                    else 
                    {
                        return true;
                    }
                }
                
            }
            catch (IOException ex)
            {
                return true;
            }
        }
    }
}
