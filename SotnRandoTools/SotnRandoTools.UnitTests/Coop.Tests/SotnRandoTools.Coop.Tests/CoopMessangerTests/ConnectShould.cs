using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopMessangerTests
{
    public class ConnectShould
    {
        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenPortIsLowerThanMinimum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => coopMessanger.Connect("255.255.255.255", SotnRandoTools.Constants.Globals.PortMinimum - 1));
        }

        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenPortIsHigherThanMaximum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => coopMessanger.Connect("255.255.255.255", SotnRandoTools.Constants.Globals.PortMaximum + 1));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenIpIsEmptyString()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.Throws<ArgumentNullException>(() => coopMessanger.Connect(String.Empty, SotnRandoTools.Constants.Globals.PortMinimum));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenIpStringIsInvalid()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.Throws<ArgumentException>(() => coopMessanger.Connect("invalid ip string", SotnRandoTools.Constants.Globals.PortMinimum));
        }
    }
}
