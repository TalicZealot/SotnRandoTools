using System;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using Xunit;

namespace SotnRandoTools.RandoTracker.Tests.TrackerTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(null, mockedSotnApi, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedToolConfig, null, mockedNotificationService));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenNotificationServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new Tracker(mockedToolConfig, mockedSotnApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            mockedToolConfig.Tracker = new TrackerConfig();
            mockedToolConfig.Tracker.Locations = false;
            mockedToolConfig.Tracker.UseOverlay = false;
            mockedToolConfig.Tracker.EnableAutosplitter = false;
            //Act
            Tracker tracker = new Tracker(mockedToolConfig, mockedSotnApi, mockedNotificationService);
            //Assert
            Assert.NotNull(tracker);
        }
    }
}
