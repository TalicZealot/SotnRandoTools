using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.InputServiceTests
{
    public class ButtonPressedShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenMoveNameArgumentIsNull()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act
            InputService inputService = new InputService(mockedJoypadApi, mockedSotnApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => inputService.ButtonPressed("", 1));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenMoveNameArgumentIsEmpty()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act
            InputService inputService = new InputService(mockedJoypadApi, mockedSotnApi);
            //Assert
            Assert.Throws<ArgumentNullException>(() => inputService.ButtonPressed(String.Empty, 1));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenFramesIsBelowMinimum()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var expectedMessage = $"Frames must be between 1 and {Constants.Globals.MaximumCooldownFrames}\r\nParameter name: frames";
            //Act
            InputService inputService = new InputService(mockedJoypadApi, mockedSotnApi);
            //Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => inputService.ButtonPressed("Move", 0));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenFramesIsHigherThanMaximum()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            var expectedMessage = $"Frames must be between 1 and {Constants.Globals.MaximumCooldownFrames}\r\nParameter name: frames";
            //Act
            InputService inputService = new InputService(mockedJoypadApi, mockedSotnApi);
            //Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => inputService.ButtonPressed("Move", Constants.Globals.MaximumCooldownFrames + 1));
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
