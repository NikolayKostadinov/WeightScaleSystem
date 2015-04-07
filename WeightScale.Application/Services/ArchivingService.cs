namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class ArchivingService : WeightScale.Application.Services.IArchivingService
    {
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
                    foreach (var filePath in filesList)
                    {
                        archive.CreateEntryFromFile(filePath.FullName,filePath.Name);
                    }

                    return filesList;
                }
            }
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
