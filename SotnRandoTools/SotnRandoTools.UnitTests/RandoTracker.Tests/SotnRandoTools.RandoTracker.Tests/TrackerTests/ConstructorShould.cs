using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
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
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(null, mockedToolConfig, mockedwatchlistService, mockedRenderingApi, mockedGameApi, mockedAlucardApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, null, mockedwatchlistService, mockedRenderingApi, mockedGameApi, mockedAlucardApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, null, mockedRenderingApi, mockedGameApi, mockedAlucardApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenRenderingApiIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, null, mockedGameApi, mockedAlucardApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenGameApiIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, mockedRenderingApi, null, mockedAlucardApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenAlucardApiIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, mockedRenderingApi, mockedGameApi, null));
        }

        [Fact(Skip = "Skip for now, too many file reads. Would be  more of an integration test atm.")]
        public void ReturnAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            TrackerConfig stubTrackerConfig = new TrackerConfig();
            stubTrackerConfig.Locations = false;
            mockedToolConfig
                .Tracker
                .Returns<TrackerConfig>(stubTrackerConfig);
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedRenderingApi = Substitute.For<IRenderingApi>();
            var mockedGameApi = Substitute.For<IGameApi>();
            var mockedAlucardApi = Substitute.For<IAlucardApi>();
            //Act
            var trackerUnderTest = new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, mockedRenderingApi, mockedGameApi, mockedAlucardApi);
            //Assert
            Assert.NotNull(trackerUnderTest);
        }
    }
}
