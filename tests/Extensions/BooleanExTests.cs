using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class BooleanExTests : BaseExtensionTest
    {
        [TestMethod]
        public void RunIfTrue_WhenTrue_ExecutesAction()
        {
            bool executed = false;
            bool result = true.RunIfTrue(() => executed = true);

            result.Should().BeTrue();
            executed.Should().BeTrue();
        }

        [TestMethod]
        public void RunIfTrue_WhenFalse_DoesNotExecuteAction()
        {
            bool executed = false;
            bool result = false.RunIfTrue(() => executed = true);

            result.Should().BeFalse();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public void RunIfFalse_WhenFalse_ExecutesAction()
        {
            bool executed = false;
            bool result = false.RunIfFalse(() => executed = true);

            result.Should().BeFalse();
            executed.Should().BeTrue();
        }

        [TestMethod]
        public void RunIfFalse_WhenTrue_DoesNotExecuteAction()
        {
            bool executed = false;
            bool result = true.RunIfFalse(() => executed = true);

            result.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public void RunIfTrue_NullAction_ThrowsArgumentNullException()
        {
            Func<bool> act = () => true.RunIfTrue(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunIfFalse_NullAction_ThrowsArgumentNullException()
        {
            Func<bool> act = () => false.RunIfFalse(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ToFalseIfNull_NullValue_ReturnsFalse()
        {
            bool? value = null;
            value.ToFalseIfNull().Should().BeFalse();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ToFalseIfNull_HasValue_ReturnsActualValue(bool expected)
        {
            bool? value = expected;
            value.ToFalseIfNull().Should().Be(expected);
        }
    }
}

