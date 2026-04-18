using FluentAssertions;
using Grondo.Exceptions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ExceptionExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ToDetailString_SimpleException_ContainsTypeAndMessage()
        {
            var ex = new InvalidOperationException("something broke");
            string detail = ex.ToDetailString();

            detail.Should().Contain("System.InvalidOperationException");
            detail.Should().Contain("something broke");
        }

        [TestMethod]
        public void ToDetailString_WithInnerException_ContainsAllLevels()
        {
            var inner = new ArgumentException("inner message");
            var outer = new InvalidOperationException("outer message", inner);
            string detail = outer.ToDetailString();

            detail.Should().Contain("outer message");
            detail.Should().Contain("inner message");
            detail.Should().Contain("Inner Exception (depth 1)");
        }

        [TestMethod]
        public void ToDetailString_NullException_ThrowsArgumentNullException()
        {
            Exception ex = null!;
            Func<string> act = () => ex.ToDetailString();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Flatten_SingleException_ReturnsSingleElement()
        {
            var ex = new InvalidOperationException("test");
            var flat = ex.Flatten().ToList();

            flat.Should().HaveCount(1);
            flat[0].Message.Should().Be("test");
        }

        [TestMethod]
        public void Flatten_NestedExceptions_ReturnsAllInOrder()
        {
            var innerMost = new ArgumentException("level 2");
            var middle = new InvalidOperationException("level 1", innerMost);
            var outer = new Exception("level 0", middle);

            var flat = outer.Flatten().ToList();

            flat.Should().HaveCount(3);
            flat[0].Message.Should().Be("level 0");
            flat[1].Message.Should().Be("level 1");
            flat[2].Message.Should().Be("level 2");
        }

        [TestMethod]
        public void Flatten_NullException_ThrowsArgumentNullException()
        {
            Exception ex = null!;
            Func<List<Exception>> act = () => [.. ex.Flatten()];
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ToErrorResponse_ExceptionBase_ReturnsErrorResponse()
        {
            var ex = new BadRequestException("bad input", "Validation Error");
            var response = ex.ToErrorResponse();

            response.Message.Should().Be("bad input");
            response.MessageHeader.Should().Be("Validation Error");
        }

        [TestMethod]
        public void ToErrorResponse_NullException_ThrowsArgumentNullException()
        {
            ExceptionBase ex = null!;
            Func<ErrorResponse> act = () => ex.ToErrorResponse();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}

