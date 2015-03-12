using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WeightScale.Application;
using WeightScale.Application.App_Start;
using WeightScale.Application.Contracts;
using WeightScale.Application.Services;
using WeightScale.ComunicationProtocol;
using WeightScale.ComunicationProtocol.Contracts;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;
using Ninject;

namespace WeightScaleSystem.ConsoleDemo
{
    class Program
    {
        private static bool received = false;
        private static bool completed = false;
        private static bool WDTimerTick = false;
        private static IComSerializer serializer = new ComSerializer();
        static void Main(string[] args)
        {
            IKernel injector = NinjectInjector.GetInjector();
            ICommandFactory commands = injector.Get<ICommandFactory>();
            //IComSerializer serializer = injector.Get<IComSerializer>();
            Worker worker = new Worker();
            IWeightScaleMessage message = GenerateWeightBlock();
            var mService = injector.Get<MeasurementService>();
            IWeightScaleMessageDto messageDto = new WeightScaleMessageDto(){Message = message, ValidationMessages = new ValidationMessageCollection()};
           
            mService.Measure(messageDto);

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

        }

        private static IWeightScaleMessage GenerateWeightBlock()
        {
            var ser = new WeightScaleMessageNew();
            //var ser = new WeightScaleMessageOld();
            ser.Number = 3;
            ser.Direction = Direction.Out;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 2;
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



        static void SecondHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("I am SecondHandler");
        }
    }

    class Worker
    {
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool shouldStop;

        public static char stopChar;
        /// <summary>
        /// Gets the simbole asynk.
        /// </summary>
        public void GetSimbole()
        {
            while (!shouldStop)
            {
                stopChar = Console.ReadKey().KeyChar;
            }
        }

        public void RequestStop()
        {
            shouldStop = true;
        }

    }
}