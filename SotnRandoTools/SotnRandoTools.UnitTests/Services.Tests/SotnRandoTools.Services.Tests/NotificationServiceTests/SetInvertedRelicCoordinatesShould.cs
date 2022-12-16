using BizHawk.Client.Common;
using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.NotificationServiceTests
{
    public class SetInvertedRelicCoordinatesShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenRelicArgumentIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.SetInvertedRelicCoordinates("", 100, 100));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenRelicArgumentIsEmpty()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.SetInvertedRelicCoordinates(String.Empty, 100, 100));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMapColArgumentIsLowerThanMinimum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => notificationService.SetInvertedRelicCoordinates("SoulOfBat", -1, 100));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMapColArgumentIsHigherThanMaximum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => notificationService.SetInvertedRelicCoordinates("SoulOfBat", 666, 100));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMaprowArgumentIsLowerThanMinimum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => notificationService.SetInvertedRelicCoordinates("SoulOfBat", 100, -1));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMapRowArgumentIsHigherThanMaximum()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => notificationService.SetInvertedRelicCoordinates("SoulOfBat", 100, 666));
        }
    }
}
