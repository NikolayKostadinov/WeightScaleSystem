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
        private static bool WDTimerTick = false;
        static void Main(string[] args)
        {
            IKernel injector = NinjectInjector.GetInjector();
            ICommandFactory commands = injector.Get<ICommandFactory>();
            Worker worker = new Worker();
            Thread workerThread = new Thread(worker.GetSimbole);
            workerThread.Start();

            //var ser = new WeightScaleMessageNew();
            var ser = new WeightScaleMessageOld();
            ser.Number = 5;
            ser.Direction = Direction.Out;
            ser.TimeOfFirstMeasure = DateTime.Now.AddDays(-1).AddHours(-1);
            ser.TimeOfSecondMeasure = DateTime.Now;
            ser.MeasurementStatus = MeasurementStatus.ProtocolPrinterFailure;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 1;
            ser.ProductCode = 141;
            //ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            ser.GrossWeight = 30;
            ser.TareWeight = 10;
            ser.NetWeight = 20;
            ser.ProductName = "Нафта";
            // ser.TotalOfGrossWeight = 10;
            // ser.TotoalOfNetWeight = 20;
            while (! (Worker.stopChar=='a'))
            {
                var command = commands.WeightScaleRequest(ser);
                SendCommand(command, command.Length);
                command = commands.SendDataToWeightScale(ser);
                SendCommand(command, command.Length);
            }

            worker.RequestStop();
            //workerThread.Join();
            Console.WriteLine("Press a key to exit...");

            //Console.WriteLine(Encoding.Default.GetString((commands.SendDataToWeightScale(ser))));
            //var validationResult = ser.Validate();
            //foreach (var validationMessage in validationResult)
            //{
            //    Console.WriteLine("{0} {1}: {2}", validationMessage.Type.ToString(), validationMessage.Field, validationMessage.Text);
            //}
            //var serializer = new ComSerializer();
            //var btime = DateTime.Now;
            //var serialized = serializer.Setialize(ser);
            //var estimatedTime = DateTime.Now - btime;
            //string result = string.Empty;
            //result = new string(Encoding.Default.GetChars(serialized));
            //ComSerializableClassAttribute attr = Attribute.GetCustomAttribute(ser.GetType(), typeof(ComSerializableClassAttribute)) as ComSerializableClassAttribute;
            //if (attr != null)
            //{
            //    Console.WriteLine("The result type: {0}\nblock type: {1}\nblock Length:{2}\nactual length: {3}",
            //        ser.GetType().Name,
            //        attr.BlockLength,
            //        (int)attr.BlockLength,
            //        result.Length);
            //}
            //var des = serializer.Deserialize<WeightScaleMessageOld>(serialized);
            //serialized = serializer.Setialize(des);
            //var result1 = new string(Encoding.Default.GetChars(serialized));
            //Console.WriteLine(result);
            //Console.WriteLine(result1);
            //Console.WriteLine("Estimated time: {0}", estimatedTime);
        }
  
        private static void SendCommand(byte[] command, int inBufferLength)
        {
            System.Timers.Timer wDTimer = new System.Timers.Timer();
            wDTimer.Interval = 5 * 1000;
            wDTimer.Elapsed += timer_Elapsed;
            SerialDataReceivedEventHandler handler = new SerialDataReceivedEventHandler(DataReceived);
            handler += SecondHandler;
            using (ComManager com = new ComManager())
            {
                com.DataReceivedHandler = handler;

                try
                {
                    com.Open();
                    com.SendComman(command, inBufferLength);
                    wDTimer.Start();
                    while (!(received||WDTimerTick))
                    {
                    }
                    received = false;
                    if (WDTimerTick)
                    {
                        WDTimerTick = false;
                        throw new InvalidOperationException(string.Format("No answer from {0}", com.PortName));
                    }
                    else 
                    {
                        WDTimerTick = false;
                        wDTimer.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            WDTimerTick = true;
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
                list.Add(prop.Name + " As " + (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Name);
            }
            return list;
        }

        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Message Received");
             
            var port = (sender as SerialPort);
            var result = new byte[port.ReceivedBytesThreshold];
            port.Read(result,0,result.Length);

            foreach (byte item in result)
            {
                Console.Write(item);
                Console.Write(" ");
            }

            Console.WriteLine();

            received = true;
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
            while (! shouldStop)
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