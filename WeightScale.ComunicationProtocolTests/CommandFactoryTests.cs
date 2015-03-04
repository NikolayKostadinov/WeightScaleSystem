//---------------------------------------------------------------------------------
// <copyright file="CommandFactoryTests.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocolTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WeightScale.ComunicationProtocol;
    using WeightScale.Domain.Abstract;
    using WeightScale.Utility.Helpers;

    /// <summary>
    /// Unit tests for CommandFactory class
    /// </summary>
    [TestClass]
    public class CommandFactoryTests
    {
        private static Mock<IBlock> mock = new Mock<IBlock>();

        /// <summary>
        /// Provides initialization for mock objects required for testing. Initializes static members of the <see cref="CommandFactoryTests" /> class.
        /// </summary>
        static CommandFactoryTests()
        {
            mock.Setup(m => m.ToBlock()).Returns(
                ("032                            " +
                 "26837     " +
                 "1         " +
                 "138                                                     " +
                 "A8756MK       " +
                 "13500                  ").ToByteArray());
            mock.Setup(m => m.Number).Returns(3);
        }

        [TestMethod]
        public void Check_If_WeightScaleRequest_Returns_Expected()
        {
            // Arrange
            byte[] expected = new byte[]
            {
                (byte)1,
                (byte)3,
                Convert.ToByte('p'),
                (byte)5
            };
            var command = new CommandFactory(null);

            // Act
            var actual = command.WeightScaleRequest(mock.Object);

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void Check_If_SendDataToWeightScale_Returns_Correct_Result()
        {
            // Arrange
            // Constructing expected value
            var list = new List<byte>();
            list.Add(0x01);
            list.Add(0x03);
            list.Add(0x02);
            list.AddRange(("032                            " +
                 "26837     " +
                 "1         " +
                 "138                                                     " +
                 "A8756MK       " +
                 "13500                  ").ToByteArray());
            list.Add(0x03);
            list.Add(253);
            list.Add(0x05);

            byte[] expected = list.ToArray();

            var command = new CommandFactory(new XorChecksumService());

            // Act
            var actual = command.SendDataToWeightScale(mock.Object);

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void EndOfTransmit_Returns_0x04()
        {
            // Arrange
            var expected = new byte[] { 0x04 };
            var command = new CommandFactory(null);

            // Act
            var actual = command.EndOfTransmit();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void Acknowledge_Returns_0x06()
        { 
            // Arrange
            var expected = new byte[] { 0x06 };
            var command = new CommandFactory(null);

            // Act
            var actual = command.Acknowledge();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void NegativeAcknowledge_Returns_0x15()
        {
            // Arrange
            var expected = new byte[] { 0x15 };
            var command = new CommandFactory(null);

            // Act
            var actual = command.NegativeAcknowledge();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}