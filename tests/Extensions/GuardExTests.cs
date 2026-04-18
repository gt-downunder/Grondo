using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class GuardExTests : BaseExtensionTest
    {
        [TestMethod]
        public void ThrowIfNull_NonNull_ReturnsValue()
        {
            "hello".ThrowIfNull("param").Should().Be("hello");
        }

        [TestMethod]
        public void ThrowIfNull_Null_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(string).ThrowIfNull("myParam"))
                .Should().Throw<ArgumentNullException>().WithParameterName("myParam");
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_ValidString_ReturnsValue() => "test".ThrowIfNullOrWhiteSpace("p").Should().Be("test");

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_Null_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(string).ThrowIfNullOrWhiteSpace("p"))
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_Whitespace_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => "  ".ThrowIfNullOrWhiteSpace("p"))
                .Should().Throw<ArgumentException>().WithParameterName("p");
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_Empty_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => "".ThrowIfNullOrWhiteSpace("p"))
                .Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ThrowIfDefault_NonDefault_ReturnsValue() => 42.ThrowIfDefault("p").Should().Be(42);

        [TestMethod]
        public void ThrowIfDefault_DefaultInt_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => 0.ThrowIfDefault("p"))
                .Should().Throw<ArgumentException>().WithParameterName("p");
        }

        [TestMethod]
        public void ThrowIfDefault_DefaultGuid_ThrowsArgumentException()
        {
            Func<Guid> act = () => Guid.Empty.ThrowIfDefault("id");
            act.Should().Throw<ArgumentException>().WithParameterName("id");
        }

        [TestMethod]
        public void ThrowIfEmpty_NonEmptyCollection_ReturnsCollection()
        {
            new[] { 1, 2, 3 }.ThrowIfEmpty("items").Should().BeEquivalentTo([1, 2, 3]);
        }

        [TestMethod]
        public void ThrowIfEmpty_NullCollection_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(IEnumerable<int>).ThrowIfEmpty("items"))
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowIfEmpty_EmptyCollection_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => Array.Empty<int>().ThrowIfEmpty("items"))
                .Should().Throw<ArgumentException>().WithParameterName("items");
        }

        // --- Numeric Guards ---

        [TestMethod]
        public void ThrowIfNegative_PositiveValue_ReturnsValue() => 5.ThrowIfNegative("n").Should().Be(5);

        [TestMethod]
        public void ThrowIfNegative_Zero_ReturnsZero() => 0.ThrowIfNegative("n").Should().Be(0);

        [TestMethod]
        public void ThrowIfNegative_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            FluentActions.Invoking(() => (-1).ThrowIfNegative("n"))
                .Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
        }

        [TestMethod]
        public void ThrowIfNegative_DoubleNegative_Throws()
        {
            FluentActions.Invoking(() => (-0.5).ThrowIfNegative("d"))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void ThrowIfNegativeOrZero_PositiveValue_ReturnsValue() => 10.ThrowIfNegativeOrZero("n").Should().Be(10);

        [TestMethod]
        public void ThrowIfNegativeOrZero_Zero_Throws()
        {
            FluentActions.Invoking(() => 0.ThrowIfNegativeOrZero("n"))
                .Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
        }

        [TestMethod]
        public void ThrowIfNegativeOrZero_Negative_Throws()
        {
            FluentActions.Invoking(() => (-5).ThrowIfNegativeOrZero("n"))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void ThrowIfZero_NonZero_ReturnsValue() => 42.ThrowIfZero("n").Should().Be(42);

        [TestMethod]
        public void ThrowIfZero_Zero_Throws()
        {
            FluentActions.Invoking(() => 0.ThrowIfZero("n"))
                .Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
        }

        [TestMethod]
        public void ThrowIfZero_NegativeValue_ReturnsValue() => (-3).ThrowIfZero("n").Should().Be(-3);

        [TestMethod]
        public void ThrowIfOutOfRange_InRange_ReturnsValue() => 5.ThrowIfOutOfRange(1, 10, "n").Should().Be(5);

        [TestMethod]
        public void ThrowIfOutOfRange_AtMin_ReturnsValue() => 1.ThrowIfOutOfRange(1, 10, "n").Should().Be(1);

        [TestMethod]
        public void ThrowIfOutOfRange_AtMax_ReturnsValue() => 10.ThrowIfOutOfRange(1, 10, "n").Should().Be(10);

        [TestMethod]
        public void ThrowIfOutOfRange_BelowMin_Throws()
        {
            FluentActions.Invoking(() => 0.ThrowIfOutOfRange(1, 10, "n"))
                .Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
        }

        [TestMethod]
        public void ThrowIfOutOfRange_AboveMax_Throws()
        {
            FluentActions.Invoking(() => 11.ThrowIfOutOfRange(1, 10, "n"))
                .Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
        }

        // --- Predicate Guard ---

        [TestMethod]
        public void ThrowIf_PredicateFalse_ReturnsValue() =>
            42.ThrowIf(x => x > 100, "Too large", "n").Should().Be(42);

        [TestMethod]
        public void ThrowIf_PredicateTrue_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => 150.ThrowIf(x => x > 100, "Too large", "n"))
                .Should().Throw<ArgumentException>().WithParameterName("n")
                .WithMessage("Too large*");
        }

        [TestMethod]
        public void ThrowIf_NullPredicate_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => 42.ThrowIf(null!, "msg", "n"))
                .Should().Throw<ArgumentNullException>();
        }

        // --- Regex Format Guard ---

        [TestMethod]
        public void ThrowIfInvalidFormat_ValidFormat_ReturnsValue() =>
            "ABC-123".ThrowIfInvalidFormat(@"^[A-Z]+-\d+$", "code").Should().Be("ABC-123");

        [TestMethod]
        public void ThrowIfInvalidFormat_InvalidFormat_ThrowsArgumentException()
        {
            FluentActions.Invoking(() => "invalid".ThrowIfInvalidFormat(@"^\d+$", "num"))
                .Should().Throw<ArgumentException>().WithParameterName("num");
        }

        [TestMethod]
        public void ThrowIfInvalidFormat_NullValue_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => default(string).ThrowIfInvalidFormat(@"\d+", "p"))
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowIfLessThan_Below_Throws() =>
            FluentActions.Invoking(() => 3.ThrowIfLessThan(5)).Should().Throw<ArgumentOutOfRangeException>();

        [TestMethod]
        public void ThrowIfLessThan_AtOrAbove_Returns() =>
            5.ThrowIfLessThan(5).Should().Be(5);

        [TestMethod]
        public void ThrowIfGreaterThan_Above_Throws() =>
            FluentActions.Invoking(() => 11.ThrowIfGreaterThan(10)).Should().Throw<ArgumentOutOfRangeException>();

        [TestMethod]
        public void ThrowIfGreaterThan_AtOrBelow_Returns() =>
            10.ThrowIfGreaterThan(10).Should().Be(10);
    }
}
