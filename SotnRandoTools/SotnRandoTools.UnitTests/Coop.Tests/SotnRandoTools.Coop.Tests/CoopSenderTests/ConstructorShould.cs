using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;
using System;
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
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(null, mockedWatchlistService, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, null, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedWatchlistService, null, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopMessangerIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedWatchlistService, mockedSotnApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act
            CoopSender coopSender = new CoopSender(mockedToolConfig, mockedWatchlistService, mockedSotnApi, mockedCoopMessanger);
            //Assert
            Assert.NotNull(coopSender);
        }
    }
}
