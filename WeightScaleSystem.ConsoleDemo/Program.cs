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
        private static int inBufferSize;
        private static IComSerializer serializer = new ComSerializer();
        static void Main(string[] args)
        {
            IKernel injector = NinjectInjector.GetInjector();
            ICommandFactory commands = injector.Get<ICommandFactory>();
            //IComSerializer serializer = injector.Get<IComSerializer>();
            Worker worker = new Worker();
            IBlock message = GenerateWeightBlock();

            using (var com = new ComManager())
            {

                SerialDataReceivedEventHandler handler = new SerialDataReceivedEventHandler(DataReceived);
                com.DataReceivedHandler = handler;
                com.Open();

                int counter = 0;

                var result = new byte[1];
                Console.WriteLine("-------------Step1-------------");

                var command = commands.WeightScaleRequest(message);

                do
                {
                    try
                    {
                        result = SendCommand(command, 1, com);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // TODO: Save exception to validation message collection
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    Console.WriteLine(result[0]);
                    counter++;
                } while (!((result[0] == (byte)ComunicationConstants.Eot) || counter > 4));

                Console.WriteLine("Counter: " + counter);

                command = commands.EndOfTransmit();
                SendCommand(command, 0, com);

                Console.WriteLine("-------------Step1 completed successfully-------------");
                if (!(result[0] == (byte)ComunicationConstants.Eot))
                {
                    //TODO: Save exception to validation message collection
                    Console.WriteLine("Cannot find WeightScale!!");
                    return;
                }

                Console.WriteLine("-------------Step2-------------");
                counter = 0;
                command = commands.SendDataToWeightScale(message);
                do
                {
                    try
                    {
                        result = SendCommand(command, 1, com);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // TODO: Save exception to validation message collection
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    Console.WriteLine(result[0]);
                    counter++;
                } while (!((result[0] == (byte)ComunicationConstants.Ack) || counter > 4));


                Console.WriteLine("Counter: " + counter);

                command = commands.EndOfTransmit();
                SendCommand(command, 0, com);

                Console.WriteLine("-------------Step2 completed successfully-------------");

                Console.WriteLine("-------------Step3-------------");
                counter = 0;
                command = commands.WeightScaleRequest(message);
                do
                {
                    try
                    {
                        result = SendCommand(command, 149, com);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // TODO: Save exception to validation message collection
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    Console.WriteLine(result[0]);
                    counter++;
                } while (!((result.Length == 149) || counter > 4));

                Console.WriteLine("Counter: " + counter);

                command = commands.Acknowledge();
                SendCommand(command, 0, com);

                Console.WriteLine("-------------Step3 completed successfully-------------");
                
                byte[] block = new byte[(int)BlockLen.NewProtocol];
                Array.Copy(result,3,block,0,block.Length);

                var des = serializer.Deserialize<WeightScaleMessageNew>(block);


                var rProps = GetProps(des);
                foreach (var prop in rProps)
                {
                    Console.WriteLine(prop);
                }


                Console.WriteLine("Completed");
            }

        }

        private static IBlock GenerateWeightBlock()
        {
            var ser = new WeightScaleMessageNew();
            //var ser = new WeightScaleMessageOld();
            ser.Number = 3;
            ser.Direction = Direction.Out;
            ser.TimeOfFirstMeasure = DateTime.Now.AddDays(-1).AddHours(-1);
            ser.TimeOfSecondMeasure = DateTime.Now;
            ser.MeasurementStatus = MeasurementStatus.ProtocolPrinterFailure;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 1;
            ser.ProductCode = 201;
            ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            ser.GrossWeight = 30;
            ser.TareWeight = 10;
            ser.NetWeight = 20;
            //ser.ProductName = "Нафта";
            // ser.TotalOfGrossWeight = 10;
            // ser.TotoalOfNetWeight = 20;
            return ser;
        }

        private static byte[] SendCommand(byte[] command, int inBufferLength, ComManager com)
        {
            System.Timers.Timer wDTimer = new System.Timers.Timer();
            wDTimer.Interval = 5 * 1000;
            wDTimer.Elapsed += timer_Elapsed;
            var result = new byte[inBufferLength];
            var unwanted = com.ReadAll();
            //clear buffer
            Console.WriteLine(unwanted.Length);
            com.SendComman(command, inBufferLength);

            if (inBufferLength == 0)
            {
                return new byte[0];
            }

            wDTimer.Start();
            while (!(received || WDTimerTick))
            {
            }
            if (received)
            {
                received = false;
                result = com.Read();

            }

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

            return result;
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
                list.Add(prop.Name + " As " + (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Name + " = " + prop.GetValue(cls));
            }
            return list;
        }

        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Message Received");
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