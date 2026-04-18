using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class CombinatorsTests : BaseTest
    {
        [TestMethod]
        public void Sequence_Result_AllSuccess_ReturnsListOfValues()
        {
            Result<IReadOnlyList<int>> r = Combinators.Sequence([Result<int>.Success(1), Result<int>.Success(2)]);
            r.IsSuccess.Should().BeTrue();
            r.Value.Should().BeEquivalentTo([1, 2]);
        }

        [TestMethod]
        public void Sequence_Result_FirstFailureShortCircuits()
        {
            Result<IReadOnlyList<int>> r = Combinators.Sequence([Result<int>.Success(1), Result<int>.Failure("boom"), Result<int>.Success(3)]);
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("boom");
        }

        [TestMethod]
        public void Traverse_Result_MapsAndSequences()
        {
            Result<IReadOnlyList<int>> r = Combinators.Traverse([1, 2, 3], x => Result<int>.Success(x * 2));
            r.Value.Should().BeEquivalentTo([2, 4, 6]);
        }

        [TestMethod]
        public void Traverse_Result_PropagatesFailure()
        {
            Result<IReadOnlyList<int>> r = Combinators.Traverse<int, int>([1, 2, 3], x =>
                x == 2 ? Result<int>.Failure("stop") : Result<int>.Success(x));
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("stop");
        }

        [TestMethod]
        public void Sequence_Maybe_AllSome_ReturnsSomeList()
        {
            Maybe<IReadOnlyList<int>> r = Combinators.Sequence([Maybe<int>.Some(1), Maybe<int>.Some(2)]);
            r.HasValue.Should().BeTrue();
            r.Value.Should().BeEquivalentTo([1, 2]);
        }

        [TestMethod]
        public void Sequence_Maybe_AnyNone_ReturnsNone()
        {
            Maybe<IReadOnlyList<int>> r = Combinators.Sequence([Maybe<int>.Some(1), Maybe<int>.None, Maybe<int>.Some(3)]);
            r.HasNoValue.Should().BeTrue();
        }

        [TestMethod]
        public void Traverse_Maybe_MapsAndSequences()
        {
            Maybe<IReadOnlyList<int>> r = Combinators.Traverse([1, 2], x => Maybe<int>.Some(x + 10));
            r.Value.Should().BeEquivalentTo([11, 12]);
        }

        [TestMethod]
        public void Sequence_Validation_AccumulatesErrors()
        {
            Validation<IReadOnlyList<int>> r = Combinators.Sequence(
            [
                Validation<int>.Valid(1),
                Validation<int>.Invalid("e1"),
                Validation<int>.Invalid("e2"),
                Validation<int>.Valid(4)
            ]);
            r.IsInvalid.Should().BeTrue();
            r.Errors.Should().BeEquivalentTo(["e1", "e2"]);
        }

        [TestMethod]
        public void Sequence_Validation_AllValid_ReturnsList()
        {
            Validation<IReadOnlyList<int>> r = Combinators.Sequence([Validation<int>.Valid(1), Validation<int>.Valid(2)]);
            r.IsValid.Should().BeTrue();
            r.Value.Should().BeEquivalentTo([1, 2]);
        }

        [TestMethod]
        public void Traverse_Validation_AccumulatesAllErrors()
        {
            Validation<IReadOnlyList<int>> r = Combinators.Traverse<int, int>([-1, 2, -3], x =>
                x < 0 ? Validation<int>.Invalid($"neg:{x}") : Validation<int>.Valid(x));
            r.IsInvalid.Should().BeTrue();
            r.Errors.Should().BeEquivalentTo(["neg:-1", "neg:-3"]);
        }

        [TestMethod]
        public void Sequence_TypedResult_AllSuccess()
        {
            Result<IReadOnlyList<int>, Error> r = Combinators.Sequence<int, Error>(
            [
                Result<int, Error>.Success(1),
                Result<int, Error>.Success(2)
            ]);
            r.Value.Should().BeEquivalentTo([1, 2]);
        }

        [TestMethod]
        public void Sequence_TypedResult_ShortCircuitsOnFailure()
        {
            var err = Error.NotFound();
            Result<IReadOnlyList<int>, Error> r = Combinators.Sequence<int, Error>(
            [
                Result<int, Error>.Success(1),
                Result<int, Error>.Failure(err),
                Result<int, Error>.Success(3)
            ]);
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void Sequence_Null_Throws()
        {
            Action act = () => Combinators.Sequence((IEnumerable<Result<int>>)null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
