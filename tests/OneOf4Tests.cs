using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class OneOf4Tests : BaseTest
    {
        [TestMethod]
        public void FromTN_SetsFlagsCorrectly()
        {
            OneOf<int, string, double, bool> a = OneOf<int, string, double, bool>.FromT0(1);
            OneOf<int, string, double, bool> b = OneOf<int, string, double, bool>.FromT1("x");
            OneOf<int, string, double, bool> c = OneOf<int, string, double, bool>.FromT2(1.5);
            OneOf<int, string, double, bool> d = OneOf<int, string, double, bool>.FromT3(true);

            a.IsT0.Should().BeTrue();
            b.IsT1.Should().BeTrue();
            c.IsT2.Should().BeTrue();
            d.IsT3.Should().BeTrue();
        }

        [TestMethod]
        public void ImplicitConversions_PickCorrectSlot()
        {
            OneOf<int, string, double, bool> a = 1;
            OneOf<int, string, double, bool> b = "x";
            OneOf<int, string, double, bool> c = 1.5;
            OneOf<int, string, double, bool> d = true;

            a.Index.Should().Be(0);
            b.Index.Should().Be(1);
            c.Index.Should().Be(2);
            d.Index.Should().Be(3);
        }

        [TestMethod]
        public void Match_DispatchesToCorrectBranch()
        {
            OneOf<int, string, double, bool> a = 2;
            OneOf<int, string, double, bool> b = "abc";
            OneOf<int, string, double, bool> c = 1.5;
            OneOf<int, string, double, bool> d = true;

            a.Match(i => $"i={i}", _ => "s", _ => "d", _ => "b").Should().Be("i=2");
            b.Match(_ => "i", s => $"s={s}", _ => "d", _ => "b").Should().Be("s=abc");
            c.Match(_ => "i", _ => "s", x => $"d={x}", _ => "b").Should().Be("d=1.5");
            d.Match(_ => "i", _ => "s", _ => "d", x => $"b={x}").Should().Be("b=True");
        }

        [TestMethod]
        public void Switch_InvokesExactlyOneAction()
        {
            OneOf<int, string, double, bool> one = true;
            int i = 0, s = 0, d = 0, b = 0;
            one.Switch(_ => i++, _ => s++, _ => d++, _ => b++);
            (i, s, d, b).Should().Be((0, 0, 0, 1));
        }

        [TestMethod]
        public void AsTN_WrongSlot_Throws()
        {
            OneOf<int, string, double, bool> one = 1;
            ((Action)(() => { var _ = one.AsT3; })).Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Equality_ReflectsSlotAndValue()
        {
            OneOf<int, string, double, bool> a = 1;
            OneOf<int, string, double, bool> b = 1;
            OneOf<int, string, double, bool> c = "1";
            (a == b).Should().BeTrue();
            (a == c).Should().BeFalse();
        }

        [TestMethod]
        public void FromT1_NullValue_Throws()
        {
            var act = () => OneOf<int, string, double, bool>.FromT1(null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
