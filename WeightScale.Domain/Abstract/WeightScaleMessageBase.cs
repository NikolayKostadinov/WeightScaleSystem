//---------------------------------------------------------------------------------
// <copyright file="WeightScaleMessageBase.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Abstract
{
    using System;
    using System.Linq;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Measurement Status
    /// </summary>
    public enum MeasurementStatus
    {
        OK = 0,
        TheWeightIsOverHighestAcceptable = 1,
        ProtocolPrinterFailure = 2,
        TheWeightScaleRamOverflow = 3,
        CalibrationMemoryFailure = 4,
        CalibrationMemoryIsUnderInitialization = 5,
        TheTransactionIsAlreadyFinished = 6,
        TheWeightIsUnderLowestAcceptable = 7,
        TheCarIsOutsideWeightScalePlatform = 8,
        TotalizerIsOverflow = 9
    }

    /// <summary>
    /// Direction of the shipment
    /// </summary>
    public enum Direction
    {
        In = 1,
        Out = 2
    }

    /// <summary>
    /// Abstract class. Base class for transportation of messages from and to Weight Scales
    /// </summary>
    public abstract class WeightScaleMessageBase : IValidateable, IComSerializable,IBlock, IWeightScaleMessage
    {
        private const int NUMBER_MIN_VALUE = 1;
        private const int NUMBER_MAX_VALUE = 99;
        private const int SER_NUM_MIN_VALUE = 1;
        private const int SER_NUM_MAX_VALUE = 99999999;
        private const int TR_NUM_MIN_VALUE = 1;
        private const int TR_NUM_MAX_VALUE = 99999;
        private const byte M_NUM_MIN_VALUE = 1;
        private const byte M_NUM_MAX_VALUE = 2;

        private byte number;
        private Direction direction;
        private DateTime? timeOfFirstMeasure;
        private DateTime? timeOfSecondMeasure;
        private MeasurementStatus measurementStatus;
        private int serialNumber;
        private int transactionNumber;
        private byte measurementNumber;
        private long productCode;
        private int? netWeight;
        private int? grossWeight;
        private int? tareWeight;
        private string vehicle;
        private int? documentNumber;

        /// <summary>
        /// Provides data block to be sent as Weight scale message block element.
        /// </summary>
        /// <returns>byte[] - Array of bytes</returns>
        public abstract byte[] ToBlock();

        /// <summary>
        /// Two bytes numeric literal
        /// 01 or 11 or 23 or....
        /// </summary>
        [ComSerializableProperty(length: 2, offset: 0, originalType: typeof(byte), serializeFormat: "d2")]
        public byte Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        /// <summary>
        /// single character 1 or 2
        /// </summary>
        [ComSerializableProperty(length: 1, offset: 2, originalType: typeof(int), serializeFormat: "d")]
        
        public Direction Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        [ComSerializableProperty(length: 12, offset: 3, originalType: typeof(DateTime), serializeFormat: "yyMMddHHmmss")]
        public DateTime? TimeOfFirstMeasure
        {
            get { return this.timeOfFirstMeasure; }
            set { this.timeOfFirstMeasure = value; }
        }

        [ComSerializableProperty(length: 12, offset: 15, originalType: typeof(DateTime), serializeFormat: "yyMMddHHmmss")]
        public DateTime? TimeOfSecondMeasure
        {
            get { return this.timeOfSecondMeasure; }
            set { this.timeOfSecondMeasure = value; }
        }

        /// <summary>
        /// Enumeration of measurement statuses 1 byte 
        /// </summary>
        [ComSerializableProperty(length: 1, offset: 27, originalType: typeof(int), serializeFormat: "d")]
        public MeasurementStatus MeasurementStatus
        {
            get { return this.measurementStatus; }
            set { this.measurementStatus = value; }
        }

        /// <summary>
        /// Eight bytes number
        /// 1 or 99999999
        /// </summary>
        [ComSerializableProperty(length: 8, offset: 28, originalType: typeof(int), serializeFormat: "")]
        public int SerialNumber
        {
            get { return this.serialNumber; }
            set { this.serialNumber = value; }
        }

        /// <summary>
        /// Eight bytes number
        /// 1 or 99999
        /// </summary>
        [ComSerializableProperty(length: 5, offset: 36, originalType: typeof(int), serializeFormat: "")]
        public int TransactionNumber
        {
            get { return this.transactionNumber; }
            set { this.transactionNumber = value; }
        }

        [ComSerializableProperty(length: 1, offset: 41, originalType: typeof(byte), serializeFormat: "")]
        public byte MeasurementNumber
        {
            get { return this.measurementNumber; }
            set { this.measurementNumber = value; }
        }

        [ComSerializableProperty(length: 12, offset: 42, originalType: typeof(long), serializeFormat: "")]
        public long ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
        }

        [ComSerializableProperty(length: 6, offset: 66, originalType: typeof(int?), serializeFormat: "")]
        public int? TareWeight
        {
            get { return this.tareWeight; }
            set { this.tareWeight = value; }
        }

        [ComSerializableProperty(length: 6, offset: 72, originalType: typeof(int?), serializeFormat: "")]
        public int? GrossWeight
        {
            get { return this.grossWeight; }
            set { this.grossWeight = value; }
        }

        [ComSerializableProperty(length: 6, offset: 78, originalType: typeof(int?), serializeFormat: "")]
        public int? NetWeight
        {
            get { return this.netWeight; }
            set { this.netWeight = value; }
        }

        [ComSerializableProperty(length: 12, offset: 102, originalType: typeof(string), serializeFormat: "")]
        public string Vehicle
        {
            get { return this.vehicle; }
            set { this.vehicle = value; }
        }

        [ComSerializableProperty(length: 12, offset: 114, originalType: typeof(int?), serializeFormat: "")]
        public int? DocumentNumber
        {
            get { return this.documentNumber; }
            set { this.documentNumber = value; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>Validation Message Collection</returns>
        public virtual ValidationMessageCollection Validate()
        {
            var validationResult = new ValidationMessageCollection();

            // Validating Number field
            if (this.number < NUMBER_MIN_VALUE || this.number > NUMBER_MAX_VALUE)
            {
                validationResult.AddError(
                    "Number",
                    string.Format("Argument \"Number\" must be between {0} and {1}.", NUMBER_MIN_VALUE, NUMBER_MAX_VALUE));
            }

            // Validating Direction field
            if (!Enum.IsDefined(typeof(Direction), this.direction))
            {
                validationResult.AddError("Direction", "The value of direction must be Direction.In of Direction.Out");
            }

            // Validate MeasurementStatus
            if (!Enum.IsDefined(typeof(MeasurementStatus), this.measurementStatus))
            {
                string message = "The stastus {0} is out of range. Acceptable statuses are between 0 and 9.";
                validationResult.AddError(
                    "MeasurementStatus",
                    string.Format(message, this.measurementNumber));
            }

            // Validate DocumentNumber
            if (SER_NUM_MIN_VALUE < this.documentNumber || this.documentNumber > SER_NUM_MAX_VALUE)
            {
                string message = "DocumentNumber {0} out of range. Document number must be between {1} and {2}";
                validationResult.AddError(
                    "DocumentNumber",
                    string.Format(message, this.documentNumber, SER_NUM_MIN_VALUE, SER_NUM_MAX_VALUE));
            }

            // Validate TransactionNumber
            if (TR_NUM_MIN_VALUE > this.transactionNumber || this.transactionNumber > TR_NUM_MAX_VALUE)
            {
                string message = "Transaction number {0} is out of range. Transaction number must be between {1} and {2}.";
                validationResult.AddError(
                "TransactionNumber",
                string.Format(message, this.transactionNumber, TR_NUM_MIN_VALUE, TR_NUM_MAX_VALUE));
            }

            // Validate MeasurementNumber
            if (M_NUM_MIN_VALUE > this.measurementNumber || this.measurementNumber > M_NUM_MAX_VALUE)
            {
                string message = "MeasurementNumber {0} is invalid. MeasurementNumber must be between {1} and {2}.";
                validationResult.AddError(
                    "MeasurementNumber",
                    string.Format(message, this.measurementNumber, M_NUM_MIN_VALUE, M_NUM_MAX_VALUE));
            }

            // Validate TimeOfFirstMeasute against TimeOfSecondMeasure
            if (this.timeOfFirstMeasure != null && this.timeOfSecondMeasure != null)
            {
                if (this.timeOfSecondMeasure < this.TimeOfFirstMeasure)
                {
                    string message = @"Invalid timing. The TimeOfFirstMeasure must be before TimeOfSecondMeasure. 
Their actual values are TimeOfFirstMeasure: {0}, timeOfSecondMeasure:{1}";
                    validationResult.AddError(
                        "TimeOfFirstMeasure || TimeOfSecondMeasure",
                        string.Format(message, this.timeOfFirstMeasure, this.timeOfSecondMeasure));
                }
            }

            // Validate ProductCode
            if (this.productCode <= 0)
            {
                string message = "The ProductCode must be greater than 0. Actual value is {0}";
                validationResult.AddError("ProductCode", string.Format(message, this.productCode));
            }

            // Validate TareWeight
            if (this.tareWeight < 0)
            {
                string message = "The TareWeight must be positive number or 0. Actual value is {0}";
                validationResult.AddError(
                    "TareWeight",
                    string.Format(message, this.tareWeight));
            }

            // Validate GrossWeight
            if (this.grossWeight < 0)
            {
                string message = "The GrossWeight must be positive number or 0. Actual value is {0}";
                validationResult.AddError(
                    "GrossWeight",
                    string.Format(message, this.grossWeight));
            }

            // Validate NetWeight
            if (this.NetWeight < 0)
            {
                string message = "The NetWeight must be positive number or 0. Actual value is {0}";
                validationResult.AddError(
                    "NetWeight",
                    string.Format(message, this.netWeight));
            }

            // Validate GrossWeight against TareWaight
            if (this.grossWeight > 0 && this.tareWeight > 0)
            {
                if (this.grossWeight < this.tareWeight)
                {
                    string message = @"The GrossWeight must be greater than TareWeight number or 0. 
Actual values are GrossWeight: {0}; TareWaight: {1} ";
                    validationResult.AddError(
                        "GrossWeight || TareWaight",
                        string.Format(message, this.grossWeight, this.tareWeight));
                }
            }

            // Validate GrossWeight and TareWaight against NetWeight
            if (this.grossWeight > 0 && this.tareWeight > 0 && this.netWeight > 0)
            {
                if (this.netWeight != (this.grossWeight - this.tareWeight))
                {
                    var message = @"The NetWeight must be equal to GrossWeight - TareWeight. 
Actual valuse is NetWeight: {0}; GrossWeight{1}; TareWaight: {2};. 
So {0} is not equal to {3}";

                    validationResult.AddError(
                        "GrossWeight || TareWaight || NetWaight",
                        string.Format(message, this.netWeight, this.grossWeight, this.tareWeight, this.grossWeight - this.tareWeight));
                }
            }

            // Validate Vehicle
            if (string.IsNullOrEmpty(this.vehicle) || string.IsNullOrWhiteSpace(this.vehicle))
            {
                validationResult.AddError("Vehicle", "The Vehicle cannot be empty.");
            }

            // Validate DocumentNumber
            if (this.documentNumber < 0)
            {
                string message = "DocumentNumber must be greater than or equal to 0. Actual Document number is {0}";
                validationResult.AddError(
                    "DocumentNumber",
                    string.Format(message, this.documentNumber));
            }

            return validationResult;
        }
    }
}