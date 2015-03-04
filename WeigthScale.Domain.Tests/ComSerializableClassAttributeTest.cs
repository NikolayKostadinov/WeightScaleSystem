using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightScale.Domain.Common;

namespace WeigthScale.Domain.Tests
{
    [TestClass]
    public class ComSerializableClassAttributeTest
    {

        private class Result
        {
            public bool IsOk { get; set; }
            public string ClassName { get; set; }
            public string PropName { get; set; }

        }

        [TestMethod]
        public void Check_For_Properly_Decorate_Objects()
        {
            var types = Assembly.GetAssembly(typeof(ComSerializableClassAttribute)).GetTypes().Where(x => (x.GetCustomAttributes<ComSerializableClassAttribute>().Count() > 0));

            var res = IsAllPropertiesDecorated(types);

            Assert.IsTrue(res.IsOk, string.Format("Property {0} in class {1} is not decorated properly.", res.PropName, res.ClassName));
        }

        private Result IsAllPropertiesDecorated(IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                var properties = item.GetProperties();

                foreach (var prop in properties)
                {
                    if (!prop.IsDefined(typeof(ComSerializablePropertyAttribute), true))
                    {
                        if (!(prop.IsDefined(typeof(ComNonSerializablePropertyAttribute), true)))
                        {
                            return new Result() { IsOk = false, ClassName = item.Name, PropName = prop.Name };
                        }
                    }
                }
            }

            return new Result() { IsOk = true };
        }
    }
}
