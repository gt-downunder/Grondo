using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class MaybeTests : BaseTest
    {
        // --- Creation ---

        [TestMethod]
        public void Some_CreatesInstanceWithValue()
        {
            var maybe = Maybe<int>.Some(42);
            maybe.HasValue.Should().BeTrue();
            maybe.HasNoValue.Should().BeFalse();
            maybe.Value.Should().Be(42);
        }

        [TestMethod]
        public void Some_NullValue_ThrowsArgumentNullException()
        {
            Action act = () => Maybe<string>.Some(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void None_HasNoValue()
        {
            Maybe<int> maybe = Maybe<int>.None;
            maybe.HasValue.Should().BeFalse();
            maybe.HasNoValue.Should().BeTrue();
        }

        [TestMethod]
        public void None_ValueAccess_ThrowsInvalidOperationException()
        {
            Maybe<int> maybe = Maybe<int>.None;
            Func<int> act = () => maybe.Value;
            act.Should().Throw<InvalidOperationException>();
        }

        // --- Map ---

        [TestMethod]
        public void Map_Some_TransformsValue()
        {
            Maybe<int> result = Maybe<int>.Some(5).Map(x => x * 2);
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(10);
        }

        [TestMethod]
        public void Map_None_ReturnsNone()
        {
            Maybe<int> result = Maybe<int>.None.Map(x => x * 2);
            result.HasNoValue.Should().BeTrue();
        }

        // --- Bind ---

        [TestMethod]
        public void Bind_Some_ReturnsBinderResult()
        {
            Maybe<string> result = Maybe<int>.Some(5).Bind(x => Maybe<string>.Some(x.ToString()));
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("5");
        }

        [TestMethod]
        public void Bind_Some_BinderReturnsNone_ReturnsNone()
        {
            Maybe<string> result = Maybe<int>.Some(5).Bind(_ => Maybe<string>.None);
            result.HasNoValue.Should().BeTrue();
        }

        [TestMethod]
        public void Bind_None_ReturnsNone()
        {
            Maybe<string> result = Maybe<int>.None.Bind(x => Maybe<string>.Some(x.ToString()));
            result.HasNoValue.Should().BeTrue();
        }

        // --- Match ---

        [TestMethod]
        public void Match_Some_CallsSomeBranch()
        {
            var result = Maybe<int>.Some(42).Match(v => $"Value: {v}", () => "Nothing");
            result.Should().Be("Value: 42");
        }

        [TestMethod]
        public void Match_None_CallsNoneBranch()
        {
            var result = Maybe<int>.None.Match(v => $"Value: {v}", () => "Nothing");
            result.Should().Be("Nothing");
        }

        // --- GetValueOrDefault ---

        [TestMethod]
        public void GetValueOrDefault_Some_ReturnsValue() =>
            Maybe<int>.Some(42).GetValueOrDefault(0).Should().Be(42);

        [TestMethod]
        public void GetValueOrDefault_None_ReturnsDefault() =>
            Maybe<int>.None.GetValueOrDefault(99).Should().Be(99);

        // --- Where ---

        [TestMethod]
        public void Where_PredicateTrue_ReturnsSome()
        {
            Maybe<int> result = Maybe<int>.Some(10).Where(x => x > 5);
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(10);
        }

        [TestMethod]
        public void Where_PredicateFalse_ReturnsNone() =>
            Maybe<int>.Some(3).Where(x => x > 5).HasNoValue.Should().BeTrue();

        [TestMethod]
        public void Where_None_ReturnsNone() =>
            Maybe<int>.None.Where(x => x > 5).HasNoValue.Should().BeTrue();

        // --- Tap ---

        [TestMethod]
        public void Tap_Some_RunsAction()
        {
            int captured = 0;
            Maybe<int> result = Maybe<int>.Some(42).Tap(v => captured = v);
            captured.Should().Be(42);
            result.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void Tap_None_DoesNotRunAction()
        {
            int captured = 0;
            Maybe<int>.None.Tap(v => captured = v);
            captured.Should().Be(0);
        }

        // --- ToResult ---

        [TestMethod]
        public void ToResult_Some_ReturnsSuccess()
        {
            var result = Maybe<int>.Some(42).ToResult("error");
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToResult_None_ReturnsFailure()
        {
            var result = Maybe<int>.None.ToResult("not found");
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("not found");
        }

        // --- ToString ---

        [TestMethod]
        public void ToString_Some_ReturnsSomeWithValue() =>
            Maybe<int>.Some(42).ToString().Should().Be("Some(42)");

        [TestMethod]
        public void ToString_None_ReturnsNone() =>
            Maybe<int>.None.ToString().Should().Be("None");

        // --- Implicit conversion ---

        [TestMethod]
        public void ImplicitConversion_NonNull_CreatesSome()
        {
            Maybe<string> maybe = "hello";
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be("hello");
        }

        [TestMethod]
        public void ImplicitConversion_Null_CreatesNone()
        {
            Maybe<string> maybe = (string?)null;
            maybe.HasNoValue.Should().BeTrue();
        }

        // --- ToEither ---

        [TestMethod]
        public void ToEither_Some_ReturnsRight()
        {
            var either = Maybe<int>.Some(42).ToEither("no value");
            either.IsRight.Should().BeTrue();
            either.Right.Should().Be(42);
        }

        [TestMethod]
        public void ToEither_None_ReturnsLeftWithProvidedValue()
        {
            var either = Maybe<int>.None.ToEither("no value");
            either.IsLeft.Should().BeTrue();
            either.Left.Should().Be("no value");
        }

        // --- ToValidation ---

        [TestMethod]
        public void ToValidation_Some_ReturnsValid()
        {
            var validation = Maybe<int>.Some(42).ToValidation("required");
            validation.IsValid.Should().BeTrue();
            validation.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToValidation_None_ReturnsInvalidWithError()
        {
            var validation = Maybe<int>.None.ToValidation("value is required");
            validation.IsInvalid.Should().BeTrue();
            validation.Errors.Should().ContainSingle().Which.Should().Be("value is required");
        }

        // --- AsEnumerable / ToArray / ToList ---

        [TestMethod]
        public void AsEnumerable_Some_YieldsSingleElement()
        {
            Maybe<int>.Some(7).AsEnumerable().Should().BeEquivalentTo([7]);
        }

        [TestMethod]
        public void AsEnumerable_None_YieldsEmpty()
        {
            Maybe<int>.None.AsEnumerable().Should().BeEmpty();
        }

        [TestMethod]
        public void ToArray_Some_ReturnsSingleElementArray()
        {
            Maybe<string>.Some("x").ToArray().Should().BeEquivalentTo(["x"]);
        }

        [TestMethod]
        public void ToArray_None_ReturnsEmptyArray()
        {
            Maybe<string>.None.ToArray().Should().BeEmpty();
        }

        [TestMethod]
        public void ToList_Some_ReturnsSingleElementList()
        {
            Maybe<int>.Some(5).ToList().Should().BeEquivalentTo([5]);
        }

        [TestMethod]
        public void ToList_None_ReturnsEmptyList()
        {
            Maybe<int>.None.ToList().Should().BeEmpty();
        }
    }
}

