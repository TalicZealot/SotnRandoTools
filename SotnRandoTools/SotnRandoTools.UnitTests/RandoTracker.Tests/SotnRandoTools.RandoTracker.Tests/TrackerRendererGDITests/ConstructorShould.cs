using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerRendererGDITests
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
            Assert.Throws<ArgumentNullException>(() => new TrackerRendererGDI(null, mockedToolConfig));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new TrackerRendererGDI(mockedFormGraphics, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerRendererGDI trackerGraphicsEngine = new TrackerRendererGDI(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.NotNull(trackerGraphicsEngine);
        }
    }
}
