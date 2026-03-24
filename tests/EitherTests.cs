using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class EitherTests : BaseTest
    {
        // --- ToMaybe ---

        [TestMethod]
        public void ToMaybe_Right_ReturnsSome()
        {
            var maybe = Either<string, int>.FromRight(42).ToMaybe();
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToMaybe_Left_ReturnsNone()
        {
            var maybe = Either<string, int>.FromLeft("error").ToMaybe();
            maybe.HasNoValue.Should().BeTrue();
        }

        // --- ToResult ---

        [TestMethod]
        public void ToResult_Right_ReturnsSuccess()
        {
            var result = Either<string, int>.FromRight(42).ToResult(l => l);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToResult_Left_ReturnsFailureWithMappedError()
        {
            var result = Either<int, string>.FromLeft(404).ToResult(l => $"Error code: {l}");
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Error code: 404");
        }

        [TestMethod]
        public void ToResult_NullMapper_ThrowsArgumentNullException()
        {
            var either = Either<string, int>.FromRight(1);
            Action act = () => either.ToResult(null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}

