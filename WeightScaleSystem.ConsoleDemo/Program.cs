using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeightScaleSystem.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //ExtractPropertiesToFile();
            var ser = new WeightScaleMessageNew();
            // var ser = new WeightScaleMessageOld();
            ser.Number = 1;
            ser.Direction = Direction.Out;
            ser.TimeOfFirstMeasure = DateTime.Now.AddDays(-1).AddHours(-1);
            ser.TimeOfSecondMeasure = DateTime.Now;
            ser.MeasurementStatus = MeasurementStatus.ProtocolPrinterFailure;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 1;
            ser.ProductCode = 141;
            ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            ser.GrossWeight = 30;
            ser.TareWeight = 10;
            ser.NetWeight = 20;
            // ser.ProductName = "Нафта";
            // ser.TotalOfGrossWeight = 10;
            // ser.TotoalOfNetWeight = 20;

            var validationResult = ser.Validate();

            foreach (var validationMessage in validationResult)
            {
                Console.WriteLine("{0} {1}: {2}", validationMessage.Type.ToString(), validationMessage.Field, validationMessage.Text);
            }

            var serializer = new ComSerializer();

            var btime = DateTime.Now;
            var serialized = serializer.Setialize(ser);
            var estimatedTime = DateTime.Now - btime;

            string result = string.Empty;
            result = new string(Encoding.Default.GetChars(serialized));

            ComSerializableClassAttribute attr = Attribute.GetCustomAttribute(ser.GetType(), typeof(ComSerializableClassAttribute)) as ComSerializableClassAttribute;
            if (attr != null)
            {
                Console.WriteLine("The result type: {0}\nblock type: {1}\nblock Length:{2}\nactual length: {3}",
                    ser.GetType().Name,
                    attr.BlockLength,
                    (int)attr.BlockLength,
                    result.Length);
            }

            var des = serializer.Deserialize<WeightScaleMessageNew>(serialized);
            serialized = serializer.Setialize(des);
            var result1 = new string(Encoding.Default.GetChars(serialized));

            Console.WriteLine(result);
            Console.WriteLine(result1);
            Console.WriteLine("Estimated time: {0}", estimatedTime);
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
    }
}