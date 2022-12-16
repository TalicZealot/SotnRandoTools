using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopMessangerTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopMessanger(null, mockedCoopReceiver, mockedCoopViewModel));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopReceiverIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopMessanger(mockedToolConfig, null, mockedCoopViewModel));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopViewModelIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopMessanger(mockedToolConfig, mockedCoopReceiver, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedCoopReceiver = Substitute.For<ICoopReceiver>();
            var mockedCoopViewModel = Substitute.For<ICoopViewModel>();
            //Act
            CoopMessanger coopMessanger = new CoopMessanger(mockedToolConfig, mockedCoopReceiver, mockedCoopViewModel);
            //Assert
            Assert.NotNull(coopMessanger);
        }
    }
}
