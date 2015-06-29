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
    using Newtonsoft.Json;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Collection container for validation messages.
    /// </summary>
    public class ValidationMessageCollection : List<ValidationMessage>, IValidationMessageCollection
    {
        public ValidationMessageCollection()
        {
            //Do nothing!!!
        }

        [JsonConstructor]
        public ValidationMessageCollection(IEnumerable<ValidationMessage> inputMessages)
        {
            if (inputMessages != null)
            {
                this.AddRange(inputMessages);
            }
        }
        /// <summary>
        /// Gets a list of messages with the status 'Info'.
        /// </summary>
        public IValidationMessageCollection Infos
        {
            get
            {
                return new ValidationMessageCollection(this.Where(x => x.Type == MessageType.Info));
            }
        }

        /// <summary>
        /// Gets a list of messages with the status 'Warning'.
        /// </summary>
        public IValidationMessageCollection Errors
        {
            get
            {
                return new ValidationMessageCollection(this.Where(x => x.Type == MessageType.Error));
            }
        }

        /// <summary>
        /// Gets a list of messages with the status 'Error'.
        /// </summary>
        public IValidationMessageCollection Warnings
        {
            get
            {
                return new ValidationMessageCollection(this.Where(x => x.Type == MessageType.Warning));
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
        /// <param name="validationResult">Collection of validation messages</param>
        public void AddMany(IValidationMessageCollection validationResult)
        {
            this.AddRange(validationResult);
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
