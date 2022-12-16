using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopMessangerTests
{
    public class StartServerShould
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
            Assert.Throws<ArgumentOutOfRangeException>(() => coopMessanger.StartServer(SotnRandoTools.Constants.Globals.PortMinimum - 1));
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
            Assert.Throws<ArgumentOutOfRangeException>(() => coopMessanger.StartServer(SotnRandoTools.Constants.Globals.PortMaximum + 1));
        }
    }
}
