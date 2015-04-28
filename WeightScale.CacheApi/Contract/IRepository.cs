using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightScale.CacheApi.Contract
{
    public interface IRepository<T,R>
    {
        Task<IEnumerable<T>> GetAllAsynk();

        IEnumerable<T> GetAll();

        Task<IEnumerable<string>> GetTargetUrlsAsync();

        IEnumerable<string> GetTargetUrls();

        Task<IEnumerable<R>> UpdateAsync(T message);

        IEnumerable<R> Update(T message);
    }
}
