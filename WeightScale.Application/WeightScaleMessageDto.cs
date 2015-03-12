using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.Application.Contracts;
using WeightScale.Domain.Abstract;

namespace WeightScale.Application
{
    public class WeightScaleMessageDto : IWeightScaleMessageDto
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public IWeightScaleMessage Message { get; set; }


        /// <summary>
        /// Gets or sets the validation messages.
        /// </summary>
        /// <value>The validation messages.</value>
        public IValidationMessageCollection ValidationMessages { get; set; }
    }
}
