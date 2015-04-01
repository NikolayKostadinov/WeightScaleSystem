//---------------------------------------------------------------------------------
// <copyright file="ValidationMessage.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System.Linq;
    using System.Text;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Represents the level of a validation error.
    /// </summary>
    public enum MessageType
    {
        Info = 0,

        Warning = 1,

        Error = 2
    }

    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public class ValidationMessage
    {
        private MessageType type;
        private string field;
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage" /> class. 
        /// Constructor that creates a validation error with the specified type, field and message.
        /// </summary>
        /// <param name="type">The type of the error.</param>
        /// <param name="field">The field that this message relates to.</param>
        /// <param name="text">The error message.</param>
        public ValidationMessage(MessageType type, string field, string text)
        {
            this.type = type;
            this.field = field;
            this.text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage" /> class. 
        /// Constructor that creates a validation error with the specified type and message.
        /// </summary>
        /// <param name="type">The type of the error.</param>
        /// <param name="text">The error message.</param>
        public ValidationMessage(MessageType type, string text)
        {
            this.type = type;
            this.text = text;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public MessageType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets or sets the field that this validation message relates to.
        /// </summary>
        public string Field
        {
            get { return this.field; }
            set { this.field = value; }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// Returns whether this is equal to the specified ValidationMessage.
        /// </summary>
        /// <param name="obj">The ValidationMessage to compare to.</param>
        /// <returns>True if they are equal.</returns>
        public override bool Equals(object obj)
        {
            ValidationMessage validationMessage = (ValidationMessage)obj;
            return validationMessage.Type  == this.type
                && validationMessage.Text  == this.text
                && validationMessage.Field == this.field;
        }

        /// <summary>
        /// Returns a HashCode.
        /// </summary>
        /// <returns>The HashCode.</returns>
        public override int GetHashCode()
        {
            return this.type.GetHashCode()
                ^ (this.field == null ? 0 : this.field.GetHashCode())
                ^ (this.text == null ? 0 : this.text.GetHashCode());
        }

        public override string ToString()
        {
            return this.GetProps();
        }

        protected string GetProps()
        {
            var props = this.GetType()
                           .GetProperties();
                         

            StringBuilder sb = new StringBuilder();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(ComSerializablePropertyAttribute), true).FirstOrDefault() as ComSerializablePropertyAttribute;
                var length = attr.Length;
                sb.Append((prop.GetValue(this) ?? string.Empty).ToString().PadLeft(length, ' '));
                sb.Append(" | ");
            }

            return sb.ToString();
        }
    }
}
