using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ResultTETests : BaseTest
    {
        [TestMethod]
        public void Success_CreatesSuccessfulResult()
        {
            var r = Result<int, Error>.Success(42);
            r.IsSuccess.Should().BeTrue();
            r.IsFailure.Should().BeFalse();
            r.Value.Should().Be(42);
        }

        [TestMethod]
        public void Failure_CreatesFailedResult()
        {
            var err = Error.NotFound("x");
            var r = Result<int, Error>.Failure(err);
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void Value_OnFailure_Throws()
        {
            var r = Result<int, Error>.Failure(Error.NotFound());
            Func<int> act = () => r.Value;
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Error_OnSuccess_Throws()
        {
            var r = Result<int, Error>.Success(1);
            Func<Error> act = () => r.Error;
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Map_TransformsSuccessValue()
        {
            Result<int, Error> r = Result<int, Error>.Success(2).Map(x => x * 10);
            r.Value.Should().Be(20);
        }

        [TestMethod]
        public void Map_PropagatesFailure()
        {
            var err = Error.NotFound();
            Result<int, Error> r = Result<int, Error>.Failure(err).Map(x => x * 10);
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void MapError_TransformsError()
        {
            Result<int, string> r = Result<int, Error>.Failure(Error.NotFound("x"))
                .MapError(e => e.Code);
            r.Error.Should().Be("not_found");
            r.IsFailure.Should().BeTrue();
        }

        [TestMethod]
        public void Bind_ChainsSuccess()
        {
            Result<string, Error> r = Result<int, Error>.Success(3)
                .Bind(x => Result<string, Error>.Success($"v={x}"));
            r.Value.Should().Be("v=3");
        }

        [TestMethod]
        public void Bind_ShortCircuitsOnFailure()
        {
            var err = Error.Validation("bad");
            Result<string, Error> r = Result<int, Error>.Failure(err)
                .Bind(x => Result<string, Error>.Success("nope"));
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void Match_DispatchesOnSuccess()
        {
            var r = Result<int, Error>.Success(5);
            string s = r.Match(v => $"ok:{v}", e => $"err:{e.Code}");
            s.Should().Be("ok:5");
        }

        [TestMethod]
        public void Match_DispatchesOnFailure()
        {
            var r = Result<int, Error>.Failure(Error.NotFound());
            string s = r.Match(v => "ok", e => e.Code);
            s.Should().Be("not_found");
        }

        [TestMethod]
        public void Ensure_FailsWhenPredicateFalse()
        {
            var err = Error.Validation("neg");
            Result<int, Error> r = Result<int, Error>.Success(-1).Ensure(x => x >= 0, err);
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void Recover_OnFailure_ProducesSuccess()
        {
            Result<int, Error> r = Result<int, Error>.Failure(Error.NotFound()).Recover(_ => 99);
            r.Value.Should().Be(99);
        }

        [TestMethod]
        public void ImplicitOperator_ConvertsValueToSuccess()
        {
            Result<string, Error> r = "hi";
            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be("hi");
        }

        [TestMethod]
        public void ToMaybe_Success_IsSome()
        {
            Result<int, Error>.Success(7).ToMaybe().HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void ToMaybe_Failure_IsNone()
        {
            Result<int, Error>.Failure(Error.NotFound()).ToMaybe().HasNoValue.Should().BeTrue();
        }

        [TestMethod]
        public void ToEither_Success_IsRight()
        {
            Result<int, Error>.Success(1).ToEither().IsRight.Should().BeTrue();
        }

        [TestMethod]
        public void ToEither_Failure_IsLeft()
        {
            Result<int, Error>.Failure(Error.NotFound()).ToEither().IsLeft.Should().BeTrue();
        }
    }
}
