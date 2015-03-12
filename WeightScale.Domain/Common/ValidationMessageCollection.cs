//---------------------------------------------------------------------------------
// <copyright file="ValidationMessageCollection.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Collection container for validation messages.
    /// </summary>
    public class ValidationMessageCollection : List<ValidationMessage>, IValidationMessageCollection
    {
        /// <summary>
        /// Gets a list of messages with the status 'Info'.
        /// </summary>
        public List<ValidationMessage> Infos
        {
            get
            {
                return (from validationMessage
                        in this
                        where validationMessage.Type == MessageType.Info
                        select validationMessage).ToList();
            }
        }

        /// <summary>
        /// Gets a list of messages with the status 'Warning'.
        /// </summary>
        public List<ValidationMessage> Errors
        {
            get
            {
                return (from validationMessage
                        in this
                        where validationMessage.Type == MessageType.Error
                        select validationMessage).ToList();
            }
        }

        /// <summary>
        /// Gets a list of messages with the status 'Error'.
        /// </summary>
        public List<ValidationMessage> Warnings
        {
            get
            {
                return (from validationMessage
                        in this
                        where validationMessage.Type == MessageType.Warning
                        select validationMessage).ToList();
            }
        }

        /// <summary>
        /// Adds a ValidationMessage with type 'Error' and the specified text. 
        /// </summary>
        /// <param name="text">The text for the ValidationMessage.</param>
        public void AddError(string text)
        {
            this.Add(new ValidationMessage(MessageType.Error, text));
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddRange(IValidationMessageCollection validationResult)
        {
            foreach (var item in validationResult as List<ValidationMessage>)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Adds a ValidationMessage with type 'Error' and the specified field and text. 
        /// </summary>
        /// <param name="field">The field the ValidationMessage relates to.</param>
        /// <param name="text">The text for the ValidationMessage.</param>
        public void AddError(string field, string text)
        {
            this.Add(new ValidationMessage(MessageType.Error, field, text));
        }

        /// <summary>
        /// Adds a ValidationMessage with type 'Warning' and the specified text. 
        /// </summary>
        /// <param name="text">The text for the ValidationMessage.</param>
        public void AddWarning(string text)
        {
            this.Add(new ValidationMessage(MessageType.Warning, text));
        }

        /// <summary>
        /// Checks whether the collection contains a ValidationMessage with type 'Error' and
        /// the specified field and text.
        /// </summary>
        /// <param name="field">The field the ValidationMessage relates to.</param>
        /// <param name="text">The text for the ValidationMessage.</param>
        /// <returns>True if a ValidationMessage is found in this collection.</returns>
        public bool ContainsError(string field, string text)
        {
            return this.Contains(new ValidationMessage(MessageType.Error, field, text));
        }
    }
}
