using NSubstitute;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using System;
using Xunit;

namespace SotnRandoTools.Coop.Tests.CoopReceiverTests
{
    public class EnqueMessageShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenDataIsNull()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            //Act
            CoopReceiver coopReceiver = new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, mockedWatchlistService);
            //Assert
            Assert.Throws<ArgumentNullException>(() => coopReceiver.EnqueMessage(null));
        }

        [Fact]
        public void ThrowArgumentException_WhenDataLengthIsTooLow()
        {
            //Arrange
            var mockedToolConfig = Substitute.For<IToolConfig>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var mockedNotificationService = Substitute.For<INotificationService>();
            var mockedWatchlistService = Substitute.For<IWatchlistService>();
            var expectedMessage = "Array length for data should be at least 2.";
            //Act
            CoopReceiver coopReceiver = new CoopReceiver(mockedToolConfig, mockedSotnApi, mockedNotificationService, mockedWatchlistService);
            //Assert
            var exception = Assert.Throws<ArgumentException>(() => coopReceiver.EnqueMessage(new byte[1] { 0xFF }));
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
