using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerGraphicsEngineTests
{
    public class InitializeItemsShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenRelicsArgumentIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            List<TrackerRelic> testRelics = new List<TrackerRelic>();
            List<Item> testProgressionItems = new List<Item>();
            List<Item> testThrustSwords = new List<Item>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentNullException>(() => trackerGraphicsEngine.InitializeItems(null, testProgressionItems, testThrustSwords));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenProgressionItemsArgumentIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            List<TrackerRelic> testRelics = new List<TrackerRelic>();
            List<Item> testProgressionItems = new List<Item>();
            List<Item> testThrustSwords = new List<Item>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentNullException>(() => trackerGraphicsEngine.InitializeItems(testRelics, null, testThrustSwords));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenThrustSwordsArgumentIsNull()
        {
            //Arrange
            var mockedFormGraphics = Substitute.For<IGraphics>();
            var mockedToolConfig = Substitute.For<IToolConfig>();
            List<TrackerRelic> testRelics = new List<TrackerRelic>();
            List<Item> testProgressionItems = new List<Item>();
            List<Item> testThrustSwords = new List<Item>();
            //Act
            TrackerGraphicsEngine trackerGraphicsEngine = new TrackerGraphicsEngine(mockedFormGraphics, mockedToolConfig);
            //Assert
            Assert.Throws<ArgumentNullException>(() => trackerGraphicsEngine.InitializeItems(testRelics, testProgressionItems, null));
        }
    }
}
