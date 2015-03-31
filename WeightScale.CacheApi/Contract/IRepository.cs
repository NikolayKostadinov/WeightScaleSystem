using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightScale.CacheApi.Contract
{
    public interface IRepository<T,R>
    {
        IEnumerable<T> GetAll();
        IEnumerable<R> Update(T message);
    }
}
