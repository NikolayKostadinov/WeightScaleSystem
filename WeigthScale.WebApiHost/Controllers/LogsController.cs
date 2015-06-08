using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WeightScale.Application.Contracts;
using WeigthScale.WebApiHost.Infrastructure;
using log4net;
using log4net.Appender;

namespace WeigthScale.WebApiHost.Controllers
{
    public class LogsController : ApiController
    {
        private readonly ILog logger;
        private readonly IFileService fileManager;

        public LogsController(ILog loggerParam, IFileService archiverParam)
        {
            this.logger = loggerParam;
            this.fileManager = archiverParam;
        }

        [HttpGet]
        public HttpResponseMessage GetLogs()
        {
            try
            {
                string path = GetLogFilesLocation();
                string archiveFileName =Path.Combine(path, "ArchivedLogs.zip");                
                var archivedFiles = fileManager.ArchiveFilesInFolder(path, archiveFileName);
                if (archivedFiles.Count() > 0)
                {
                    LogArchivedFiles(archivedFiles);
                    return GenerateFileResponse(archiveFileName); 
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Files not found");
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ex);
            }
        }

        private HttpResponseMessage GenerateFileResponse(string archiveFileName)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(archiveFileName, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(archiveFileName);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = stream.Length;
            return result;
        }

        private void LogArchivedFiles(IEnumerable<FileInfo> archivedFiles)
        {
            logger.Info("------------------- Archived .log Files -------------------");
            foreach (var file in archivedFiles)
            {
                logger.Info(file.Name);
            }
            logger.Info("-----------------------------------------------------------");
        }

        [HttpPost]
        public HttpResponseMessage PostClearLogs(IEnumerable<string> files)
        {
            string path = GetLogFilesLocation();
            try
            {
                var result = this.fileManager.ClearFiles(path, files);
                if (result.Count() > 0)
                {
                    StringBuilder message = new StringBuilder();
                    foreach (var validationMessage in result)
                    {
                        message.Append(string.Format("File \"{0}\" could not be deleted.", validationMessage.Field));
                    }

                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, message.ToString());
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex.Message, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private string GetLogFilesLocation()
        {
            var fileAppender = this.logger.Logger.Repository.GetAppenders().FirstOrDefault() as FileAppender;
            if (fileAppender != null)
            {
                var pathName = Path.GetDirectoryName(fileAppender.File);
                return pathName;
            }
            else
            {
                throw new DirectoryNotFoundException("Current logging path not found");
            }
        }

    }
}
