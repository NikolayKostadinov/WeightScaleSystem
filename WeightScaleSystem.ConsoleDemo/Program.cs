using System;
using System.Collections.Generic;
using System.Globalization;
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
            var ser = new NewWeightScaleMessage();
            // var ser = new OldWeightScaleMessage();
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

            Console.WriteLine(result);
            Console.WriteLine("Estimated time: {0}", estimatedTime);
        }
    }
}
