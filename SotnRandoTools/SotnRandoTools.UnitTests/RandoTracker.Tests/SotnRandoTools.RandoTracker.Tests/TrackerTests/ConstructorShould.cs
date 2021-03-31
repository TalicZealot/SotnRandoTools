using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
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
        public void ReturnAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
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
