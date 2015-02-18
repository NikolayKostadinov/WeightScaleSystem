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
    }
}
