using BizHawk.Client.Common;
using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.NotificationServiceTests
{
    public class PlayAlertShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenUrlArgumentIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.PlayAlert(""));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenUrlArgumentIsEmpty()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.PlayAlert(String.Empty));
        }
    }
}
