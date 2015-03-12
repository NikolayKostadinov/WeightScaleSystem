using System;
using System.Linq;
using System.Collections.Generic;
using WeightScale.Domain.Common;

namespace WeightScale.Domain.Abstract
{
    public interface IValidationMessageCollection
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="validate">The validate.</param>
        void AddRange(IValidationMessageCollection validationResult);

        void AddError(string field, string text);

        void AddError(string text);

        void AddWarning(string text);
        bool ContainsError(string field, string text);
        System.Collections.Generic.List<ValidationMessage> Errors { get; }
        System.Collections.Generic.List<ValidationMessage> Infos { get; }
        System.Collections.Generic.List<ValidationMessage> Warnings { get; }
    }
}
