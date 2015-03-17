//---------------------------------------------------------------------------------
// <copyright file="IValidationMessageCollection.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WeightScale.Domain.Common;

    public interface IValidationMessageCollection : IEnumerable<ValidationMessage>
    {
        System.Collections.Generic.List<ValidationMessage> Errors { get; }

        System.Collections.Generic.List<ValidationMessage> Infos { get; }

        System.Collections.Generic.List<ValidationMessage> Warnings { get; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the many.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        void AddMany(IValidationMessageCollection validationResult);

        void AddError(string field, string text);

        void AddError(string text);

        void AddWarning(string text);

        bool ContainsError(string field, string text);
    }
}
