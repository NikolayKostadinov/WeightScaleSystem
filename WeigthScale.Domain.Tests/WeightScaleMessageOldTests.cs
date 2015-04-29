using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeigthScale.Domain.Tests
{
    [TestClass]
    public class WeightScaleMessageOldTests
    {
        [TestMethod]
        public void Check_If_Valid_WeightScaleMessageOld_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 0;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount);
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageOld_With_Invalid_ProductName_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();
            message.ProductName = string.Empty; // Set ProductName to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "ProductName";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The ProductName cannot be empty.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageOld_With_TotalOfGrossWeight_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();
            message.TotalOfGrossWeight = -876; // Set TotalOfGrossWeight to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalOfGrossWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalOfGrossWeight must be between 0 and 999999999. The actual value is -876.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageOld_With_TotalOfGrossWeight_Grether_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();
            message.TotalOfGrossWeight = 1000000000; // Set TotalOfGrossWeight to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalOfGrossWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalOfGrossWeight must be between 0 and 999999999. The actual value is 1000000000.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageOld_With_TotalOfNetWeight_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();
            message.TotalOfNetWeight = -876; // Set TotalOfNetWeight to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalOfNetWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalOfNetWeight must be between 0 and 999999999. The actual value is -876.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageOld_With_TotalOfNetWeight_Grether_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageOld message = GetValidWeightScaleMessageOld();
            message.TotalOfNetWeight = 1000000000; // Set TotalOfNetWeight to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalOfNetWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalOfNetWeight must be between 0 and 999999999. The actual value is 1000000000.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        /// <summary>
        /// Gets the valid weight scale message old.
        /// </summary>
        /// <returns></returns>
        private WeightScaleMessageOld GetValidWeightScaleMessageOld()
        {
            return new WeightScaleMessageOld
            {
                Number = 1,
                Direction = Direction.In,
                TimeOfFirstMeasure = DateTime.Now,
                TimeOfSecondMeasure = DateTime.Now.AddSeconds(1),
                MeasurementStatus = 0,
                SerialNumber = 12345,
                TransactionNumber = 200,
                MeasurementNumber = 1,
                ProductCode = 202,
                TareWeight = 10000,
                GrossWeight = 25000,
                NetWeight = 15000,
                Vehicle = "A3325KX",
                DocumentNumber = 12345,
                ProductName = "Otpadaci",
                TotalOfGrossWeight = 25000,
                TotalOfNetWeight = 15000
            };
        }
    }
}
