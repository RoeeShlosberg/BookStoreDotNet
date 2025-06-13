using Xunit;

namespace BooksApi.Tests
{
    public class SimpleTest
    {
        [Fact]
        public void SimpleTest_ShouldPass()
        {
            // Arrange
            var expected = 2;
            
            // Act
            var actual = 1 + 1;
            
            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
