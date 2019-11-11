namespace WeightScaleSystem.ConsoleDemo
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using WeightScale.Application;
    using WeightScale.Application.AppStart;
    using WeightScale.Application.Contracts;
    using WeightScale.Application.Services;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Domain.Concrete;
    using Ninject;
    using log4net;
    using log4net.Config;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.Concrete;
    using WeightScale.CacheApi.SoapProxy;

    class Program
    {
        static int measures = 0;
        static int errors = 0;
        static volatile bool stop = false;
        static int iterations = 0;
        private const string ClearLogsWebApiController = @"api/Logs/PostClearLogs";

        static void Main(string[] args)
        {
            // Configure log4net
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            XmlConfigurator.Configure(configFile);
            ILog logger = LogManager.GetLogger("WeightScaleSystem.ConsoleDemo");
            // end of configuration

            IKernel injector = NinjectInjector.GetInjector;
            AutomapperConfig.AutoMapperConfig();
            var mapper = injector.Get<IMappingService>();

            var service = new MeasurementRequestsRepository(new Context("_system", "SYS"));
            var message = GenerateWeightBlock();
            var messageDto = new WeightScaleMessageDto() { Id = 2, Message = message, ValidationMessages = new ValidationMessageCollection(new List<ValidationMessage>() { new ValidationMessage(MessageType.Error, "Some test error!!!") }) };
            var soapMessage = new SoapMessage() { Message = new CWeigthScaleMessageNew() };
            mapper.ToProxy(messageDto, soapMessage);
            soapMessage.Id = messageDto.Id;
            soapMessage.URL = @"http://www.google.com";
            //soapMessage.ValidationMessages = new CValidationMessage[0];
            var result = service.Update(soapMessage);
            Console.WriteLine("There was {0} errors!", (result ?? new List<CValidationMessage>()).Count());
            //IWeightScaleMessage message = GenerateWeightBlock();

            //IWeightScaleMessageDto messageDto = new WeightScaleMessageDto() { Message = message, ValidationMessages = new ValidationMessageCollection() };
            //Thread stopper = new Thread(new ThreadStart(Stopper));
            //stoper.Start();
            //Thread stoper1 = new Thread(new ThreadStart(StopAtTheMorning));
            //stoper1.Start();
            //logger.Debug("Application begin at " + DateTime.Now + ".");

            //try
            //{
            //    using (var mService = injector.Get<IMeasurementService>())
            //    {
            //        //var mt = new MutexTest(mService, new WeightScaleMessageDto() { Message = GenerateWeightBlock(), ValidationMessages = new ValidationMessageCollection() });

            //        //Thread statusCheckerThread = new Thread(new ThreadStart(mt.CheckStatus));
            //        //statusCheckerThread.Start();

            //        while (!stop)
            //        {
            //            IWeightScaleMessage message = GenerateWeightBlock();
            //            IWeightScaleMessageDto messageDto = new WeightScaleMessageDto() { Message = message, ValidationMessages = new ValidationMessageCollection() };
            //            try
            //            {
            //                Task t1 = Task.Run(() => mService.IsWeightScaleOk(messageDto));
            //                Task t2 = Task.Run(() => mService.IsWeightScaleOk(messageDto));
            //                Task.WaitAll(new Task[] { t1, t2 });
            //            }
            //            catch (AggregateException ex)
            //            {
            //                foreach (var exc in ex.Flatten().InnerExceptions)
            //                {
            //                    Console.WriteLine(exc.Message);
            //                }
            //            }
            //            catch (InvalidOperationException ex)
            //            {
            //                Console.WriteLine(ex.Message + ex.StackTrace);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.Message);
            //            }
            //            //iterations++;

            //            //var begin = DateTime.Now;
            //            //mService.Measure(messageDto);
            //            //var estimatedTime = DateTime.Now - begin;

            //            //if (((WeightScaleMessageNew)messageDto.Message).MeasurementStatus == MeasurementStatus.OK)
            //            //{
            //            //    if (messageDto.ValidationMessages.Count() == 0)
            //            //    {
            //            //        measures++;
            //            //    }
            //            //    else
            //            //    {
            //            //        errors++;
            //            //    }
            //            //}
            //            //else
            //            //{
            //            //    errors++;
            //            //}

            //            //logger.Debug(string.Format("Number of iteration: {0} Successful measurements: {1} Errors: {2}", iterations, measures, errors));
            //            //logger.Debug(string.Format("Estimated time: {0}", estimatedTime));
            //            //Console.CursorLeft = 0;
            //            //Console.CursorTop = 0;
            //            //Console.WriteLine(iterations);
            //            //Thread.Sleep(5000);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.GetType().ToString());
            //    Console.WriteLine(ex.Message);
            //}
            //logger.Debug("Application end at " + DateTime.Now + ".");
            return;
        }

        /// <summary>
        /// Clears the logs.
        /// </summary>
        private static void ClearLogs()
        {
            string url = @"http://10.94.23.142:8111";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            try
            {
                var removingFileList = new List<string>() { "test.log", "test1.log" };
                string wsDelete = string.Format(@"{0}/{1}", url, ClearLogsWebApiController);
                //logger.Info(wsDelete);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.PostAsJsonAsync(wsDelete, removingFileList).Result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message, ex);
            }

            if (response != null && response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Removed logs files: {0} - {1}", url, string.Empty));
            }
        }

        /// <summary>
        /// Stops at the morning.
        /// </summary>
        private static void StopAtTheMorning()
        {
            while (!stop)
            {
                if (DateTime.Now > DateTime.ParseExact("20.03.2015 07:00", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture))
                {
                    stop = true;
                }
            }
        }

        /// <summary>
        /// Stoppers this instance.
        /// </summary>
        private static void Stopper()
        {
            try
            {
                Console.ReadKey();
                stop = true;
            }
            catch (Exception)
            {
                return;
            }
        }

        private static IWeightScaleMessage GenerateWeightBlock()
        {
            var ser = new WeightScaleMessageNew();
            //var ser = new WeightScaleMessageOld();
            ser.Number = 3;
            ser.Direction = Direction.In;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = (byte)((iterations % 2) + 1); // имам брояч на итерациите и всеки път давам 1/2/1/2/
            ser.ProductCode = 201;
            ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            ser.TimeOfFirstMeasure = DateTime.Now;
            ser.TimeOfSecondMeasure = DateTime.Now;
            //ser.LoadCapacity = 80000;
            return ser;
        }

        /// <summary>
        /// Extracts the properties to file.
        /// </summary>
        private static void ExtractPropertiesToFile()
        {
            var listOfProps = new List<string>();

            var class1 = new WeightScaleMessageNew();

            listOfProps = GetProps(class1);

            WriteResultToFile(listOfProps);

            var class2 = new WeightScaleMessageOld();

            listOfProps = GetProps(class2);

            WriteResultToFile(listOfProps);
        }

        /// <summary>
        /// Writes the result to file.
        /// </summary>
        /// <param name="listOfProps">The list of props.</param>
        private static void WriteResultToFile(List<string> listOfProps)
        {
            string path = "properties.txt";


            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.

                File.WriteAllLines(path, listOfProps, Encoding.UTF8);
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            File.AppendAllLines(path, listOfProps, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the props.
        /// </summary>
        /// <param name="class1">The class1.</param>
        /// <returns></returns>
        private static List<string> GetProps(object cls)
        {
            var props = cls.GetType()
                           .GetProperties()
                           .Where(x => x.CustomAttributes.Where(y => y.AttributeType == typeof(ComSerializablePropertyAttribute)).Count() != 0)
                           .OrderBy(x => ((x.GetCustomAttributes(typeof(ComSerializablePropertyAttribute), true).FirstOrDefault()) as ComSerializablePropertyAttribute).Offset);
            var list = new List<string>(props.Count());
            list.Add(string.Empty);
            list.Add(cls.GetType().Name);
            list.Add("----------------------------------------");
            list.Add(string.Empty);

            foreach (var prop in props)
            {
                list.Add(prop.Name + " As " + (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Name + " = " + prop.GetValue(cls));
            }
            return list;
        }


    }

    public class MutexTest
    {
        private readonly IMeasurementService mService;
        private readonly IWeightScaleMessageDto messageDto;

        public MutexTest(IMeasurementService mServiceParam, IWeightScaleMessageDto messageParam)
        {
            this.mService = mServiceParam;
            this.messageDto = messageParam;
        }
        public void CheckStatus()
        {
            int rownum = 1;
            while (true)
            {
                if (mService.IsWeightScaleOk(messageDto))
                {
                    Console.CursorTop = rownum++;
                    Console.WriteLine("{0} The status of the scale {1} is OK", DateTime.Now, messageDto.Message.Number);
                }
                else
                {
                    var valMessages = GetProps(messageDto.ValidationMessages);
                    foreach (var err in messageDto.ValidationMessages.Errors)
                    {
                        valMessages.Add(err.Text);
                    }

                    foreach (var prop in valMessages)
                    {
                        Console.WriteLine(prop);
                    }
                    messageDto.ValidationMessages.Clear();
                }
                Thread.Sleep(20000);
            }
        }

        private List<string> GetProps(object cls)
        {
            var props = cls.GetType()
                           .GetProperties()
                           .Where(x => x.CustomAttributes.Where(y => y.AttributeType == typeof(ComSerializablePropertyAttribute)).Count() != 0)
                           .OrderBy(x => ((x.GetCustomAttributes(typeof(ComSerializablePropertyAttribute), true).FirstOrDefault()) as ComSerializablePropertyAttribute).Offset);
            var list = new List<string>(props.Count());
            list.Add(string.Empty);
            list.Add(cls.GetType().Name);
            list.Add("----------------------------------------");
            list.Add(string.Empty);

            foreach (var prop in props)
            {
                list.Add(prop.Name + " As " + (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Name + " = " + prop.GetValue(cls));
            }
            return list;
        }
    }

    class Context : IConnectionParameters
    {
        public Context(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }

        public string Password { get; }
    }
}