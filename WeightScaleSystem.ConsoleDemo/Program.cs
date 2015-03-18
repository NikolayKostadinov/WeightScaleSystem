namespace WeightScaleSystem.ConsoleDemo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
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


    class Program
    {
        static void Main(string[] args)
        {
            // Confugure log4net
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            XmlConfigurator.Configure(configFile);
            ILog logger = LogManager.GetLogger("WeightScaleSystem.ConsoleDemo");
            // end of configuretion

            IKernel injector = NinjectInjector.GetInjector;
            IWeightScaleMessage message = GenerateWeightBlock();
            
            IWeightScaleMessageDto messageDto = new WeightScaleMessageDto() { Message = message, ValidationMessages = new ValidationMessageCollection() };
           

            logger.Debug("Application begin.");
            try
            {
                using (var mService = injector.Get<MeasurementService>())
                {
                    if (mService.IsWeightScaleOk(messageDto))
                    {
                        Console.WriteLine("The status of the scale {0} is Ok", messageDto.Message.Number);
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
                    var begin = DateTime.Now;

                    mService.Measure(messageDto);

                    logger.Debug(string.Format("Estimated time: {0}", DateTime.Now - begin));

                    var props = GetProps(messageDto.Message);
                    props.Add(string.Empty);
                    props.AddRange(GetProps(messageDto.ValidationMessages));
                    foreach (var err in messageDto.ValidationMessages.Errors)
                    {
                        props.Add(err.Text);
                    }

                    foreach (var prop in props)
                    {
                        Console.WriteLine(prop);
                    }

                    logger.Debug("Application finish.");
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.GetType().ToString());
                Console.WriteLine(ex.Message);
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
            ser.MeasurementNumber = 1;
            ser.ProductCode = 201;
            ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
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

}