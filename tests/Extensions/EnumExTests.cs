using System.Runtime.Serialization;
using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class EnumExTests : BaseExtensionTest
    {
        private enum TestEnum
        {
            [System.ComponentModel.Description("First value description")]
            [EnumMember(Value = "first_value")]
            First,

            Second,

            [System.ComponentModel.Description("Third value description")]
            Third
        }

        [TestMethod]
        public void GetDescription_HasDescriptionAttribute_ReturnsDescription() => TestEnum.First.GetDescription().Should().Be("First value description");

        [TestMethod]
        public void GetDescription_NoDescriptionAttribute_ReturnsName() => TestEnum.Second.GetDescription().Should().Be("Second");

        [TestMethod]
        public void GetDescription_NullValue_ThrowsArgumentNullException()
        {
            Enum value = null!;
            Func<string> act = () => value.GetDescription();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetCustomAttribute_HasAttribute_ReturnsAttribute()
        {
            System.ComponentModel.DescriptionAttribute? attr = TestEnum.First.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            attr.Should().NotBeNull();
            attr!.Description.Should().Be("First value description");
        }

        [TestMethod]
        public void GetCustomAttribute_NoAttribute_ReturnsNull()
        {
            System.ComponentModel.DescriptionAttribute? attr = TestEnum.Second.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            attr.Should().BeNull();
        }

        [TestMethod]
        public void GetEnumMemberValue_HasEnumMember_ReturnsValue() => TestEnum.First.GetEnumMemberValue().Should().Be("first_value");

        [TestMethod]
        public void GetEnumMemberValue_NoEnumMember_ReturnsNull() => TestEnum.Second.GetEnumMemberValue().Should().BeNull();
    }
}

