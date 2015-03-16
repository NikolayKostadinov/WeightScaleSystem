//---------------------------------------------------------------------------------
// <copyright file="NewWeightScaleMessage.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Concrete
{
    using System;
    using System.Linq;
    using System.Text;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Class for transportation of messages from and to Weight Scales communicating over new protocol
    /// </summary>
    [ComSerializableClass(BlockLen.NewProtocol)]
    public class WeightScaleMessageNew : WeightScaleMessageBase
    {
        private const int EXCISE_DOC_NUM_LENGTH = 10;
        private const int TOTAL_NET_OF_INPUT_MIN = 0;
        private const int TOTAL_NET_OF_INPUT_MAX = 999999999;
        private const int TOTAL_NET_OF_OUTPUT_MIN = 0;
        private const int TOTAL_NET_OF_OUTPUT_MAX = 999999999;
        private const int TOTAL_NET_BY_PRODUCT_INPUT_MIN = 0;
        private const int TOTAL_NET_BY_PRODUCT_INPUT_MAX = 999999999;
        private const int TOTAL_NET_BY_PRODUCT_OUTPUT_MIN = 0;
        private const int TOTAL_NET_BY_PRODUCT_OUTPUT_MAX = 999999999;

        private string exciseDocumentNumber;
        private int? totalNetOfInput;
        private int? totalNetOfOutput;
        private int? totalNetByProductInput;
        private int? totalNetByProductOutput;

        /// <summary>
        /// Gets or Sets number of excise document
        /// </summary>
        [ComSerializableProperty(length: 12, offset: 54, originalType: typeof(string), serializeFormat: "")]
        public string ExciseDocumentNumber
        {
            get { return this.exciseDocumentNumber; }
            set { this.exciseDocumentNumber = value; }
        }

        [ComSerializableProperty(length: 9, offset: 84, originalType: typeof(int?), serializeFormat: "")]
        public int? TotalNetOfInput
        {
            get { return this.totalNetOfInput; }
            set { this.totalNetOfInput = value; }
        }

        [ComSerializableProperty(length: 9, offset: 93, originalType: typeof(int?), serializeFormat: "")]
        public int? TotalNetOfOutput
        {
            get { return this.totalNetOfOutput; }
            set { this.totalNetOfOutput = value; }
        }

        [ComSerializableProperty(length: 9, offset: 126, originalType: typeof(int), serializeFormat: "")]
        public int? TotalNetByProductInput
        {
            get { return this.totalNetByProductInput; }
            set { this.totalNetByProductInput = value; }
        }

        [ComSerializableProperty(length: 9, offset: 135, originalType: typeof(int?), serializeFormat: "")]
        public int? TotalNetByProductOutput
        {
            get { return this.totalNetByProductOutput; }
            set { this.totalNetByProductOutput = value; }
        }

        public override ValidationMessageCollection Validate()
        {
            var validationResult = new ValidationMessageCollection();
            validationResult.AddRange(base.Validate());

            // Validate ExciseDocumentNumber
            if (!string.IsNullOrEmpty(this.exciseDocumentNumber))
            {
                if (this.exciseDocumentNumber.Length != EXCISE_DOC_NUM_LENGTH)
                {
                    validationResult.AddError(
                        "ExciseDocumentNumber",
                        string.Format("ExciseDocumentNumber length must be {0} characters. The Actual value is {1}", EXCISE_DOC_NUM_LENGTH, this.exciseDocumentNumber.Length));
                }
            }

            // Validate TotalNetOfInput
            if (TOTAL_NET_OF_INPUT_MIN > this.totalNetOfInput || this.totalNetOfInput > TOTAL_NET_OF_INPUT_MAX)
            {
                string message = "The value of TotalNetOfInput must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalNetOfInput",
                    string.Format(message, TOTAL_NET_OF_INPUT_MIN, TOTAL_NET_OF_INPUT_MAX, this.totalNetOfInput));
            }

            // Validate ТotalNetOfOutput
            if (TOTAL_NET_OF_OUTPUT_MIN > this.totalNetOfOutput || this.totalNetOfOutput > TOTAL_NET_OF_OUTPUT_MAX)
            {
                string message = "The value of TotalNetOutput must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalNetOutput",
                    string.Format(message, TOTAL_NET_OF_OUTPUT_MIN, TOTAL_NET_OF_OUTPUT_MAX, this.totalNetOfOutput));
            }

            // Validate TotalNetByProductInput
            if (TOTAL_NET_BY_PRODUCT_INPUT_MIN > this.totalNetByProductInput || this.totalNetByProductInput > TOTAL_NET_BY_PRODUCT_INPUT_MAX)
            {
                string message = "The value of TotalNetByProductInput must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalNetByProductInput",
                    string.Format(message, TOTAL_NET_BY_PRODUCT_INPUT_MIN, TOTAL_NET_BY_PRODUCT_INPUT_MAX, this.totalNetByProductInput));
            }

            // Validate TotalNetByProductOutput
            if (TOTAL_NET_BY_PRODUCT_OUTPUT_MIN > this.totalNetByProductOutput || this.totalNetByProductOutput > TOTAL_NET_BY_PRODUCT_OUTPUT_MAX)
            {
                string message = "The value of TotalNetByProductOutput must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalNetByProductOutput",
                    string.Format(message, TOTAL_NET_BY_PRODUCT_OUTPUT_MIN, TOTAL_NET_BY_PRODUCT_OUTPUT_MAX, this.totalNetByProductOutput));
            }

            return validationResult;
        }

        /// <summary>
        /// Provides data block to be sent as Weight scale message block element.
        /// </summary>
        /// <returns>byte[] - Array of bytes</returns>
        public override byte[] ToBlock(IComSerializer serializer)
        {
            return serializer.Setialize(this);
        }

        public override string ToString()
        {
            return GetProps();
        }

        protected string GetProps()
        {
            var props = this.GetType()
                           .GetProperties()
                           .Where(x => x.CustomAttributes.Where(y => y.AttributeType == typeof(ComSerializablePropertyAttribute)).Count() != 0)
                           .OrderBy(x => ((x.GetCustomAttributes(typeof(ComSerializablePropertyAttribute), true).FirstOrDefault()) as ComSerializablePropertyAttribute).Offset);

            StringBuilder sb = new StringBuilder();
            foreach (var prop in props)
            {
                var attr =prop.GetCustomAttributes(typeof(ComSerializablePropertyAttribute), true).FirstOrDefault() as ComSerializablePropertyAttribute;
                var length = attr.Length;
                sb.Append((prop.GetValue(this)??string.Empty).ToString().PadLeft(length,' '));
                sb.Append(" | ");
            }
            return sb.ToString();
        }
    }
}