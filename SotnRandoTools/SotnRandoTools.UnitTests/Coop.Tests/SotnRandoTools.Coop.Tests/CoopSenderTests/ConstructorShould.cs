using System;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopSenderTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(null, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, null, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopMessangerIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedSotnApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act
            CoopSender coopSender = new CoopSender(mockedToolConfig, mockedSotnApi, mockedCoopMessanger);
            //Assert
            Assert.NotNull(coopSender);
        }
    }
}
