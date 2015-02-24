//---------------------------------------------------------------------------------
// <copyright file="IValidateable.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WeightScale.Domain.Common;

    public interface IValidateable
    {
        ValidationMessageCollection Validate();
    }
}
