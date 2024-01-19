using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using System;
using System.Collections.Generic;
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
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
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
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
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
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
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
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
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
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphicsEngine, mockedToolConfig, mockedwatchlistService, mockedSotnApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedGraphicsEngine = Substitute.For<ITrackerRenderer>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            mockedToolConfig.Tracker = new TrackerConfig();
            mockedToolConfig.Tracker.Locations = false;
            mockedToolConfig.Tracker.UseOverlay = false;
            mockedToolConfig.Tracker.EnableAutosplitter = false;
            mockedGraphicsEngine
                .When(g => g.InitializeItems(Arg.Any<List<Models.TrackerRelic>>(), Arg.Any<List<Models.Item>>(), Arg.Any<List<Models.Item>>()))
                .Do(x => { });
            mockedGraphicsEngine
                .When(g => g.CalculateGrid(Arg.Any<int>(), Arg.Any<int>()))
                .Do(x => { });
            //Act
            Tracker tracker = new Tracker(mockedGraphicsEngine, mockedToolConfig, mockedwatchlistService, mockedSotnApi, mockedNotificationService);
            //Assert
            Assert.NotNull(tracker);
        }
    }
}
