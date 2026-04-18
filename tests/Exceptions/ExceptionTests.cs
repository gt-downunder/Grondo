using System.Net;
using FluentAssertions;
using Grondo.Exceptions;

namespace Grondo.Tests.Exceptions
{
    [TestClass]
    public class ExceptionTests : BaseExceptionTest
    {
        [TestMethod]
        public void BadRequestException_Throws_400Error()
        {
            Action act = () => ThrowException(new BadRequestException(string.Empty));

            act.Should().Throw<BadRequestException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void BadRequestException_HasCorrectDefaultHeader()
        {
            var ex = new BadRequestException("test");
            ex.MessageHeader.Should().Be("Bad Request");
            ((int)ex.StatusCode).Should().Be(400);
        }

        [TestMethod]
        public void NotAuthorizedException_Throws_401Error()
        {
            Action act = () => ThrowException(new NotAuthorizedException(string.Empty));

            act.Should().Throw<NotAuthorizedException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void NotAuthorizedException_HasCorrectDefaultHeader()
        {
            var ex = new NotAuthorizedException("test");
            ex.MessageHeader.Should().Be("Not authorized");
            ex.MessageHeader.Should().NotBe("Not authorised");
            ((int)ex.StatusCode).Should().Be(401);
        }

        [TestMethod]
        public void ForbiddenException_Throws_403Error()
        {
            Action act = () => ThrowException(new ForbiddenException(string.Empty));

            act.Should().Throw<ForbiddenException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void ForbiddenException_HasCorrectDefaultHeader()
        {
            var ex = new ForbiddenException("test");
            ex.MessageHeader.Should().Be("Forbidden");
            ((int)ex.StatusCode).Should().Be(403);
        }

        [TestMethod]
        public void EntityNotFoundException_Throws_404Error()
        {
            Action act = () => ThrowException(new EntityNotFoundException(string.Empty));

            act.Should().Throw<EntityNotFoundException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void EntityNotFoundException_HasCorrectDefaultHeader()
        {
            var ex = new EntityNotFoundException("test");
            ex.MessageHeader.Should().Be("Not found");
            ((int)ex.StatusCode).Should().Be(404);
        }

        [TestMethod]
        public void BusinessException_Throws_409Error()
        {
            Action act = () => ThrowException(new BusinessException(string.Empty));

            act.Should().Throw<BusinessException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [TestMethod]
        public void BusinessException_HasCorrectDefaultHeader()
        {
            var ex = new BusinessException("test");
            ex.MessageHeader.Should().Be("Business rule violation");
            ((int)ex.StatusCode).Should().Be(409);
        }

        [TestMethod]
        public void ConflictException_Throws_409Error()
        {
            Action act = () => ThrowException(new ConflictException(string.Empty));

            act.Should().Throw<ConflictException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [TestMethod]
        public void ConflictException_HasCorrectDefaultHeader()
        {
            var ex = new ConflictException("test");
            ex.MessageHeader.Should().Be("Conflict");
            ((int)ex.StatusCode).Should().Be(409);
        }

        [TestMethod]
        public void DuplicateFoundException_Throws_409Error()
        {
            Action act = () => ThrowException(new DuplicateFoundException(string.Empty));

            act.Should().Throw<DuplicateFoundException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [TestMethod]
        public void DuplicateFoundException_HasCorrectDefaultHeader()
        {
            var ex = new DuplicateFoundException("test");
            ex.MessageHeader.Should().Be("Duplicate found");
            ((int)ex.StatusCode).Should().Be(409);
        }

        [TestMethod]
        public void TechnicalException_Throws_500Error()
        {
            Action act = () => ThrowException(new TechnicalException(string.Empty));

            act.Should().Throw<TechnicalException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void TechnicalException_HasCorrectDefaultHeader()
        {
            var ex = new TechnicalException("test");
            ex.MessageHeader.Should().Be("Internal server error");
            ((int)ex.StatusCode).Should().Be(500);
        }

        [TestMethod]
        public void TechnicalException_DefaultConstructor_HasDefaultMessage()
        {
            var ex = new TechnicalException();
            ex.Message.Should().Be("Please contact the system administrator");
            ex.MessageHeader.Should().Be("Internal server error");
        }

        [TestMethod]
        public void MethodNotAvailableException_Throws_501Error()
        {
            Action act = () => ThrowException(new MethodNotAvailableException(string.Empty));

            act.Should().Throw<MethodNotAvailableException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public void MethodNotAvailableException_HasCorrectDefaultHeader()
        {
            var ex = new MethodNotAvailableException("test");
            ex.MessageHeader.Should().Be("Method not available");
            ((int)ex.StatusCode).Should().Be(501);
        }

        [TestMethod]
        public void ExceptionBase_CustomMessageHeader_Overrides()
        {
            var ex = new BadRequestException("msg", "Custom Header");
            ex.MessageHeader.Should().Be("Custom Header");
            ex.Message.Should().Be("msg");
        }

        [TestMethod]
        public void ExceptionBase_InnerException_IsPreserved()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new BadRequestException("outer", innerException: inner);
            ex.InnerException.Should().BeSameAs(inner);
        }

        [TestMethod]
        public void PaymentRequiredException_Throws_402Error()
        {
            Action act = () => ThrowException(new PaymentRequiredException(string.Empty));
            act.Should().Throw<PaymentRequiredException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.PaymentRequired);
        }

        [TestMethod]
        public void PaymentRequiredException_HasCorrectDefaultHeader()
        {
            var ex = new PaymentRequiredException("test");
            ex.MessageHeader.Should().Be("Payment Required");
            ((int)ex.StatusCode).Should().Be(402);
        }

        [TestMethod]
        public void PaymentRequiredException_CustomHeader_IsPreserved()
        {
            var ex = new PaymentRequiredException("test", "Subscription Required");
            ex.MessageHeader.Should().Be("Subscription Required");
        }
    }
}
