namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Timers;
    using WeightScale.Application.Contracts;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Domain.Concrete;

    public class MeasurementService : IMeasurementService, IDisposable
    {
        private const int INTERVAL = 5 * 1000;
        private bool received = false;
        private bool wDTimerTick = false;
        private readonly IComSerializer serializer;
        private readonly ICommandFactory commands;
        private readonly IComManager com;
        private readonly Timer wDTimer;
        private int iterations;

        public MeasurementService(ICommandFactory commandParam, IComSerializer serializerParam, IComManager comManagerParam, int iterationsParam = 5)
        {
            this.commands = commandParam;
            this.serializer = serializerParam;
            wDTimer = new Timer(INTERVAL);
            wDTimer.Elapsed += WDTimer_Elapsed;
            this.Iterations = iterationsParam;
            this.com = comManagerParam;
        }

        /// <summary>
        /// Gets or sets the iterations.
        /// </summary>
        /// <value>The iterations.</value>
        private int Iterations
        {
            set
            {
                if (value < 0)
                {
                    var message = "The argument iterationsParam must be greater by 0. Current value is " + value;
                    throw new ArgumentException("iterationsParam", message);
                }

                this.iterations = value;
            }
        }

        public void Measure(IWeightScaleMessageDto messageDto)
        {
            int blockLength = GetBlockLength(messageDto.Message);
            const int PAILOAD_LEN = 5;


            SerialDataReceivedEventHandler handler = new SerialDataReceivedEventHandler(DataReceived);
            com.DataReceivedHandler = handler;
            com.Open();

            var comAnswer = new byte[1];

            var command = commands.WeightScaleRequest(messageDto.Message);
            var trailingCommand = commands.EndOfTransmit();
            try
            {
                string errMessage = "Cannot find WeightScale number" + messageDto.Message.Number;
                comAnswer = DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Eot, errMessage, trailingCommand);
                Console.WriteLine("Answer Step1: {0}",comAnswer[0]);
                // -------------Step1 completed successfully------------- //
                // -------------Step2------------- //;

                command = commands.SendDataToWeightScale(messageDto.Message);
                comAnswer = DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Ack, "Cannot send data to weight scale", trailingCommand);
                Console.WriteLine("Answer Step2: {0}", comAnswer[0]);
                // -------------Step2 completed successfully-------------
                // -------------Step3-------------

                command = commands.WeightScaleRequest(messageDto.Message);
                trailingCommand = commands.Acknowledge();
                Predicate<byte[]> checkMessage = x => this.commands.CheckMeasurementDataFromWeightScale(blockLength, messageDto.Message.Number, x);
                comAnswer = DoProtocolStep(command, blockLength + PAILOAD_LEN, checkMessage, "Invalid data in received block", trailingCommand);
            }
            catch (InvalidOperationException ex)
            {
                messageDto.ValidationMessages.AddError(ex.Message);
                return;
            }
            // -------------Step3 completed successfully-------------

            FormResult(blockLength, comAnswer,messageDto);
        }

        private void FormResult(int blockLength, byte[] comAnswer, IWeightScaleMessageDto message)
        {
            byte[] block = new byte[blockLength];
            Array.Copy(comAnswer, 3, block, 0, block.Length);
            try
            {
                //TODO: CheckThis
                var des = serializer.Deserialize<WeightScaleMessageNew>(block);
                message.Message = des;
                message.ValidationMessages.AddRange(des.Validate());
            }
            catch (Exception ex) 
            {
                message.ValidationMessages.AddError(ex.Message);
            }
        }

        private byte[] DoProtocolStep(byte[] command, int resultLength, Predicate<byte[]> isOk, string errMessage, byte[] trailingCommand)
        {
            int counter = 0;
            byte[] result = new byte[resultLength];
            do
            {
                result = this.SendCommand(command, resultLength, com);
                counter++;
                DisplayResult(result);
            } while (!(isOk(result) || (counter > this.iterations)));

            if (!isOk(result))
            {
                throw new InvalidOperationException(errMessage);
            }

            SendCommand(trailingCommand, 0, com);
            return result;
        }
 
        /// <summary>
        /// Displays the result.
        /// </summary>
        /// <param name="result">The result.</param>
        private void DisplayResult(byte[] result)
        {
            foreach (byte item in result)
            {
                Console.Write(item);
                Console.Write(" ");
            }

            Console.WriteLine();
        }

        private byte[] SendCommand(byte[] command, int inBufferLength, IComManager com)
        {
            var result = new byte[inBufferLength];
            var unwanted = com.ReadAll();
            com.SendComman(command, inBufferLength);

            if (inBufferLength == 0)
            {
                return new byte[0];
            }

            this.wDTimer.Start();
            while (!(received || wDTimerTick))
            {
            }
            if (received)
            {
                received = false;
                result = com.Read();
            }

            if (wDTimerTick)
            {
                wDTimerTick = false;
                throw new InvalidOperationException(string.Format("No answer from {0}", com.PortName));
            }
            else
            {
                wDTimerTick = false;
                this.wDTimer.Stop();
            }

            return result;
        }


        private void WDTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            this.wDTimerTick = true;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine("Message Received");
            received = true;
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

        private int GetBlockLength(IWeightScaleMessage message)
        {
            var attr = message.GetType().GetCustomAttributes(true)
                .Where(x => x is ComSerializableClassAttribute)
                .FirstOrDefault() as ComSerializableClassAttribute;

            if (attr == null)
            {
                throw new ArgumentException(
                    "serObject",
                    "Serialized Object must be decorated with ComSerializableClassAttribute");
            }

            return (int)attr.BlockLength;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.com.Dispose();
        }
    }
}
