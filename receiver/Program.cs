using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.Application;

namespace receiver
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var com = new ComManager("COM1",4800,Parity.Even,8,StopBits.One)) 
            {
                com.ReceiveBytesThreshold = 1;
                com.DataReceivedHandler = new SerialDataReceivedEventHandler(DataReceived);
                com.Open();
                while (true)
                {
                }
            }
        }

        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Message Received");

            var port = (sender as SerialPort);
            var result = new byte[port.ReceivedBytesThreshold];
            port.Read(result, 0, result.Length);

            foreach (byte item in result)
            {
                Console.Write(item);
                Console.Write(" ");
            }

            Console.WriteLine();
            port.Write(new byte[] { 4 }, 0, 1);

            //received = true;
        }
    }
}
