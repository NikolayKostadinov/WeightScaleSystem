using System;
namespace WeightScale.Domain.Abstract
{
    interface IWeightScaleMessage
    {
        byte[] ToBlock();
        WeightScale.Domain.Common.ValidationMessageCollection Validate();
    }
}
