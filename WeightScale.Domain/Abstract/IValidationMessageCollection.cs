using System;
using System.Linq;
using System.Collections.Generic;
using WeightScale.Domain.Common;

namespace WeightScale.Domain.Abstract
{
    public interface IValidationMessageCollection:IEnumerable<ValidationMessage>
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds collection of validation messages.
        /// </summary>
        /// <param name="validate">The validate.</param>
        void AddMany(IValidationMessageCollection validationResult);

        void AddError(string field, string text);

        void AddError(string text);

        void AddWarning(string text);
        bool ContainsError(string field, string text);
        System.Collections.Generic.List<ValidationMessage> Errors { get; }
        System.Collections.Generic.List<ValidationMessage> Infos { get; }
        System.Collections.Generic.List<ValidationMessage> Warnings { get; }
    }
}
