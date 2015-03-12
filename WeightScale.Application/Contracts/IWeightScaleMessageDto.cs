using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.Domain.Abstract;

namespace WeightScale.Application.Contracts
{
    public interface IWeightScaleMessageDto
    {
        IWeightScaleMessage Message { get; set; }
        IValidationMessageCollection ValidationMessages { get; set; }
    }
}
