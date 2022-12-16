using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerGraphicsEngineTests
{
    public class DrawSeedInfoShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenGraphicsSeedInfoIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentNullException>(() => trackerGraphicsEngine.DrawSeedInfo(null));
        }

        [Fact]
        public void ThrowArgumentException_WhenGraphicsSeedInfoIsEmpty()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentException>(() => trackerGraphicsEngine.DrawSeedInfo(String.Empty));
        }
    }
}
