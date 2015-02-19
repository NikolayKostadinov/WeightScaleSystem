//---------------------------------------------------------------------------------
// <copyright file="XorChecksumServiceTests.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocolTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WeightScale.ComunicationProtocol;
    using WeightScale.Utility.Helpers;

    /// <summary>
    /// Unit tests for XORChecksumService class
    /// </summary>
    [TestClass]
    public class XorChecksumServiceTests
    {
        [TestMethod]
        public void CaclulateCheckSum_Returns_Ecpected_Without_Leadin_And_Trailing()
        {
            // Arrange
            byte[] input = new byte[] { 0xAA };
            byte expected = 0x2A;
            var checksumService = new XorChecksumService();

            // Act
            var actual = checksumService.CalculateCheckSum(input, null, null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CaclulateCheckSum_Returns_Ecpected_With_Leadin_And_Trailing()
        {
            // Arrange
            byte[] leading = new byte[] { 0x55 };
            byte[] input = new byte[] { 0xAA };
            byte[] trailing = new byte[] { 0 };
            byte expected = 0x7F;
            var checksumService = new XorChecksumService();

            // Act
            var actual = checksumService.CalculateCheckSum(input, leading, trailing);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateCheckSum_Returns_Expected_RealData()
        {
            // Arrange
            var inputs = new List<byte[]>()
            {
                "032                            26864     1         138                                                     E5955KA       13513                  ".ToByteArray(), // 0
                "032                            26865     1         138                                                     A6784BX       13517                  ".ToByteArray(), // 1
                "032                            26866     2         138                                                     A6588MA       13514                  ".ToByteArray(), // 2
                "032                            26870     2         138                                                     C2739HA       13515                  ".ToByteArray(), // 3
                "032                            26864     1         138                                                     E5955KA       13513                  ".ToByteArray(), // 4
                "032                            26865     1         138                                                     A6784BX       13517                  ".ToByteArray(), // 5
                "032                            26866     2         138                                                     A6588MA       13514                  ".ToByteArray(), // 6
                "032                            26870     2         138                                                     C2739HA       13515                  ".ToByteArray(), // 7
                "012                             5436     1         141                                                338079214676                              ".ToByteArray(), // 8
                "012                             4995     1         141                                                338079214676                              ".ToByteArray(), // 9
                "052                            30280     1                                                                 K9334AX                              ".ToByteArray(), // 10
                "042                            24766     1         209                                                     A8033KC        6658                  ".ToByteArray(), // 11
                "042                            24767     1         209                                                     A3078MB        6659                  ".ToByteArray(), // 12
                "012                                3     1              B. OTPADAK                                         A4836BK       69604".ToByteArray(), // 13                 
                "022                             5532     1         138                                                845279120198                              ".ToByteArray(), // 14
                "022                             5533     2         138                                                845279120198                              ".ToByteArray(), // 15
                "102                                5     2         028                                                333333333333                              ".ToByteArray(), // 16
            };

            var actual = new List<byte>();
            var expected = new byte[]
            {
                241, // 0
                225, // 1
                250, // 2
                247, // 3
                241, // 4
                225, // 5
                250, // 6
                247, // 7
                151, // 8
                146, // 9
                227, // 10
                226, // 11
                225, // 12
                230, // 13
                159, // 14
                157, // 15
                141, // 16
            };
            var checksumService = new XorChecksumService();

            // Act
            foreach (var input in inputs)
            {
                actual.Add(checksumService.CalculateCheckSum(input, null, new byte[] { (byte)CommunicationConstants.Etx }));
            }

            // Assert
            Assert.AreEqual(expected.Length, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], string.Format("{0} input fails. Expected: {1} Actual:{2}", i, expected[i], actual[i]));
            }
        }
    }
}