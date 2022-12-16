using BizHawk.Client.Common;
using NSubstitute;
using SotnApi.Interfaces;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.InputServiceTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenJoypadApiIsNull()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new InputService(null, mockedSotnApi));
        }

        [Fact]
        public void ThrowArgumentNullException_WhenSotnApiIsNull()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new InputService(mockedJoypadApi, null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedJoypadApi = Substitute.For<IJoypadApi>();
            var mockedSotnApi = Substitute.For<ISotnApi>();
            //Act
            InputService inputService = new InputService(mockedJoypadApi, mockedSotnApi);
            //Assert
            Assert.NotNull(inputService);
        }
    }
}
