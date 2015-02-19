using System;
using System.Linq;

namespace WeightScale.Domain.Abstract
{
    public enum Direction
    {
        In = 1,
        Out = 2
    }

    public abstract class WeightScaleMessageBase
    {
        private const int NUMBER_MIN_VALUE = 1;
        private const int NUMBER_MAX_VALUE = 99;
        private const int DOC_NUM_MIN_VALUE = 1;
        private const int DOC_NUM_MAX_VALUE = 99999999;
        private const int TR_NUM_MIN_VALUE = 1;
        private const int TR_NUM_MAX_VALUE = 99999;
        private const byte M_NUM_MIN_VALUE = 1;
        private const byte M_NUM_MAX_VALUE = 2;

        private int number;
        private Direction direction;
        private DateTime timeOfTareMeasure;
        private DateTime timeOfGrossMeasure;
        private MeasurementStatus measurementStatus;
        private int documentNumber;
        private int transactionNumber;
        private byte measurementNumber;
        private long productCode;

        /// <summary>
        /// Two bytes numeric literal
        /// 01 or 11 or 23 or....
        /// </summary>
        public int Number
        {
            get
            {
                return this.number;
            }
            set
            {
                if (value < NUMBER_MIN_VALUE || value > NUMBER_MAX_VALUE)
                {
                    throw new ArgumentOutOfRangeException(
                        "Number",
                        string.Format("Argument \"Number\" must be between {0} and {1}.",
                            NUMBER_MIN_VALUE,
                            NUMBER_MAX_VALUE));
                }
                this.number = value;
            }
        }

        /// <summary>
        /// single charater 1 or 2
        /// </summary>
        public Direction Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                if (Direction.In > value || value < Direction.Out)
                {
                    throw new ArgumentOutOfRangeException("Direction", "The value of direction must be Direction.In of Direction.Out");
                }
                this.direction = value;
            }
        }

        /// <summary>
        /// YYMMDDHHMMSS
        /// </summary>
        public DateTime TimeOfTareMeasute
        {
            get
            {
                return this.timeOfTareMeasure;
            }
            set
            {
                this.timeOfTareMeasure = value;
            }
        }

        /// <summary>
        /// YYMMDDHHMMSS
        /// </summary>
        public DateTime TimeOfGrossMeasute
        {
            get
            {
                return this.timeOfGrossMeasure;
            }
            set
            {
                this.timeOfGrossMeasure = value;
            }
        }

        /// <summary>
        /// Enumeration of measurement statuses 1 byte 
        /// </summary>
        public MeasurementStatus MeasurementStatus
        {
            get
            {
                return this.measurementStatus;
            }

            set
            {
                if (Enum.IsDefined(typeof(MeasurementStatus), value))
                {
                    throw new ArgumentOutOfRangeException(
                        "MeasurementStatus",
                        string.Format(
                            "The stastus {0} is out of range. Acceptable statuses are between 0 and 9.",
                            value));
                }
                this.measurementStatus = value;
            }
        }

        /// <summary>
        /// Eight bytes number
        /// 1 or 99999999
        /// </summary>
        public int DocumentNumber
        {
            get
            {
                return this.documentNumber;
            }
            set
            {
                if (DOC_NUM_MIN_VALUE < value || value > DOC_NUM_MAX_VALUE)
                {
                    throw new ArgumentOutOfRangeException(
                        "DocumentNumber",
                        string.Format("DocumentNumber {0} out of range. Document number must be between {1} and {2}",
                            value,
                            DOC_NUM_MIN_VALUE,
                            DOC_NUM_MAX_VALUE));
                }

                this.documentNumber = value;
            }
        }

        /// <summary>
        /// Eight bytes number
        /// 1 or 99999
        public int TransactionNumber
        {
            get
            {
                return this.transactionNumber;
            }
            set
            {
                if (TR_NUM_MIN_VALUE > value || value < TR_NUM_MAX_VALUE)
                {
                    throw new ArgumentOutOfRangeException(
                        "TransactionNumber",
                        string.Format(
                            "Transaction number {0} is out of range. Transaction number must be between {1} and {2}.",
                            value,
                            TR_NUM_MIN_VALUE, TR_NUM_MAX_VALUE));
                }

                this.transactionNumber = value;
            }
        }

        public byte MeasurementNumber
        {
            get
            {
                return this.measurementNumber;
            }
            set
            {
                if (M_NUM_MIN_VALUE < measurementNumber || measurementNumber > M_NUM_MAX_VALUE)
                {
                    throw new ArgumentOutOfRangeException(
                        "MeasurementNumber",
                        string.Format(
                            "MeasurementNumber {0} is invalid. MeasurementNumber must be between {1} and {2}.",
                            value,
                            M_NUM_MIN_VALUE,
                            M_NUM_MAX_VALUE));
                }

                this.measurementNumber = value;
            }
        }

        public long ProdunctCode 
        {
            get 
            {
                return this.productCode;
            } 
            set
            {
                this.productCode = value;
            } 
        }
    }
}