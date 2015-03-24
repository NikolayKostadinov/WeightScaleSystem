//---------------------------------------------------------------------------------
// <copyright file="IComSerializer.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Abstract
{
    using System;

    /// <summary>
    /// Provides obligation for implementation of Serialize and Deserialize methods.
    /// </summary>
    public interface IComSerializer
    {
        T Deserialize<T>(byte[] input) where T : class, IComSerializable, new();

        byte[] Setialize<T>(T serObj) where T : class, IComSerializable;
    }
}