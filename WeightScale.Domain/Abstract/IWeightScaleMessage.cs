//---------------------------------------------------------------------------------
// <copyright file="IWeightScaleMessage.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Abstract
{
    using System;
    using System.Linq;

    public interface IWeightScaleMessage : IValidateable, IComSerializable, IBlock
    {
        string ToString();
    }
}
