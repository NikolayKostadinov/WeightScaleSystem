//---------------------------------------------------------------------------------
// <copyright file="ComSerializableClassAttribute.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// current size of the result byte array depending of type of the communication protocol
    /// </summary>
    public enum BlockLen : int
    {
        OldProtocol = 126,
        NewProtocol = 144,
        NewOverflowProtocol=149,
    }

    /// <summary>
    /// Attribute. Allow IComSerializable classes to specify size of their output byte array
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ComSerializableClassAttribute : Attribute
    {
        private int blockLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComSerializableClassAttribute" /> class.
        /// </summary>
        /// <param name="blockLength">The length of output byte array.</param>
        public ComSerializableClassAttribute(BlockLen blockLength)
        {
            this.BlockLength = blockLength;
        }

        public BlockLen BlockLength
        {
            get
            {
                return (BlockLen)this.blockLength;
            }

            private set
            {
                if (!Enum.IsDefined(typeof(BlockLen), value))
                {
                    string message =
                        "BlockLength must be \"BlockLength.OldProtocol\" or \"BlockLength.NewProtocol\". Actual value is {0}";
                    throw new ArgumentOutOfRangeException("BlockLength", string.Format(message, value));
                }

                this.blockLength = (int)value;
            }
        }
    }
}
