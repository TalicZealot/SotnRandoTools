using BizHawk.Client.Common;
using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.NotificationServiceTests
{
    public class AddMessageShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenMessageArgumentIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.AddMessage(""));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMessageArgumentIsEmpty()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => notificationService.AddMessage(String.Empty));
        }
    }
}
