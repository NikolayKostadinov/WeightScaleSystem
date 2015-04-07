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
using WeightScale.Application.Services;
using log4net;
using log4net.Appender;

namespace WeigthScale.WebApiHost.Controllers
{
    public class LogsController : ApiController
    {
        private readonly ILog logger;
        private readonly IArchivingService archiver;

        public LogsController(ILog loggerParam, IArchivingService archiverParam)
        {
            this.logger = loggerParam;
            this.archiver = archiverParam;
        }

        [HttpGet]
        public HttpResponseMessage GetLogs()
        {
            try
            {
                string archiveFileName = @"ArchivedLogs.zip";
                string path = GetLogFilesLocation();
                var archivedFiles = archiver.ArchiveFilesInFolder(path, archiveFileName);
                LogArchivedFiles(archivedFiles);
                return GenerateFileResponse(archiveFileName);  
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
        public IHttpActionResult PostClearLogs(IEnumerable<string> files)
        {
            throw new NotImplementedException();
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
