//  ------------------------------------------------------------------------------------------------
//   <copyright file="MeasurementClientMain.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.MeasurementsClient
{
    #region Using

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

    using log4net;

    using Newtonsoft.Json;

    using Ninject;

    using WeightScale.Application.AppStart;
    using WeightScale.Application.Contracts;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.Domain.Abstract;
    using WeightScale.MeasurementsClient.Properties;
    using WeightScale.Utility.Helpers;

    #endregion

    internal class MeasurementClientMain
    {
        private const string ArchivedFilesDirectory = @"\logs\ws\";

        private const string ClearLogsWebApiController = @"api/Logs/PostClearLogs";

        private const string GetLogsWebApiController = @"api/Logs/GetLogs";

        private const string IpAddressPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}";

        private static readonly IJsonDeserializeService Deserializer;

        private static readonly IKernel Injector;

        private static readonly ILog Logger;

        private static readonly IMappingService Mapper;

        private static readonly IRepository<SoapMessage, CValidationMessage> Repository;

        private static volatile int lastLogProcessingHour;

        private static volatile bool stopProcessMeasurementThread;

        static MeasurementClientMain()
        {
            AutomapperConfig.AutoMapperConfig();
            Injector = NinjectInjector.GetInjector;
            Injector.Bind<IConnectionParameters>().To<ConnectionParameters>();
            Repository = Injector.Get<IRepository<SoapMessage, CValidationMessage>>();
            Deserializer = Injector.Get<IJsonDeserializeService>();
            Mapper = Injector.Get<IMappingService>();
            Logger = LogManager.GetLogger("WeightScale.MeasurementsClient");
            lastLogProcessingHour = DateTime.Now.AddHours(-1).Hour;
        }

        public static bool StopProcessMeasurementThread
        {
            get => stopProcessMeasurementThread;

            set => stopProcessMeasurementThread = value;
        }

        internal static void ProcessLogs()
        {
            try
            {
                var currentHour = DateTime.Now.Hour;

                if (currentHour != lastLogProcessingHour
                    && DateTime.Now.Minute > Settings.Default.LogFilesCheckMinutesOffset)
                {
                    var urlsList = Repository.GetTargetUrls();
                    if (urlsList != null)
                    {
                        var client = new HttpClient();
                        foreach (var url in urlsList)
                        {
                            try
                            {
                                var getLogsUrl = $"{url}/{GetLogsWebApiController}";
                                using (var request = new HttpRequestMessage(HttpMethod.Get, getLogsUrl))
                                {
                                    Logger.Debug($"------------- Processing Log files: {url} -------------");

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
                                Logger.Error(exx.ToMessageAndCompleteStacktrace());
                            }
                        }

                        lastLogProcessingHour = currentHour;
                    }
                }
            }
            catch (FaultException faultEx)
            {
                Logger.Error(faultEx.ToMessageAndCompleteStacktrace());
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    foreach (var innerEx in (ex as AggregateException).InnerExceptions)
                    {
                        Logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                    }
                }
                else
                {
                    Logger.Error(ex.ToMessageAndCompleteStacktrace());
                }
            }
        }

        internal static async void ProcessMeasurementsAsync()
        {
            do
            {
                try
                {
                    var beginTotal = DateTime.Now;
                    var requests = await Repository.GetAllAsync();
                    ProcessMeasurementRequests(requests, beginTotal);
                }
                catch (FaultException faultEx)
                {
                    Logger.Error(faultEx.ToMessageAndCompleteStacktrace());
                }
                catch (Exception ex)
                {
                    Logger.Error("From ProcessMeasurements " + ex.ToMessageAndCompleteStacktrace());
                }
            }
            while (!StopProcessMeasurementThread);
        }

        private static void ClearLogs(string fileName, string url, HttpClient client)
        {
            try
            {
                IList<string> removingFileList = new List<string>();

                using (var archive = ZipFile.OpenRead(fileName))
                {
                    foreach (var entry in archive.Entries)
                    {
                        removingFileList.Add(entry.Name);
                        Logger.Info(entry.Name);
                    }
                }

                if (removingFileList.Count > 0)
                {
                    HttpResponseMessage response = null;
                    try
                    {
                        var deleteUri = $@"{url}/{ClearLogsWebApiController}";
                        Logger.Info(deleteUri);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        response = client.PostAsJsonAsync(deleteUri, removingFileList).Result;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Error(ex.Message, ex);
                    }

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        Logger.Debug($"Removed logs files: {url} {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToMessageAndCompleteStacktrace());
            }
        }

        private static string CreateArchivedFilePath(string url, int currentHour)
        {
            var ip = new Regex(IpAddressPattern);
            var match = ip.Matches(url);
            var uri = match[0].Value.Replace(':', '-');

            var folder = $@"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}{ArchivedFilesDirectory}{uri}";

            var folderExists = Directory.Exists(folder);
            if (!folderExists)
            {
                Directory.CreateDirectory(folder);
                GrantDirectoryFullAccess(folder);
            }

            var fileName = $"{folder}/{DateTime.Now:yyyy-MM-dd-HH-mm}-{uri}.zip";
            return fileName;
        }

        private static long GetLogs(HttpContent content, string fileName)
        {
            var contentStreamLength = 0L;

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var contentStream = content.ReadAsStreamAsync().Result)
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

        private static IEnumerable<SoapMessage> GetMeasurementsRequests()
        {
            IEnumerable<SoapMessage> result = null;
            try
            {
                result = Repository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }

        private static IWeightScaleMessageDto GetWeightScaleMessageDto(SoapMessage item)
        {
            var message = Injector.Get<IWeightScaleMessageDto>();
            message.Id = item.Id;
            message.Message = item.Message.ToDomainType();
            message.ValidationMessages = Injector.Get<IValidationMessageCollection>();
            return message;
        }

        private static bool GrantDirectoryFullAccess(string fullPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(fullPath);
                var accessControl = directoryInfo.GetAccessControl();
                accessControl.AddAccessRule(
                    new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
                directoryInfo.SetAccessControl(accessControl);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToMessageAndCompleteStacktrace());
                return false;
            }
        }

        /// <summary>
        /// Logs the estimated time for transaction.
        /// </summary>
        /// <param name="beginTotal">
        /// The begin total.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        private static void LogEstimatedTimeForTransaction(DateTime beginTotal, long id)
        {
            var estimatedTime = DateTime.Now - beginTotal;
            Logger.Info($"[Id: \t{id}\t]    Total estimated time for transaction: {estimatedTime:ss\\:fff}");
        }

        private static void Main()
        {
            // make these
            var runAsWindowsService = Properties.Settings.Default.RunAsWindowsService;
            if (runAsWindowsService)
            {
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] { new MeasurementsWindowsService(Logger) };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                ProcessMeasurementsAsync();
                while (true)
                {
                }
            }
        }


        private static void ProcessMeasurementRequests(IEnumerable<SoapMessage> result, DateTime beginTotal)
        {
            if (result == null)
            {
                return;
            }

            var tasks = new List<Task>();
            foreach (var item in result)
            {
                var message = GetWeightScaleMessageDto(item);
                var messageType = message.Message.GetType().Name;
                var task = Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            var response = await SendMeasurementRequestAsync(messageType, message, item);
                            await SendMeasurementResultToCacheAsync(response, message, item, beginTotal);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"[Id: \t{item.Id}\t] {ex.Message}");
                            await SendErrorMessageToCache(item, message);
                            LogEstimatedTimeForTransaction(beginTotal, item.Id);
                        }
                    });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task SendErrorMessageToCache(SoapMessage item, IWeightScaleMessageDto message)
        {
            try
            {
                message.ValidationMessages.AddError($"Cannot execute measurement on {item.URL} ");
                Mapper.ToProxy(message, item);
                var validationMessages = (await Repository.UpdateAsync(item))?.ToList();
                if (validationMessages != null && validationMessages.Count > 0)
                {
                    foreach (var vm in validationMessages)
                    {
                        Logger.Error($"[Id: \t{item.Id}\t] ValidationMessage: {vm}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[Id: {item.Id}] Message cannot be sent due to: {ex.Message} ");
            }
        }

        private static HttpResponseMessage SendMeasurementRequest(
            string messageType,
            IWeightScaleMessageDto message,
            SoapMessage item)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
            Logger.Debug($"[Id: \t{message.Id}\t] ------------- Processing message Id: {message.Id} -------------");
            Logger.Debug($"[Id: \t{item.Id}\t] Sent request message: {item.URL} - {JsonConvert.SerializeObject(message)}");

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsJsonAsync(item.URL, message).Result;
            }
            catch (HttpRequestException ex)
            {
                Logger.Error(ex.Message, ex);
            }
            catch (WebException wex)
            {
                Logger.Error(wex.Message, wex);
            }
            catch (AggregateException ae)
            {
                foreach (var innerEx in ae.InnerExceptions)
                {
                    Logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                }
            }

            return response;
        }

        private static async Task<HttpResponseMessage> SendMeasurementRequestAsync(
            string messageType,
            IWeightScaleMessageDto message,
            SoapMessage item)
        {
            var vms = Injector.Get<IValidationMessageCollection>();
            var timeout = Settings.Default.Timeout;
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            var cancellationToken = cancellationTokenSource.Token;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
            Logger.Debug(string.Format("[Id: \t{0}\t] ------------- Processing message Id: {0} -------------", message.Id));
            Logger.Debug($"[Id: \t{item.Id}\t] Sent request message: {item.URL} - {JsonConvert.SerializeObject(message)}");

            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsJsonAsync(item.URL, message, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                var validationMessage = $"[Id: \t{item.Id}\t] Requested URL {item.URL} is unreachable. ";
                Logger.Error(validationMessage);
                vms.AddError("General Error", validationMessage);
            }
            catch (WebException wex)
            {
                Logger.Error(wex.Message, wex);
            }
            catch (AggregateException ae)
            {
                foreach (var innerEx in ae.InnerExceptions)
                {
                    Logger.Error(innerEx.ToMessageAndCompleteStacktrace());
                }
            }
            catch (TaskCanceledException ecex)
            {
                Logger.Error(
                    ecex.CancellationToken.IsCancellationRequested
                        ? $"[Id: \t{item.Id}\t] Measure operation with URL {item.URL} was canceled due to timeout."
                        : $"[Id: \t{item.Id}\t] {"ecex " + ecex.Message + ecex.StackTrace}");
            }
            catch (OperationCanceledException oce)
            {
                Logger.Error(
                    oce.CancellationToken.IsCancellationRequested
                        ? $"[Id: \t{item.Id}\t] Measure operation with URL {item.URL} was canceled due to timeout."
                        : $"[Id: \t{item.Id}\t] {"oce " + oce.Message + oce.StackTrace}");
            }
            catch (Exception exc)
            {
                Logger.Error($"[Id: \t{item.Id}\t] {"exc " + exc.Message + exc.StackTrace}");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            if (response == null && vms.Any())
            {
                message.ValidationMessages.AddMany(vms);
            }

            return response;
        }

        private static async Task SendMeasurementResultToCacheAsync(
            HttpResponseMessage response,
            IWeightScaleMessageDto message,
            SoapMessage item,
            DateTime beginTotal)
        {
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonAnswer = await response.Content.ReadAsStringAsync();
                    await UpdateMeasurementResultAsync(jsonAnswer, message, item, beginTotal);
                }
                else
                {
                    Logger.Error(
                        $"Error Code: {(response != null ? response.StatusCode.ToString() : "null")} : Message: {(response != null ? response.ReasonPhrase : "The response is null")}");
                    await SendErrorMessageToCache(item, message);
                }
            }
        }

        private static void UpdateMeasurementResult(string jsonAnswer, IWeightScaleMessageDto message, SoapMessage item)
        {
            var incomingMeasurementResult =
                Deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
            var currentSoap = item;
            Mapper.ToProxy(incomingMeasurementResult, currentSoap);
            var validationMessages = Repository.Update(currentSoap)?.ToList();

            if (validationMessages != null && validationMessages.Count > 0)
            {
                foreach (var vm in validationMessages)
                {
                    Logger.Error($"[Id: \t{item.Id}\t] ValidationMessage: {vm}");
                }
            }
        }

        private static async Task UpdateMeasurementResultAsync(
            string jsonAnswer,
            IWeightScaleMessageDto message,
            SoapMessage item,
            DateTime beginTotal)
        {
            Logger.Debug($"[Id: \t{item.Id}\t] Received response message: {jsonAnswer}");

            var incomingMeasurementResult =
                Deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
            var currentSoap = item;

            Mapper.ToProxy(incomingMeasurementResult, currentSoap);

            var validationMessages = (await Repository.UpdateAsync(currentSoap))?.ToList();

            if (validationMessages != null && validationMessages.Count > 0)
            {
                foreach (var vm in validationMessages)
                {
                    Logger.Error($"[Id: \t{item.Id}\t] ValidationMessage: {vm}");
                }
            }

            LogEstimatedTimeForTransaction(beginTotal, item.Id);
        }
    }
}