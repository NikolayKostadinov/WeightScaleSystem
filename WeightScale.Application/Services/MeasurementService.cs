//---------------------------------------------------------------------------------
// <copyright file="MeasurementService.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Timers;
    using Ninject;
    using WeightScale.Application.Contracts;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Domain.Concrete;
    using log4net;

    public class MeasurementService : IMeasurementService, IDisposable
    {
        private const int INTERVAL = 5 * 1000;
        private volatile bool received = false;
        private bool wDTimerTick = false;
        private readonly IComSerializer serializer;
        private readonly ICommandFactory commands;
        private readonly IComManager com;
        private readonly IKernel kernel;
        private readonly Timer wDTimer;
        private readonly ILog loger;
        private int iterations;


        public MeasurementService(ICommandFactory commandParam, IComSerializer serializerParam, IComManager comManagerParam, IKernel kernel, ILog loger, int iterationsParam = 5)
        {
            this.commands = commandParam;
            this.serializer = serializerParam;
            wDTimer = new Timer(INTERVAL);
            wDTimer.Elapsed += WDTimer_Elapsed;
            this.Iterations = iterationsParam;
            this.com = comManagerParam;
            this.kernel = kernel;
            this.loger = loger;

            SerialDataReceivedEventHandler handler = new SerialDataReceivedEventHandler(DataReceived);
            com.DataReceivedHandler = handler;
            com.Open();
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

        public bool IsWeightScaleOk(IWeightScaleMessageDto messageDto) 
        {
            var command = commands.WeightScaleRequest(messageDto.Message);
            var trailingCommand = commands.EndOfTransmit();
            var validationMessages = kernel.Get<IValidationMessageCollection>();
            try
            {
                string errMessage = "Cannot find WeightScale number" + messageDto.Message.Number;
                var comAnswer = DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Eot, errMessage, trailingCommand, validationMessages);
                loger.Debug(string.Format("Command: {0} Answer Step1: {1}", ByteArrayToString(command), ByteArrayToString(comAnswer)));
            }
            catch (InvalidOperationException ex) 
            {
                command = commands.Acknowledge();
                SendCommand(command, 0, com);
                loger.Info(string.Format("Command clear broken communication: {0}", ByteArrayToString(command)));
                messageDto.ValidationMessages.AddError(ex.Message);
                messageDto.ValidationMessages.AddMany(validationMessages);
                loger.Error(ex.Message);
                foreach (var err in validationMessages)
                {
                    loger.Error(err.Text);
                }
                return false;
            }
            if (messageDto.ValidationMessages.Count() > 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        public void Measure(IWeightScaleMessageDto messageDto)
        {
            int blockLength = GetBlockLength(messageDto.Message);
            const int PAILOAD_LEN = 5;

            var comAnswer = new byte[1];

            var command = commands.WeightScaleRequest(messageDto.Message);
            var trailingCommand = commands.EndOfTransmit();
            var validationMessages = kernel.Get<IValidationMessageCollection>();
            try
            {
                string errMessage = "Cannot find WeightScale number" + messageDto.Message.Number;
                comAnswer = DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Eot, errMessage, trailingCommand, validationMessages);
                loger.Debug(string.Format("Command: {0} Answer Step1: {1}", ByteArrayToString(command), ByteArrayToString(comAnswer)));
                // -------------Step1 completed successfully------------- //
                // -------------Step2------------- //;

                command = commands.SendDataToWeightScale(messageDto.Message);
                comAnswer = DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Ack, "Cannot send data to weight scale", trailingCommand, validationMessages);
                loger.Debug(string.Format("Command: {0} Answer Step2: {1}", ByteArrayToString(command, blockLength), ByteArrayToString(comAnswer)));
                // -------------Step2 completed successfully-------------
                // -------------Step3-------------

                command = commands.WeightScaleRequest(messageDto.Message);
                trailingCommand = commands.Acknowledge();
                Predicate<byte[]> checkMessage = x => this.commands.CheckMeasurementDataFromWeightScale(blockLength, messageDto.Message.Number, x);
                comAnswer = DoProtocolStep(command, blockLength + PAILOAD_LEN, checkMessage, "Invalid data in received block", trailingCommand, validationMessages);
                FormResult(blockLength, comAnswer, messageDto);
                loger.Debug(string.Format("Command: {0} Answer Step3: {1}", ByteArrayToString(command), ByteArrayToString(comAnswer, blockLength, messageDto)));
            }
            catch (InvalidOperationException ex)
            {
                command = commands.Acknowledge();
                SendCommand(command, 0, com);
                loger.Info(string.Format("Command clear broken communication: {0}", ByteArrayToString(command)));
                messageDto.ValidationMessages.AddError(ex.Message);
                messageDto.ValidationMessages.AddMany(validationMessages);
                loger.Error(ex.Message);
                foreach (var err in validationMessages)
                {
                    loger.Error(err.Text);
                }
            }
            // -------------Step3 completed successfully-------------
        }

        private void FormResult(int blockLength, byte[] comAnswer, IWeightScaleMessageDto message)
        {
            byte[] block = new byte[blockLength];
            Array.Copy(comAnswer, 3, block, 0, block.Length);
            try
            {
                // Executes generic method without nowing of concrete type
                MethodInfo deserialize = serializer.GetType().GetMethod("Deserialize");
                MethodInfo genericMethod = deserialize.MakeGenericMethod(message.Message.GetType());
                IWeightScaleMessage des = genericMethod.Invoke(serializer, new object[] { block }) as IWeightScaleMessage;

                message.Message = des;
                message.ValidationMessages.AddMany(des.Validate());
            }
            catch (Exception ex)
            {
                message.ValidationMessages.AddError(ex.Message);
            }
        }

        private byte[] DoProtocolStep(byte[] command, int resultLength, Predicate<byte[]> isOk, string errMessage, byte[] trailingCommand, IValidationMessageCollection ValidationMessages)
        {
            int counter = 0;
            byte[] result = new byte[resultLength];
            do
            {
                result = this.SendCommand(command, resultLength, com);
                counter++;
            } while (!(isOk(result) || (counter > this.iterations)));

            if (!isOk(result))
            {
                ValidationMessages.AddError(string.Format("The command is: {0}", ByteArrayToString(command)));
                ValidationMessages.AddError(string.Format("The answer is: {0}", ByteArrayToString(result)));
                throw new InvalidOperationException(errMessage);
            }

            SendCommand(trailingCommand, 0, com);
            return result;
        }

        private string ByteArrayToString(byte[] result, int blockLength = 0, IWeightScaleMessageDto messageDto = null)
        {
            StringBuilder strResult = new StringBuilder();
            if (blockLength == 0)
            {
                foreach (byte item in result)
                {
                    strResult.Append(DecodeByte(item));
                    strResult.Append(" ");
                }
            }
            else
            {
                byte[] block = new byte[blockLength];
                Array.Copy(result, 3, block, 0, block.Length);
                //write decodec message
                strResult.Append(DecodeByte(result[0]));
                strResult.Append(" ");
                strResult.Append(result[1]);
                strResult.Append(" ");
                strResult.Append(DecodeByte(result[2]));
                strResult.Append(" ");
                if (messageDto == null)
                {
                    strResult.Append(Encoding.Default.GetString(block));
                }
                else
                {
                    strResult.Append(messageDto.Message.ToString());
                }
                strResult.Append(" ");
                strResult.Append(DecodeByte(result[result.Length - 2]));
                strResult.Append(" ");
                strResult.Append(result[result.Length - 1]);
                strResult.Append(" ");
            }

            return strResult.ToString();
        }

        private string DecodeByte(byte item)
        {
            if (Enum.IsDefined(typeof(ComunicationConstants), item))
            {
                return string.Format("<{0}>", ((ComunicationConstants)item).ToString());
            }
            else
            {
                return item.ToString();
            }
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
