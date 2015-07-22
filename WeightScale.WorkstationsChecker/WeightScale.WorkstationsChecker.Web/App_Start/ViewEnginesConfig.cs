namespace WeightScale.WorkstationsChecker.Web.App_Start
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    public static class ViewEnginesConfig
    {
        /// <summary>
        /// Registers the view engines.
        /// </summary>
        /// <param name="engines">The engines.</param>
        public static void RegisterViewEngines(ViewEngineCollection engines)
        {
            engines.Clear();
            engines.Add(new RazorViewEngine());
        }
    }
}