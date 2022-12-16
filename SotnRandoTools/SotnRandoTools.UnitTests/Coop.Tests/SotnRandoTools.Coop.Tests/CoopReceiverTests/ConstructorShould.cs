using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopReceiverTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(null, mockedSotnApi, mockedNotificationService, mockedWatchlistService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, null, mockedNotificationService, mockedWatchlistService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenNotificationServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedSotnApi, null, mockedWatchlistService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act
            CoopReceiver coopReceiver = new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, mockedWatchlistService);
            //Assert
            Assert.NotNull(coopReceiver);
        }
    }
}
