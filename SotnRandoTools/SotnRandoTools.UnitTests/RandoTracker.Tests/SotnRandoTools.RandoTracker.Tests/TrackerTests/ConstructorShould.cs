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
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(null, mockedToolConfig, mockedwatchlistService, mockedSotnApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, null, mockedwatchlistService, mockedSotnApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenWatchlistServiceIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, null, mockedSotnApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedwatchlistService = Substitute.For<IWatchlistService>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, null));
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
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act
            var trackerUnderTest = new Tracker(mockedGraphics, mockedToolConfig, mockedwatchlistService, mockedSotnApi);
            //Assert
            Assert.NotNull(trackerUnderTest);
        }
    }
}
