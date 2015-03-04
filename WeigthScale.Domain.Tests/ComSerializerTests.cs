using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeigthScale.Domain.Tests
{
    [TestClass]
    public class ComSerializerTests
    {

        WeightScaleMessageNew serializable = new WeightScaleMessageNew()
        {
            Number = 1,
            Direction = Direction.In,
            TimeOfFirstMeasure = DateTime.Now.AddDays(-1),
            TimeOfSecondMeasure = DateTime.Now,
            MeasurementStatus = MeasurementStatus.OK,
            SerialNumber = 12345678,
            TransactionNumber = 12345,
            MeasurementNumber = 1,
            ProductCode = 123456789012,
            ExciseDocumentNumber = "0123456789",
            TareWeight = 1000,
            GrossWeight = 2000,
            NetWeight = 1000,
            TotalNetOfInput = 10000,
            TotalNetOfOutput = 20000,
            Vehicle = "Test",
            DocumentNumber = 12345,
            TotalNetByProductInput = 12345678,
            TotalNetByProductOutput = 123456789
        };

        [TestMethod]
        public void SerializerTest()
        {
            //Arrange
            IComSerializer serializer = new ComSerializer();
            //Act
            var serialized = serializer.Setialize(serializable);
            var deserialized = serializer.Deserialize<WeightScaleMessageNew>(serialized);
            //Assert
            var expectedProps = serializable.GetType().GetProperties();

            foreach (var prop in expectedProps)
            {
                var expectedValue = prop.GetValue(serializable);
                var actualValue = prop.GetValue(deserialized);
                if (prop.PropertyType == typeof(DateTime?))
                {
                    Assert.AreEqual(string.Format("{0:yyyyMMddHHmmss}", expectedValue), string.Format("{0:yyyyMMddHHmmss}", actualValue), 
                        string.Format("The expected value of the property {0} is {1} but actual value is {2}", prop.Name, expectedValue, actualValue));
                }
                else
                {
                    Assert.AreEqual(expectedValue, actualValue,
                        string.Format("The expected value of the property {0} is {1} but actual value is {2}", prop.Name, expectedValue, actualValue));
                }
            }

        }
    }
}
