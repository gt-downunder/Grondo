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

        // --- Swap ---

        [TestMethod]
        public void Swap_Right_BecomesLeft()
        {
            Either<int, string> swapped = Either<string, int>.FromRight(7).Swap();
            swapped.IsLeft.Should().BeTrue();
            swapped.Left.Should().Be(7);
        }

        [TestMethod]
        public void Swap_Left_BecomesRight()
        {
            Either<int, string> swapped = Either<string, int>.FromLeft("err").Swap();
            swapped.IsRight.Should().BeTrue();
            swapped.Right.Should().Be("err");
        }

        // --- BiMap ---

        [TestMethod]
        public void BiMap_Right_AppliesRightMapper()
        {
            Either<int, int> r = Either<string, int>.FromRight(2).BiMap(l => l.Length, r => r * 10);
            r.IsRight.Should().BeTrue();
            r.Right.Should().Be(20);
        }

        [TestMethod]
        public void BiMap_Left_AppliesLeftMapper()
        {
            Either<int, int> r = Either<string, int>.FromLeft("abc").BiMap(l => l.Length, r => r * 10);
            r.IsLeft.Should().BeTrue();
            r.Left.Should().Be(3);
        }

        // --- TapBoth ---

        [TestMethod]
        public void TapBoth_Right_InvokesRightAction()
        {
            int hitL = 0, hitR = 0;
            Either<string, int>.FromRight(1).TapBoth(_ => hitL++, _ => hitR++);
            hitL.Should().Be(0);
            hitR.Should().Be(1);
        }

        [TestMethod]
        public void TapBoth_Left_InvokesLeftAction()
        {
            int hitL = 0, hitR = 0;
            Either<string, int>.FromLeft("x").TapBoth(_ => hitL++, _ => hitR++);
            hitL.Should().Be(1);
            hitR.Should().Be(0);
        }

        // --- ToValidation ---

        [TestMethod]
        public void ToValidation_Right_IsValid()
        {
            var v = Either<int, string>.FromRight("ok").ToValidation(l => $"code={l}");
            v.IsValid.Should().BeTrue();
            v.Value.Should().Be("ok");
        }

        [TestMethod]
        public void ToValidation_Left_IsInvalidWithMappedError()
        {
            var v = Either<int, string>.FromLeft(42).ToValidation(l => $"code={l}");
            v.IsInvalid.Should().BeTrue();
            v.Errors.Should().ContainSingle().Which.Should().Be("code=42");
        }

        // --- ToResult (typed) ---

        [TestMethod]
        public void ToResultTyped_Right_IsSuccess()
        {
            var r = Either<Error, int>.FromRight(5).ToResult();
            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(5);
        }

        [TestMethod]
        public void ToResultTyped_Left_IsFailure()
        {
            var err = Error.NotFound();
            var r = Either<Error, int>.FromLeft(err).ToResult();
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be(err);
        }

        // --- LINQ ---

        [TestMethod]
        public void SelectMany_Right_ComposesQueries()
        {
            Either<string, int> r = from a in Either<string, int>.FromRight(2)
                    from b in Either<string, int>.FromRight(3)
                    select a + b;
            r.IsRight.Should().BeTrue();
            r.Right.Should().Be(5);
        }

        [TestMethod]
        public void SelectMany_LeftShortCircuits()
        {
            Either<string, int> r = from a in Either<string, int>.FromLeft("e")
                    from b in Either<string, int>.FromRight(3)
                    select a + b;
            r.IsLeft.Should().BeTrue();
            r.Left.Should().Be("e");
        }
    }
}

