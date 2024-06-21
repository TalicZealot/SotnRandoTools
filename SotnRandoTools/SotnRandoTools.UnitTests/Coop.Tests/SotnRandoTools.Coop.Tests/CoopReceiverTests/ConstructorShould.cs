using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
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
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(null, mockedWatchlistService, mockedSotnApi, mockedNotificationService, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedWatchlistService, null, mockedNotificationService, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenNotificationServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedWatchlistService,  mockedSotnApi, null, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, null, mockedSotnApi, mockedNotificationService, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopControllerIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedWatchlistService, mockedSotnApi, mockedNotificationService, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act
            CoopReceiver coopReceiver = new CoopReceiver(mockedToolConfig, mockedWatchlistService, mockedSotnApi, mockedNotificationService, mockedCoopMessanger);
            //Assert
            Assert.NotNull(coopReceiver);
        }
    }
}
