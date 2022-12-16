using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.InputServiceTests
{
    public class DirectionHeldShould
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
            Assert.Throws<ArgumentNullException>(() => inputService.DirectionHeld(""));
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
            Assert.Throws<ArgumentNullException>(() => inputService.DirectionHeld(String.Empty));
        }
    }
}
