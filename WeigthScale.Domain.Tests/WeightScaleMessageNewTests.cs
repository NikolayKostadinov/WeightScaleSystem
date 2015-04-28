using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeigthScale.Domain.Tests
{
    [TestClass]
    public class WeightScaleMessageNewTests
    {

        [TestMethod]
        public void Check_If_Valid_WeightScaleMessageNew_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 0;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount);
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_Direction_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.Direction = 0; // Set Direction to invalid value
            
            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "Direction";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of direction must be Direction.In of Direction.Out";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_DateOfFirstIsgreaterThanDateOfSecondTime_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TimeOfFirstMeasure =DateTime.Now.AddSeconds(1) ;
            message.TimeOfSecondMeasure = DateTime.Now;
            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TimeOfFirstMeasure || TimeOfSecondMeasure";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = string.Format("Invalid timing. The TimeOfFirstMeasure must be before TimeOfSecondMeasure. \r\nTheir actual values are TimeOfFirstMeasure: {0}, timeOfSecondMeasure:{1}",message.TimeOfFirstMeasure,message.TimeOfSecondMeasure);

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_GrossWeightIsLessThanTareWeght_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.GrossWeight = 5000;
            message.TareWeight = 10000;
            
            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 2;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "GrossWeight || TareWaight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The GrossWeight must be greater than TareWeight number or 0. \r\nActual values are GrossWeight: 5000; TareWaight: 10000 ";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_MeasurementStatus_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.MeasurementStatus = (MeasurementStatus)10; // Set MeasurementStatus to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "MeasurementStatus";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The Status 10 is out of range. Acceptable statuses are between 0 and 9.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_Vehicle_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.Vehicle = string.Empty; // Set Vehicle to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "Vehicle";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The Vehicle cannot be empty.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_ProductCode_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.ProductCode = -987; // Set ProductCode to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "ProductCode";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The ProductCode must be greater than 0. Actual value is -987";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_TareWeight_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TareWeight = -987; // Set TareWeight to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TareWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The TareWeight must be positive number or 0. Actual value is -987";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_GrossWeight_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.GrossWeight = -987; // Set GrossWeight to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "GrossWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The GrossWeight must be positive number or 0. Actual value is -987";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_NetWeight_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.NetWeight = -987; // Set NetWeight to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "NetWeight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The NetWeight must be positive number or 0. Actual value is -987";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Number_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.Number = 0; // Set Number to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "Number";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "Argument \"Number\" must be between 1 and 99.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_MeasurementNumber_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.MeasurementNumber = 0; // Set MeasurementNumber to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "MeasurementNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "MeasurementNumber 0 is invalid. MeasurementNumber must be between 1 and 2.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Number_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.Number = 100; // Set number to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "Number";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "Argument \"Number\" must be between 1 and 99.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_MeasurementNumber_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.MeasurementNumber = 4; // Set MeasurementNumber to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "MeasurementNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "MeasurementNumber 4 is invalid. MeasurementNumber must be between 1 and 2.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_totalNetOfInput_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetOfInput = -83643; // Set TotalNetOfInput to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetOfInput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetOfInput must be between 0 and 999999999. The actual value is -83643.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }


        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_totalNetOfInput_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetOfInput = 1099999999; // Set TotalNetOfInput to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetOfInput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetOfInput must be between 0 and 999999999. The actual value is 1099999999.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_exciseDocumentNumber_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.ExciseDocumentNumber = "1a1a1a"; // Set ExciseDocumentNumber to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "ExciseDocumentNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "ExciseDocumentNumber length must be 10 characters. The Actual value is 6";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_Invalid_GrossWeightAndTareWaightagainstNetWeight_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.NetWeight = 14999; // Change NetWeight to invalid value

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "GrossWeight || TareWaight || NetWaight";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = string.Format("The NetWeight must be equal to GrossWeight - TareWeight. \r\nActual values is NetWeight: {0}; GrossWeight{1}; TareWaight: {2};. \r\nSo {0} is not equal to {3}",
                                        message.NetWeight,message.GrossWeight,message.TareWeight,message.GrossWeight - message.TareWeight);

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_totalNetOfOutput_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetOfOutput = -843; // Set TotalNetOfOutput to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetOfOutput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetOfOutput must be between 0 and 999999999. The actual value is -843.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }


        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_totalNetOfOutput_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetOfOutput = 1099999998; // Set TotalNetOfOutput to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetOfOutput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetOfOutput must be between 0 and 999999999. The actual value is 1099999998.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TotalNetByProductInput_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetByProductInput = -3; // Set totalNetByProductInput to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetByProductInput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetByProductInput must be between 0 and 999999999. The actual value is -3.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TotalNetByProductInput_Greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetByProductInput = 1099999998; // Set TotalNetOfOutput to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetByProductInput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetByProductInput must be between 0 and 999999999. The actual value is 1099999998.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }


        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TotalNetByProductOutput_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetByProductOutput = -100; // Set totalNetByProductOutput to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetByProductOutput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetByProductOutput must be between 0 and 999999999. The actual value is -100.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TotalNetByProductOutput_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TotalNetByProductOutput = 1099249998; // Set TotalNetOfOutput to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TotalNetByProductOutput";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "The value of TotalNetByProductOutput must be between 0 and 999999999. The actual value is 1099249998.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }


        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_DocumentNumber_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.DocumentNumber = -100; // Set DocumentNumber to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "DocumentNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "DocumentNumber -100 out of range. Document number must be between 1 and 99999999";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_DocumentNumber_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.DocumentNumber = 109924999; // Set DocumentNumber to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "DocumentNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "DocumentNumber 109924999 out of range. Document number must be between 1 and 99999999";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TransactionNumber_Less_Than_Minimum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TransactionNumber = -100; // Set TransactionNumber to the value less than minimal acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TransactionNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = "Transaction number -100 is out of range. Transaction number must be between 1 and 99999.";

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        [TestMethod]
        public void Check_If_WeightScaleMessageNew_With_TransactionNumber_greater_Than_Maximum_IsValid()
        {
            // Arrange
            WeightScaleMessageNew message = GetValidWeightScaleMessageNew();
            message.TransactionNumber = 109924; // Set TransactionNumber to the value greater than maximum acceptable.

            // Act
            var result = message.Validate();

            // Assert
            int expectedValidationMessagesCount = 1;
            int actualValidationMessagesCount = result.Count;
            Assert.AreEqual(expectedValidationMessagesCount, actualValidationMessagesCount, "The number of the validation messages is greater than expected!");

            Assert.AreEqual(result[0].Type, MessageType.Error, "The type of ValidationMessage is not Error.");

            string expectedFieldName = "TransactionNumber";

            Assert.AreEqual(expectedFieldName, result[0].Field, "Field Names are not equal!");

            string expectedText = string.Format("Transaction number {0} is out of range. Transaction number must be between 1 and 99999.", message.TransactionNumber);

            Assert.AreEqual(expectedText, result[0].Text, "Texts are not equal!");
        }

        /// <summary>
        /// Gets the valid weight scale message new.
        /// </summary>
        /// <returns></returns>
        private WeightScaleMessageNew GetValidWeightScaleMessageNew()
        {
            return new WeightScaleMessageNew
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
                ExciseDocumentNumber = "1234567890",
                TareWeight = 10000,
                GrossWeight = 25000,
                NetWeight = 15000,
                TotalNetOfInput = 1238098,
                TotalNetOfOutput = 37000,
                Vehicle = "A3325KX",
                DocumentNumber = 12345,
                TotalNetByProductInput = 123456789,
                TotalNetByProductOutput = 27000,
            };
        }
    }
}
