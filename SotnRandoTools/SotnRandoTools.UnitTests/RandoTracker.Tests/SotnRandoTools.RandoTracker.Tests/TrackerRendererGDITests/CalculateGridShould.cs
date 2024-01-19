using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerRendererGDITests
{
    public class CalculateGridShould
    {
        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenWidthIsLowerThanMinimum()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerRendererGDI trackerGraphicsEngine = new TrackerRendererGDI(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => trackerGraphicsEngine.CalculateGrid(0, 100));
        }

        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenWidthIsHigherThanMaximum()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerRendererGDI trackerGraphicsEngine = new TrackerRendererGDI(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => trackerGraphicsEngine.CalculateGrid(9999, 100));
        }

        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenHeightIsLowerThanMinimum()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerRendererGDI trackerGraphicsEngine = new TrackerRendererGDI(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => trackerGraphicsEngine.CalculateGrid(100, 0));
        }

        [Fact]
        public void ThrowArgumentOutOfRangeException_WhenHeightIsHigherThanMaximum()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            //Act
            TrackerRendererGDI trackerGraphicsEngine = new TrackerRendererGDI(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => trackerGraphicsEngine.CalculateGrid(100, 9999));
        }
    }
}
