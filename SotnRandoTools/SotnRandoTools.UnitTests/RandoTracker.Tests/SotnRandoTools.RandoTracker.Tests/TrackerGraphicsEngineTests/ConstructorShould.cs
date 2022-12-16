using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerGraphicsEngineTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenFormGraphicsIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new TrackerGraphicsEngine(null, mockedToolConfig));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new TrackerGraphicsEngine(mockedFormGraphics, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.NotNull(trackerGraphicsEngine);
        }
    }
}
