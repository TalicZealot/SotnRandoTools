using System;
using BizHawk.Client.Common;
using NSubstitute;
using SotnRandoTools.Configuration.Interfaces;
using Xunit;

namespace SotnRandoTools.Services.Tests.NotificationServiceTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenToolConfigIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            var mockedGlobalConfig = Substitute.For<Config>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new NotificationService(null, mockedGuiApi, mockedEmuClientApi, mockedGlobalConfig));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenGuiApiIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            var mockedGlobalConfig = Substitute.For<Config>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new NotificationService(mockedToolConfig, null, mockedEmuClientApi, mockedGlobalConfig));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenEmuClientIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            var mockedGlobalConfig = Substitute.For<Config>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new NotificationService(mockedToolConfig, mockedGuiApi, null, mockedGlobalConfig));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedGuiApi = Substitute.For<IGuiApi>();
            var mockedEmuClientApi = Substitute.For<IEmuClientApi>();
            var mockedGlobalConfig = Substitute.For<Config>();
            mockedToolConfig.Coop = new Configuration.CoopConfig();
            //Act
            NotificationService notificationService = new NotificationService(mockedToolConfig, mockedGuiApi, mockedEmuClientApi, mockedGlobalConfig);
            //Assert
            Assert.NotNull(notificationService);
        }
    }
}
