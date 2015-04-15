namespace WeightScale.MeasurementsClient
{
    using System;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;
    using Ninject;
    using WeightScale.Application.AppStart;
    using WeightScale.Application.Contracts;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.Domain.Abstract;
    using WeightScale.Utility.Helpers;
    using log4net;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.IO;

    class MeasurementClientMain
    {
        private const string GetLogsWebApiController = @"api/Logs/GetLogs";
        private const string ClearLogsWebApiController = @"api/Logs/PostClearLogs";
        private const string IpAddressPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";

        private static IKernel injector;
        private static IRepository<SoapMessage, CValidationMessage> repository;
        private static IJsonDeserializeService deserializer;
        private static IMappingService mapper;
        private static ILog logger;

        private static volatile int LastLogProcessingHour;

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
            LastLogProcessingHour = DateTime.Now.AddHours(-1).Hour;
        }

        static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new MeasurementsService(logger) 
            };
            ServiceBase.Run(servicesToRun);
            //do
            //{
            //    ProcessMeasurements();
            //    //ProcessLogs();
            //} while (true);
        }

        internal static void ProcessMeasurements()
        {
            do
            {
                try
                {
                    DateTime beginTotal = DateTime.Now;

                    IEnumerable<SoapMessage> result = null;
                    try
                    {
                        result = repository.GetAll();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message, ex);
                    }

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            var message = injector.Get<IWeightScaleMessageDto>();
                            message.Id = item.Id;
                            message.Message = item.Message.ToDomainType();
                            message.ValidationMessages = injector.Get<IValidationMessageCollection>();
                            string messageType = message.Message.GetType().Name;

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

                            if (response != null && response.IsSuccessStatusCode)
                            {
                                string jsonAnswer = response.Content.ReadAsStringAsync().Result;
                                logger.Debug(string.Format("Received response message: {0}", jsonAnswer));
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
                            else
                            {
                                logger.Error(string.Format("Error Code: {0} : Message: {1}", response.StatusCode, response.ReasonPhrase));
                            }
                        }
                        logger.Info(string.Format("Total estimated time for transaction: {0}", (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
                    }
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

        internal static void ProcessLogs()
        {
            try
            {
                var currentHour = DateTime.Now.Hour;

                if (currentHour != LastLogProcessingHour && DateTime.Now.Minute > Properties.Settings.Default.LogFilesCheckMinutesOffset)
                {
                    var urlsList = repository.GetTargetUrls();
                    if (urlsList != null)
                    {
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
                                    var contentAsString = content.ReadAsStringAsync().Result;
                                    if (contentAsString.Contains("Files not found"))
                                    {
                                        logger.Info(string.Format("Files not found : {0}", url));
                                    }
                                    else
                                    {
                                        var contentStreamLength = GetLogs(content, fileName);
                                        logger.Debug(string.Format("Getting logs files: {0} - {1} - {2}", url, fileName, contentStreamLength));
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

                        LastLogProcessingHour = currentHour;
                    }
                }
            }
            catch (FaultException faultEx)
            {
                logger.Error(faultEx.ToMessageAndCompleteStacktrace());
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToMessageAndCompleteStacktrace());
            }
        }
 
        private static long GetLogs(HttpContent content, string fileName)
        {
            var contentStreamLength = 0L;

            using (Stream contentStream = content.ReadAsStreamAsync().Result,
            stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                contentStreamLength = contentStream.Length;
                if (contentStreamLength > 0)
                {
                    contentStream.CopyTo(stream);
                }
            }
            return contentStreamLength;
        }
 
        private static string CreateArchivedFilePath(string url, int currentHour)
        {
            Regex ip = new Regex(IpAddressPattern);
            MatchCollection address = ip.Matches(url);

            var folder = string.Format(@"{0}\logs\ws\{1}", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), address[0]);
            bool folderExists = Directory.Exists(folder);
            if (!folderExists)
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = string.Format("{0}/{1:yyyy-MM-dd-HH-mm}-{2}.zip", folder, DateTime.Now, address[0]);
            return fileName;
        }
 
        private static void ClearLogs(string fileName, string url, HttpClient client)
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
    }
}
