//  ------------------------------------------------------------------------------------------------
//   <copyright file="ConnectionParameters.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.MeasurementsClient
{
    using WeightScale.CacheApi.Contract;

    public class ConnectionParameters : IConnectionParameters
    {
        public string UserName
        {
            get
            {
                return Properties.Settings.Default.UserName;
            }
        }

        public string Password
        {
            get
            {
                return Properties.Settings.Default.Password;
            }
        }
    }
}