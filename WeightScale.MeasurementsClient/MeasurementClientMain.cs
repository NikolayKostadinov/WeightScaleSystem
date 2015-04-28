﻿namespace WeightScale.MeasurementsClient
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
                ProcessMeasurements();
                while (true) ;
            }
        }

        internal async static void ProcessMeasurements()
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
                    logger.Error(ex.ToMessageAndCompleteStacktrace());
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
                    string messageType = message.Message.GetType().Name;
                    SendMeasurementRequestAsync(messageType, message, item).ContinueWith(response => SendMeasurementResultToCacheAsync(response.Result, message,item));
                }

                logger.Info(string.Format("Total estimated time for transaction: {0}", (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
            }
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
                    logger.Error(string.Format("Error Code: {0} : Message: {1}", response.StatusCode, response.ReasonPhrase));
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
                    logger.Error(string.Format("ValidationMessage: {0}", vm.ToString()));
                }
            }
        }

        private async static void UpdateMeasurementResultAsync(string jsonAnswer, IWeightScaleMessageDto message, SoapMessage item)
        {
            logger.Debug(string.Format("Received response message: {0}", jsonAnswer));

            IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
            SoapMessage currentSoap = item;

            mapper.ToProxy(incommmingMeasurementResult, currentSoap);

            var validationMessages = await repository.UpdateAsync(currentSoap);

            if (validationMessages != null && validationMessages.Count() > 0)
            {
                foreach (var vm in validationMessages)
                {
                    logger.Error(string.Format("ValidationMessage: {0}", vm.ToString()));
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
            logger.Debug(string.Format("------------- Processing message Id: {0} -------------", message.Id));
            logger.Debug(string.Format("Sended request message: {0} - {1}", item.URL, JsonConvert.SerializeObject(message)));

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

        private static Task<HttpResponseMessage> SendMeasurementRequestAsync(string messageType, IWeightScaleMessageDto message, SoapMessage item)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
            logger.Debug(string.Format("------------- Processing message Id: {0} -------------", message.Id));
            logger.Debug(string.Format("Sended request message: {0} - {1}", item.URL, JsonConvert.SerializeObject(message)));

            Task<HttpResponseMessage> response = null;
            try
            {
                response = client.PostAsJsonAsync(item.URL, message);
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
