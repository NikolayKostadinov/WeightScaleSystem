namespace WeightScale.MeasurementsClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Ninject;
    using WeightScale.Application.AppStart;
    using WeightScale.Application.Contracts;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.Domain.Abstract;
    using WeightScale.Utility.Helpers;
    using log4net;

    class MeasurementClientMain
    {
        private const string GetLogsWebApiController = @"api/Logs/GetLogs";
        private const string ClearLogsWebApiController = @"api/Logs/PostClearLogs";
        private const string IpAddressPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}";
        private const string ArchivedFilesDirectory = @"\logs\ws\";

        private static readonly IKernel injector;
        private static readonly IRepository<SoapMessage, CValidationMessage> repository;
        private static readonly IJsonDeserializeService deserializer;
        private static readonly IMappingService mapper;
        private static readonly ILog logger;

        private static volatile int lastLogProcessingHour;

        private static volatile bool stopProcessMeasurementThread;

        public static bool StopProcessMeasurementThread
        {
            get
            {
                return stopProcessMeasurementThread;
            }
            set
            {
                stopProcessMeasurementThread = value;
            }
        }

        static MeasurementClientMain()
        {
            AutomapperConfig.AutoMapperConfig();
            injector = NinjectInjector.GetInjector;
            repository = injector.Get<IRepository<SoapMessage, CValidationMessage>>();
            deserializer = injector.Get<IJsonDeserializeService>();
            mapper = injector.Get<IMappingService>();
            logger = LogManager.GetLogger("WeightScale.MeasurementsClient");
            lastLogProcessingHour = DateTime.Now.AddHours(-1).Hour;
        }
          
        static void Main()
        {
            // make these
            bool runAsWindowsService = true;
            if (runAsWindowsService)
            {
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] 
                { 
                    new MeasurementsWindowsService(logger) 
                };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                ProcessMeasurementsAsync();
                while (true);
            }
        }

        internal async static void ProcessMeasurementsAsync()
        {
            do
            {
                try
                {
                    DateTime beginTotal = DateTime.Now;
                    await repository.GetAllAsynk().ContinueWith(result => ProcessMeasurementRequestsAsync(result.Result, beginTotal));
                }
                catch (FaultException faultEx)
                {
                    logger.Error(faultEx.ToMessageAndCompleteStacktrace());
                }
                catch (Exception ex)
                {
                    logger.Error("From ProcessMeasurements " + ex.ToMessageAndCompleteStacktrace());
                }
            } while (!StopProcessMeasurementThread);
        }

        private async static void ProcessMeasurementRequestsAsync(IEnumerable<SoapMessage> result, DateTime beginTotal)
        {
            if (result != null)
            {
                foreach (var item in result)
                {
                    var message = GetWeightScaleMessageDto(item);
                    Task action;
                    string messageType = message.Message.GetType().Name;
                    try
                    {
                        SendMeasurementRequestAsync(messageType, message, item)
                            .ContinueWith(response => SendMeasurementResultToCacheAsync(response.Result, message, item));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format("[Id: {0}] {1}", item.Id, ex.Message));
                        SendErrorMessageToCache(item, message);
                    }
                    finally
                    {
                        LogEstimatedTimeForTransaction(beginTotal, item.Id);
                    }
                }
            }
        }

        private static void SendErrorMessageToCache(SoapMessage item, IWeightScaleMessageDto message)
        {
            try
            {
                message.ValidationMessages.AddError(string.Format("Cannot execute measurement on {0} ", item.URL));
                mapper.ToProxy(message, item);
                var validationMessages = repository.UpdateAsync(item).Result;
                if (validationMessages != null && validationMessages.Count() > 0)
                {
                    foreach (var vm in validationMessages)
                    {
                        logger.Error(string.Format("[Id: {0}] ValidationMessage: {1}", item.Id, vm.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("[Id: {0}] Message cannot be sent due to: {1} ", item.Id, ex.Message));
            }
        }

        /// <summary>
        /// Logs the estimated time for transaction.
        /// </summary>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="beginTotal">The begin total.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private async static void LogEstimatedTimeForTransaction(DateTime beginTotal, long id)
        {
            logger.Info(string.Format("[Id: {0}] Total estimated time for transaction: {1}", id, (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
        }

        private async static void SendMeasurementResultToCacheAsync(HttpResponseMessage response, IWeightScaleMessageDto message, SoapMessage item)
        {
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    await response.Content.ReadAsStringAsync().ContinueWith(jsonAnswer => UpdateMeasurementResultAsync(jsonAnswer.Result, message, item));
                }
                else
                {
                    logger.Error(string.Format("Error Code: {0} : Message: {1}", 
                        (response != null) ? response.StatusCode.ToString() : "null", 
                        (response != null) ? response.ReasonPhrase : "The response is null"));
                    SendErrorMessageToCache(item, message);
                }
            }
        }

        internal static void ProcessLogs()
        {
            try
            {
                var currentHour = DateTime.Now.Hour;

                if (currentHour != lastLogProcessingHour && DateTime.Now.Minute > Properties.Settings.Default.LogFilesCheckMinutesOffset)
                {
                    var urlsList = repository.GetTargetUrls();
                    if (urlsList != null)
                    {
                        var client = new HttpClient();
                        foreach (var url in urlsList)
                        {
                            try
                            {
                                var wsUrl = string.Format("{0}/{1}", url, GetLogsWebApiController);
                                using (var request = new HttpRequestMessage(HttpMethod.Get, wsUrl))
                                {
                                    logger.Debug(string.Format("------------- Processing Log files: {0} -------------", url));

                                    var fileName = CreateArchivedFilePath(url, currentHour);
                                    var content = client.SendAsync(request).Result.Content;
                                    if (!content.ReadAsStringAsync().Result.Contains("Files not found"))
                                    {
                                        var contentStreamLength = GetLogs(content, fileName);
                                        if (contentStreamLength > 0)
                                        {
                                            ClearLogs(fileName, url, client);
                                        }
                                    }
                                }
                            }
                            catch (Exception exx)
                            {
                                logger.Error(exx.ToMessageAndCompleteStacktrace());
                            }
                        }

                        lastLogProcessingHour = currentHour;
                    }
                }
            }
            catch (FaultException faultEx)
            {
                logger.Error(faultEx.ToMessageAndCompleteStacktrace());
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    foreach (var innerEx in (ex as AggregateException).InnerExceptions)
                    {
                        logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                    }
                }
                else
                {
                    logger.Error(ex.ToMessageAndCompleteStacktrace());
                }
            }
        }

        private static void UpdateMeasurementResult(string jsonAnswer, IWeightScaleMessageDto message, SoapMessage item)
        {
            IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
            SoapMessage currentSoap = item;
            mapper.ToProxy(incommmingMeasurementResult, currentSoap);
            var validationMessages = repository.Update(currentSoap);

            if (validationMessages != null && validationMessages.Count() > 0)
            {
                foreach (var vm in validationMessages)
                {
                    logger.Error(string.Format("[Id: {0}] ValidationMessage: {1}", item.Id, vm.ToString()));
                }
            }
        }

        private async static void UpdateMeasurementResultAsync(string jsonAnswer, IWeightScaleMessageDto message, SoapMessage item)
        {
            logger.Debug(string.Format("[Id: {0}] Received response message: {1}", item.Id, jsonAnswer));

            IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
            SoapMessage currentSoap = item;

            mapper.ToProxy(incommmingMeasurementResult, currentSoap);

            var validationMessages = await repository.UpdateAsync(currentSoap);

            if (validationMessages != null && validationMessages.Count() > 0)
            {
                foreach (var vm in validationMessages)
                {
                    logger.Error(string.Format("[Id: {0}] ValidationMessage: {1}", item.Id, vm.ToString()));
                }
            }
        }

        private static IWeightScaleMessageDto GetWeightScaleMessageDto(SoapMessage item)
        {
            var message = injector.Get<IWeightScaleMessageDto>();
            message.Id = item.Id;
            message.Message = item.Message.ToDomainType();
            message.ValidationMessages = injector.Get<IValidationMessageCollection>();
            return message;
        }

        private static HttpResponseMessage SendMeasurementRequest(string messageType, IWeightScaleMessageDto message, SoapMessage item)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
            logger.Debug(string.Format("[Id: {0}] ------------- Processing message Id: {0} -------------", message.Id));
            logger.Debug(string.Format("[Id: {0}] Sent request message: {1} - {2}", item.Id, item.URL, JsonConvert.SerializeObject(message)));

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsJsonAsync(item.URL, message).Result;
            }
            catch (HttpRequestException ex)
            {
                logger.Error(ex.Message, ex);
            }
            catch (WebException wex)
            {
                logger.Error(wex.Message, wex);
            }
            catch (AggregateException ae)
            {
                foreach (var innerEx in ae.InnerExceptions)
                {
                    logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                }

            }

            return response;
        }

        private async static Task<HttpResponseMessage> SendMeasurementRequestAsync(string messageType, IWeightScaleMessageDto message, SoapMessage item)
        {
            var vms = injector.Get<IValidationMessageCollection>();
            var ctSource = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            var cancelationToken = ctSource.Token;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
            logger.Debug(string.Format("[Id: {0}] ------------- Processing message Id: {0} -------------", message.Id));
            logger.Debug(string.Format("[Id: {0}] Sent request message: {1} - {2}", item.Id, item.URL, JsonConvert.SerializeObject(message)));
            

            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsJsonAsync(item.URL, message, cancelationToken);
            }
            catch (HttpRequestException ex)
            {
                string vMessage = string.Format("[Id: {0}] Requested URL {1} is unreachable. ", item.Id, item.URL);
                logger.Error(vMessage);
                vms.AddError("General Error", vMessage);
            }
            catch (WebException wex)
            {
                logger.Error(wex.Message, wex);
            }
            catch (AggregateException ae)
            {
                foreach (var innerEx in ae.InnerExceptions)
                {
                    logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                }
            }
            catch (TaskCanceledException ecex)
            {
                if (ecex.CancellationToken.IsCancellationRequested)
                {
                    logger.Error(string.Format("[Id: {0}] Measure operation with URL {1} was canceled due to timeout.", item.Id, item.URL));
                }
                else
                {
                    logger.Error(string.Format("[Id: {0}] {1}", item.Id, "ecex " + ecex.Message + ecex.StackTrace));
                }
            }
            catch (OperationCanceledException oce)
            {
                if (oce.CancellationToken.IsCancellationRequested)
                {
                    logger.Error(string.Format("[Id: {0}] Measure operation with URL {1} was canceled due to timeout.", item.Id, item.URL));
                }
                else
                {
                    logger.Error(string.Format("[Id: {0}] {1}", item.Id, "oce " + oce.Message + oce.StackTrace));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format("[Id: {0}] {1}", item.Id, "exc " + exc.Message + exc.StackTrace));
            }
            finally
            {
                ctSource.Dispose();
            }

            if (response == null && vms.Count() > 0)
            {
                message.ValidationMessages.AddMany(vms);
            }
            return response;
        }

        private static IEnumerable<SoapMessage> GetMeasurementsRequests()
        {
            IEnumerable<SoapMessage> result = null;
            try
            {
                result = repository.GetAll();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return result;
        }

        private static long GetLogs(HttpContent content, string fileName)
        {
            var contentStreamLength = 0L;

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (Stream contentStream = content.ReadAsStreamAsync().Result)
                {
                    contentStreamLength = contentStream.Length;
                    if (contentStreamLength > 0)
                    {
                        contentStream.CopyTo(stream);
                    }
                }
            }

            return contentStreamLength;
        }

        private static string CreateArchivedFilePath(string url, int currentHour)
        {
            Regex ip = new Regex(IpAddressPattern);
            MatchCollection match = ip.Matches(url);
            var uri = match[0].Value.Replace(':', '-');

            var folder = string.Format(@"{0}{1}{2}",
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                ArchivedFilesDirectory,
                uri);

            bool folderExists = Directory.Exists(folder);
            if (!folderExists)
            {
                Directory.CreateDirectory(folder);
                GrantDirectoryFullAccess(folder);
            }

            var fileName = string.Format("{0}/{1:yyyy-MM-dd-HH-mm}-{2}.zip", folder, DateTime.Now, uri);
            return fileName;
        }

        private static void ClearLogs(string fileName, string url, HttpClient client)
        {
            try
            {
                IList<string> removingFileList = new List<string>();

                using (ZipArchive archive = ZipFile.OpenRead(fileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        removingFileList.Add(entry.Name);
                        logger.Info(entry.Name);
                    }
                }

                if (removingFileList.Count > 0)
                {
                    HttpResponseMessage response = null;
                    try
                    {
                        string wsDelete = string.Format(@"{0}/{1}", url, ClearLogsWebApiController);
                        logger.Info(wsDelete);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        response = client.PostAsJsonAsync(wsDelete, removingFileList).Result;
                    }
                    catch (HttpRequestException ex)
                    {
                        logger.Error(ex.Message, ex);
                    }

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        logger.Debug(string.Format("Removed logs files: {0} - {1}", url, fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToMessageAndCompleteStacktrace());
            }
        }

        private static bool GrantDirectoryFullAccess(string fullPath)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        new SecurityIdentifier(
                            WellKnownSidType.WorldSid,
                            null),
                        FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToMessageAndCompleteStacktrace());
                return false;
            }
        }
    }
}
