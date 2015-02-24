//---------------------------------------------------------------------------------
// <copyright file="ComSerializablePropertyAttribute.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System;
    using System.Linq;
    using WeightScale.Utility.Helpers;

    /// <summary>
    /// Attribute. Allow IComSerializable classes to specify format for serialization of their property to byte array
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComSerializablePropertyAttribute : Attribute
    {
        private int length;
        private int offset;
        private Type originalType;
        private string serializeFormat;
        private Alignment align;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComSerializablePropertyAttribute" /> class.
        /// </summary>
        /// <param name="length">length of property in bytes</param>
        /// <param name="offset">offset in the result byte array in bytes</param>
        /// <param name="originalType">original type of property as Type</param>
        /// <param name="serializeFormat">string formatter f.e. "d2" -> 01</param>
        /// <param name="align">Left or right alignment. Default is right.</param>
        public ComSerializablePropertyAttribute(
            int length,
            int offset,
            Type originalType,
            string serializeFormat,
            Alignment align = Alignment.Right)
        {
            this.Length = length;
            this.Offset = offset;
            this.OriginalType = originalType;
            this.SerializeFormat = serializeFormat;
            this.Align = align;
        }

        public int Length
        {
            get
            {
                return this.length;
            }

            private set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Length", string.Format("Length must be greater than 1. Actual Length is {0}", value));
                }

                this.length = value;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Offset", string.Format("Offset must be greater than or equal 0. Actual Offset is {0}", value));
                }

                this.offset = value;
            }
        }

        public Type OriginalType
        {
            get
            {
                return this.originalType;
            }

            private set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("SerializeFormat", "SerializeFormat cannot be null.");
                }

                this.originalType = value;
            }
        }

        public string SerializeFormat
        {
            get
            {
                return this.serializeFormat;
            }

            private set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("SerializeFormat", "SerializeFormat cannot be null.");
                }

                this.serializeFormat = value;
            }
        }

        public Alignment Align
        {
            get
            {
                return this.align;
            }

            private set
            {
                if (!Enum.IsDefined(typeof(Alignment), value))
                {
                    throw new ArgumentOutOfRangeException("Align", "Align must be Alignment.Left or Alignment.Right.");
                }

                this.align = value;
            }
        }
    }
}
