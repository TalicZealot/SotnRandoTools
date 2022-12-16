using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopMessangerTests
{
    public class SendDataShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenDataIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.Throws<ArgumentNullException>(() => coopMessanger.SendData(Enums.MessageType.Item, null));
        }

        [Fact]
        public void ThrowArgumentException_WhenDataLengthIsTooLow()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            var expectedMessage = "Array length for data should be at least 2.";
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            var exception = Assert.Throws<ArgumentException>(() => coopMessanger.SendData(Enums.MessageType.Item, new byte[1] { 0xFF }));
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
