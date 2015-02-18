//---------------------------------------------------------------------------------
// <copyright file="StringExtentions.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------

namespace WeightScale.ComunicationProtocol.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// Alignment of result byte area
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// Left alignment
        /// </summary>
        Left,

        /// <summary>
        /// Right Alignment
        /// </summary>
        Right
    }

    /// <summary>
    /// Provides extension methods to string class
    /// </summary>
    public static class StringExtentions
    {
        public static byte[] ToByteArray(this string str)
        {
            byte[] result = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                result[i] = Convert.ToByte(str[i]);
            }

            return result;
        }

        public static byte[] ToByteArray(this string str, Alignment align, int lenght)
        {
            if (str.Length > lenght)
            {
                throw new ArgumentException(
                    string.Format(
                    "The lenght of output byte array must be greater than or equal to lenght of string. Lengt of string: {0}, Lenght of array: {1} .", 
                    str.Length, 
                    lenght));
            }

            string alignedString = str;
            switch (align)
            {
                case Alignment.Left:
                    alignedString = alignedString.PadRight(lenght, ' ');
                    break;
                case Alignment.Right:
                    alignedString = alignedString.PadLeft(lenght, ' ');
                    break;
                default:
                    throw new ArgumentException("Invalid argument \"align\". Must be Alignment.Legt or Alignment.Right");
            }

            byte[] result = alignedString.ToByteArray();
            return result;
        }
    }
}