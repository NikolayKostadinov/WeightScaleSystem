//---------------------------------------------------------------------------------
// <copyright file="ComNonSerializablePropertyAttribute.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false,Inherited=true)]
    public class ComNonSerializablePropertyAttribute:Attribute
    {
    }
}
