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
    using System.Threading;
    using System.Timers;
    using log4net;
    using Ninject;
    using WeightScale.Application.Contracts;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Presents two services. Measure witch makes measurement and IsWeightScaleOk witch checks weight scale status.
    /// </summary>
    public class MeasurementService : IMeasurementService, IDisposable
    {
        private const int INTERVAL = 5 * 1000;
        private readonly IComSerializer serializer;
        private readonly ICommandFactory commands;
        private readonly IComManager com;
        private readonly IKernel kernel;
        private readonly System.Timers.Timer watchDogTimer;
        private readonly ILog loger;
        private volatile bool received = false;
        private volatile bool watchDogTimerTick = false;
        private int iterations;
        private readonly Mutex mutex = new Mutex();


        public MeasurementService(ICommandFactory commandParam, IComSerializer serializerParam, IComManager comManagerParam, IKernel kernel, ILog loger, int iterationsParam = 5)
        {
            this.commands = commandParam;
            this.serializer = serializerParam;
            this.watchDogTimer = new System.Timers.Timer(INTERVAL);
            this.watchDogTimer.Elapsed += this.WDTimer_Elapsed;
            this.Iterations = iterationsParam;
            this.com = comManagerParam;
            this.kernel = kernel;
            this.loger = loger;

            SerialDataReceivedEventHandler handler = new SerialDataReceivedEventHandler(this.DataReceived);
            this.com.DataReceivedHandler = handler;
            this.com.Open();
        }

        /// <summary>
        /// Sets the iterations.
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
            if (mutex.WaitOne(INTERVAL * 3))
            {
                var command = this.commands.WeightScaleRequest(messageDto.Message);
                var trailingCommand = this.commands.EndOfTransmit();
                var validationMessages = this.kernel.Get<IValidationMessageCollection>();
                try
                {
                    string errMessage = "Cannot find WeightScale number" + messageDto.Message.Number;
                    var comAnswer = this.DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Eot, errMessage, trailingCommand, validationMessages);
                    this.loger.Debug(string.Format("Command: {0} Answer Step1: {1}", this.ByteArrayToString(command), this.ByteArrayToString(comAnswer)));
                }
                catch (InvalidOperationException ex)
                {
                    command = this.commands.Acknowledge();
                    this.SendCommand(command, 0, this.com);
                    this.loger.Info(string.Format("Command clear broken communication: {0}", this.ByteArrayToString(command)));
                    messageDto.ValidationMessages.AddError(ex.Message);
                    messageDto.ValidationMessages.AddMany(validationMessages);
                    this.loger.Error(ex.Message);
                    foreach (var err in validationMessages)
                    {
                        this.loger.Error(err.Text);
                    }

                    mutex.ReleaseMutex();
                    return false;
                }

                if (messageDto.ValidationMessages.Count() > 0)
                {
                    mutex.ReleaseMutex();
                    return false;
                }
                else
                {
                    mutex.ReleaseMutex();
                    return true;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot get exclusive access to measurement service. ");
            }
        }

        public void Measure(IWeightScaleMessageDto messageDto)
        {
            if (mutex.WaitOne(INTERVAL * 3))
            {
                int blockLength = this.GetBlockLength(messageDto.Message);
                const int PAILOAD_LEN = 5;

                var comAnswer = new byte[1];

                var command = this.commands.WeightScaleRequest(messageDto.Message);
                var trailingCommand = this.commands.EndOfTransmit();
                var validationMessages = this.kernel.Get<IValidationMessageCollection>();
                try
                {
                    string errMessage = "Cannot find WeightScale number" + messageDto.Message.Number;
                    comAnswer = this.DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Eot, errMessage, trailingCommand, validationMessages);
                    this.loger.Debug(string.Format("Command: {0} Answer Step1: {1}", this.ByteArrayToString(command), this.ByteArrayToString(comAnswer)));

                    // -------------Step1 completed successfully------------- 
                    // -------------Step2------------- 
                    command = this.commands.SendDataToWeightScale(messageDto.Message);
                    comAnswer = this.DoProtocolStep(command, 1, x => x[0] == (byte)ComunicationConstants.Ack, "Cannot send data to weight scale", trailingCommand, validationMessages);
                    this.loger.Debug(string.Format("Command: {0} Answer Step2: {1}", this.ByteArrayToString(command, blockLength), this.ByteArrayToString(comAnswer)));

                    // -------------Step2 completed successfully-------------
                    // -------------Step3-------------
                    command = this.commands.WeightScaleRequest(messageDto.Message);
                    trailingCommand = this.commands.Acknowledge();
                    Predicate<byte[]> checkMessage = x => this.commands.CheckMeasurementDataFromWeightScale(blockLength, messageDto.Message.Number, x);
                    comAnswer = this.DoProtocolStep(command, blockLength + PAILOAD_LEN, checkMessage, "Invalid data in received block", trailingCommand, validationMessages);
                    this.FormResult(blockLength, comAnswer, messageDto);
                    this.loger.Debug(string.Format("Command: {0} Answer Step3: {1}", this.ByteArrayToString(command), this.ByteArrayToString(comAnswer, blockLength, messageDto)));
                }
                catch (InvalidOperationException ex)
                {
                    command = this.commands.Acknowledge();
                    this.SendCommand(command, 0, this.com);
                    this.loger.Info(string.Format("Command clear broken communication: {0}", this.ByteArrayToString(command)));
                    messageDto.ValidationMessages.AddError(ex.Message);
                    messageDto.ValidationMessages.AddMany(validationMessages);
                    this.loger.Error(ex.Message);
                    foreach (var err in validationMessages)
                    {
                        this.loger.Error(err.Text);
                    }
                }
                // -------------Step3 completed successfully-------------
                finally 
                {
                    mutex.ReleaseMutex();
                }
            }
            else 
            {
                throw new InvalidOperationException("Cannot get exclusive access to measurement service. ");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.com.Dispose();
        }

        private void FormResult(int blockLength, byte[] comAnswer, IWeightScaleMessageDto message)
        {
            byte[] block = new byte[blockLength];
            Array.Copy(comAnswer, 3, block, 0, block.Length);
            try
            {
                // Executes generic method without nowing of concrete type
                MethodInfo deserialize = this.serializer.GetType().GetMethod("Deserialize");
                MethodInfo genericMethod = deserialize.MakeGenericMethod(message.Message.GetType());
                IWeightScaleMessage des = genericMethod.Invoke(this.serializer, new object[] { block }) as IWeightScaleMessage;

                message.Message = des;
                message.ValidationMessages.AddMany(des.Validate());
            }
            catch (Exception ex)
            {
                message.ValidationMessages.AddError(ex.Message);
            }
        }

        private byte[] DoProtocolStep(byte[] command, int resultLength, Predicate<byte[]> isOk, string errMessage, byte[] trailingCommand, IValidationMessageCollection validationMessages)
        {
            int counter = 0;
            byte[] result = new byte[resultLength];
            do
            {
                result = this.SendCommand(command, resultLength, this.com);
                counter++;
            }
            while (!(isOk(result) || (counter > this.iterations)));

            if (!isOk(result))
            {
                validationMessages.AddError(string.Format("The command is: {0}", this.ByteArrayToString(command)));
                validationMessages.AddError(string.Format("The answer is: {0}", this.ByteArrayToString(result)));
                throw new InvalidOperationException(errMessage);
            }

            this.SendCommand(trailingCommand, 0, this.com);
            return result;
        }

        private string ByteArrayToString(byte[] result, int blockLength = 0, IWeightScaleMessageDto messageDto = null)
        {
            StringBuilder strResult = new StringBuilder();
            if (blockLength == 0)
            {
                foreach (byte item in result)
                {
                    strResult.Append(this.DecodeByte(item));
                    strResult.Append(" ");
                }
            }
            else
            {
                byte[] block = new byte[blockLength];
                Array.Copy(result, 3, block, 0, block.Length);

                // write decoded message
                strResult.Append(this.DecodeByte(result[0]));
                strResult.Append(" ");
                strResult.Append(result[1]);
                strResult.Append(" ");
                strResult.Append(this.DecodeByte(result[2]));
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
                strResult.Append(this.DecodeByte(result[result.Length - 2]));
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

            this.watchDogTimer.Start();
            while (!(this.received || this.watchDogTimerTick))
            {
            }

            if (this.received)
            {
                this.received = false;
                result = com.Read();
            }

            if (this.watchDogTimerTick)
            {
                this.watchDogTimerTick = false;
                throw new InvalidOperationException(string.Format("No answer from {0}", com.PortName));
            }
            else
            {
                this.watchDogTimerTick = false;
                this.watchDogTimer.Stop();
            }

            return result;
        }

        private void WDTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            this.watchDogTimerTick = true;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.received = true;
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
    }
}
