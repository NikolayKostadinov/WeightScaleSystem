//---------------------------------------------------------------------------------
// <copyright file="StringExtentions.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Utility.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

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
        /// <summary>
        /// Converts string into byte array, which consists of single byte ASCII code for every single character in input string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>byte array</returns>
        public static byte[] ToByteArray(this string str)
        {
            if (str == null)
            {
                throw new ArgumentException("The input string cannot be empty.");
            }

            byte[] strToByte = Encoding.Default.GetBytes(str);
            return strToByte;
        }

        /// <summary>
        /// Converts string into byte array with fixed length, aligned in intended direction, which consists of single byte ASCII code for every single character in input string, padded with necessary number of spaces.
        /// </summary>
        /// <param name="str">Input string;</param>
        /// <param name="align">Direction of the alignment;</param>
        /// <param name="lenght">Desired length of the output array.</param>
        /// <returns>byte array</returns>
        public static byte[] ToByteArray(this string str, Alignment align, int lenght)
        {
            if (str == null)
            {
                throw new ArgumentException("The input string cannot be empty.");
            }

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
                    throw new ArgumentException("Invalid argument \"align\". Must be Alignment.Left or Alignment.Right");
            }

            byte[] result = alignedString.ToByteArray();
            return result;
        }
    }
}