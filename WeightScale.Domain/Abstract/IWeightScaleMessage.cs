using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightScale.Domain.Abstract
{
    public interface IWeightScaleMessage : IValidateable, IComSerializable, IBlock
    {
        string ToString();
    }
}
