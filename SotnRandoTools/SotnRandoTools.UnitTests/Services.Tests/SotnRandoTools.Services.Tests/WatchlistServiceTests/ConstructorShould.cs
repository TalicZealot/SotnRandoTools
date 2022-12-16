using BizHawk.Emulation.Common;
using NSubstitute;
using System;
using Xunit;

namespace SotnRandoTools.Services.Tests.WatchlistServiceTests
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowArgumentNullException_WhenMemoryDomainsIsNull()
        {
            //Act&Assert
            Assert.Throws<ArgumentNullException>(() => new WatchlistService(null));
        }

        [Fact]
        public void ReturnsAnInstance_WhenParametersAreNotNull()
        {
            //Arrange
            var mockedMemoryDomains = Substitute.For<IMemoryDomains>();
            //Act
            WatchlistService watchlistService = new WatchlistService(mockedMemoryDomains);
            //Assert
            Assert.NotNull(watchlistService);
        }
    }
}
