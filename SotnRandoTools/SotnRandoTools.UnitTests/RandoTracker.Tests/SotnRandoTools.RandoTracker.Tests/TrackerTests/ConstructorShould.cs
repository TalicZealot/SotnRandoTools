using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using System;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerTests
{
    public class ConstructorShould
    {

        [Fact]
        public void ThrowArgumentNullException_WhenGraphicsIsNull()
        {
            //Arrange
            var mockedMemAPI = Substitute.For<IMemoryApi>();
            var mockedGraphicsEngine = Substitute.For<ITrackerGraphicsEngine>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(null, mockedToolConfig, mockedwatchlistService, mockedSotnApi, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedGraphicsEngine = Substitute.For<ITrackerGraphicsEngine>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphicsEngine, null, mockedwatchlistService, mockedSotnApi, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedGraphicsEngine = Substitute.For<ITrackerGraphicsEngine>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphicsEngine, mockedToolConfig, null, mockedSotnApi, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedGraphicsEngine = Substitute.For<ITrackerGraphicsEngine>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphicsEngine, mockedToolConfig, mockedwatchlistService, null, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenNotificationServiceIsNull()
        {
            //Arrange
            var mockedGraphicsEngine = Substitute.For<ITrackerGraphicsEngine>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphicsEngine, mockedToolConfig, mockedwatchlistService, mockedSotnApi, null));
        }
    }
}
