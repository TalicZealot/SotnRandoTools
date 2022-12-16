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
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(null, mockedWatchlistService, mockedInputService, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, null, mockedInputService, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenInputServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedWatchlistService, null, mockedSotnApi, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedWatchlistService, mockedInputService, null, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopMessangerIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopSender(mockedToolConfig, mockedWatchlistService, mockedInputService, mockedSotnApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedInputService = Substitute.For<IInputService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedCoopMessanger = Substitute.For<ICoopMessanger>();
            //Act
            CoopSender coopSender = new CoopSender(mockedToolConfig, mockedWatchlistService, mockedInputService, mockedSotnApi, mockedCoopMessanger);
            //Assert
            Assert.NotNull(coopSender);
        }
    }
}
