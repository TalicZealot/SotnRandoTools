using System;
using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.Services;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopReceiverTests
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
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(null, mockedSotnApi, mockedNotificationService, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, null, mockedNotificationService, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenNotificationServiceIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedSotnApi, null, mockedCoopMessanger));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCoopControllerIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedCoopMessanger = Substitute.For<ICoopController>();
            //Act
            CoopReceiver coopReceiver = new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, mockedCoopMessanger);
            //Assert
            Assert.NotNull(coopReceiver);
        }
    }
}
