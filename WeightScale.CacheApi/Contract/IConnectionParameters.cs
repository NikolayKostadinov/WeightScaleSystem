//  ------------------------------------------------------------------------------------------------
//   <copyright file="IConnectionParameters.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.CacheApi.Contract
{
    public interface IConnectionParameters
    {
        string UserName { get; }

        string Password { get; }
    }
}