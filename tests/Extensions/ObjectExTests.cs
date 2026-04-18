using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ObjectExTests : BaseExtensionTest
    {
        [TestMethod]
        public void IsNumeric_IntValue_ReturnsTrue() => ((object)42).IsNumeric().Should().BeTrue();

        [TestMethod]
        public void IsNumeric_DoubleValue_ReturnsTrue() => ((object)3.14).IsNumeric().Should().BeTrue();

        [TestMethod]
        public void IsNumeric_NumericString_ReturnsTrue() { object value = "123.45"; value.IsNumeric().Should().BeTrue(); }

        [TestMethod]
        public void IsNumeric_NonNumericString_ReturnsFalse() { object value = "hello"; value.IsNumeric().Should().BeFalse(); }

        [TestMethod]
        public void IsNumeric_Null_ReturnsFalse() => default(object?).IsNumeric().Should().BeFalse();

        [TestMethod]
        public void IsNumeric_BoolValue_ReturnsFalse() => ((object)true).IsNumeric().Should().BeFalse();

        [TestMethod]
        public void ToNullableDouble_ValidString_ReturnsValue() { object value = "3.14"; value.ToNullableDouble().Should().BeApproximately(3.14, 0.001); }

        [TestMethod]
        public void ToNullableDouble_Invalid_ReturnsNull() { object value = "abc"; value.ToNullableDouble().Should().BeNull(); }

        [TestMethod]
        public void ToNullableDouble_Null_ReturnsNull() => default(object?).ToNullableDouble().Should().BeNull();

        [TestMethod]
        public void ToNullableInteger_ValidString_ReturnsValue() { object value = "42"; value.ToNullableInteger().Should().Be(42); }

        [TestMethod]
        public void ToNullableInteger_Invalid_ReturnsNull() { object value = "xyz"; value.ToNullableInteger().Should().BeNull(); }

        [TestMethod]
        public void ToNullableBoolean_TrueString_ReturnsTrue() { object value = "true"; value.ToNullableBoolean().Should().BeTrue(); }

        [TestMethod]
        public void ToNullableBoolean_Invalid_ReturnsNull() { object value = "maybe"; value.ToNullableBoolean().Should().BeNull(); }

        [TestMethod]
        public void ToNullableDateTime_ValidDate_ReturnsValue()
        {
            object value = "2024-01-15";
            DateTime? result = value.ToNullableDateTime();
            result.Should().NotBeNull();
            result!.Value.Year.Should().Be(2024);
        }

        [TestMethod]
        public void ToNullableDateTime_WithFormat_ReturnsValue()
        {
            object value = "15/01/2024";
            value.ToNullableDateTime("dd/MM/yyyy")!.Value.Day.Should().Be(15);
        }

        [TestMethod]
        public void ToNullableDateTime_Invalid_ReturnsNull() { object value = "not-a-date"; value.ToNullableDateTime().Should().BeNull(); }

        [TestMethod]
        public void ToStringContent_StringObject_CreatesContent()
        {
            "hello".ToStringContent("text/plain").Headers.ContentType!.MediaType.Should().Be("text/plain");
        }

        [TestMethod]
        public void ToStringContent_NonStringObject_SerializesToJson()
        {
            new { Name = "test" }.ToStringContent("application/json").Headers.ContentType!.MediaType.Should().Be("application/json");
        }

        // --- Pipe ---

        [TestMethod]
        public void Pipe_ExecutesSideEffect_ReturnsOriginal()
        {
            int sideEffect = 0;
            42.Pipe(x => sideEffect = x).Should().Be(42);
            sideEffect.Should().Be(42);
        }

        [TestMethod]
        public void Pipe_InFluentChain_MaintainsFlow()
        {
            string? captured = null;
            "hello".Pipe(s => captured = s).ToUpper().Should().Be("HELLO");
            captured.Should().Be("hello");
        }

        [TestMethod]
        public void Pipe_NullAction_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => 42.Pipe(default(Action<int>)!))
                .Should().Throw<ArgumentNullException>();
        }
    }
}

