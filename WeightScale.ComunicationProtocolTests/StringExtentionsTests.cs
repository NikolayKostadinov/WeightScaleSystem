//---------------------------------------------------------------------------------
// <copyright file="StringExtentionsTests.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------

namespace WeightScale.ComunicationProtocolTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WeightScale.Utility;
    using WeightScale.Utility.Helpers;

    [TestClass]
    public class StringExtentionsTests
    {
        [TestMethod]
        public void ToByteArray_Without_Parameters_Returns_Expected()
        {
            // Arrange
            byte[] expected = new byte[] { 1, 2, 3 };
            string input = new string(new char[]
                {
                    Convert.ToChar(1),
                    Convert.ToChar(2),
                    Convert.ToChar(3)
                });

            // Act
            byte[] actual = input.ToByteArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToByteArray_Without_Parameters_Throws_exception_If_sting_Is_null()
        {
            // Arrange
            string input = null;

            // Act
            byte[] actual = input.ToByteArray();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToByteArray_Without_Parameters_Throws_exception_If_sting_Is_Empty()
        {
            // Arrange
            string input = string.Empty;

            // Act
            byte[] actual = input.ToByteArray();
        }

        [TestMethod]
        public void ToByteArray_With_Left_Parameter_Returns_Expected()
        {
            // Arrange
            byte[] expected = new byte[] { 32, 32, 1, 2, 3 };
            string input = new string(new char[]
                {
                    Convert.ToChar(1),
                    Convert.ToChar(2),
                    Convert.ToChar(3)
                });

            // Act
            byte[] actual = input.ToByteArray(Alignment.Right, 5);

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void ToByteArray_With_Right_Parameter_Returns_Expected()
        {
            // Arrange
            byte[] expected = new byte[] { 1, 2, 3, 32, 32 };
            string input = new string(new char[]
                {
                    Convert.ToChar(1),
                    Convert.ToChar(2),
                    Convert.ToChar(3)
                });

            // Act
            byte[] actual = input.ToByteArray(Alignment.Left, 5);

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToByteArray_With_Parameters_Throws_exception_If_sting_Is_null()
        {
            // Arrange
            string input = null;

            // Act
            byte[] actual = input.ToByteArray(Alignment.Right, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToByteArray_With_Parameters_Throws_exception_If_sting_Is_Empty()
        {
            // Arrange
            string input = string.Empty;

            // Act
            byte[] actual = input.ToByteArray(Alignment.Right, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToByteArray_With_Parameters_Throws_exception_If_Alihgtment_is_wrong()
        {
            // Arrange
            string input = "test";

            // Act
            byte[] actual = input.ToByteArray((Alignment)3, 5);
        }
    }
}