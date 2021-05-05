//  ------------------------------------------------------------------------------------------------
//   <copyright file="IRepository.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.CacheApi.Contract
{
    #region Using

    using System.Collections.Generic;
    using System.Threading.Tasks;

    #endregion

    public interface IRepository<TEntity, TResult>
    {
        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        IEnumerable<string> GetTargetUrls();

        Task<IEnumerable<string>> GetTargetUrlsAsync();

        IEnumerable<TResult> Update(TEntity message);

        Task<IEnumerable<TResult>> UpdateAsync(TEntity message);
    }
}