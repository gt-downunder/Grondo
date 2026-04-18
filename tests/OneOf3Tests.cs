using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class OneOf3Tests : BaseTest
    {
        [TestMethod]
        public void FromTN_SetsFlagsCorrectly()
        {
            OneOf<int, string, Guid> a = OneOf<int, string, Guid>.FromT0(1);
            OneOf<int, string, Guid> b = OneOf<int, string, Guid>.FromT1("x");
            OneOf<int, string, Guid> c = OneOf<int, string, Guid>.FromT2(Guid.NewGuid());

            a.IsT0.Should().BeTrue();
            b.IsT1.Should().BeTrue();
            c.IsT2.Should().BeTrue();

            a.Index.Should().Be(0);
            b.Index.Should().Be(1);
            c.Index.Should().Be(2);
        }

        [TestMethod]
        public void ImplicitConversions_PickCorrectSlot()
        {
            OneOf<int, string, Guid> a = 7;
            OneOf<int, string, Guid> b = "x";
            OneOf<int, string, Guid> c = Guid.NewGuid();

            a.IsT0.Should().BeTrue();
            b.IsT1.Should().BeTrue();
            c.IsT2.Should().BeTrue();
        }

        [TestMethod]
        public void Match_DispatchesOnIndex()
        {
            OneOf<int, string, double> a = 2;
            OneOf<int, string, double> b = "abc";
            OneOf<int, string, double> c = 3.5;

            a.Match(i => i * 10, _ => -1, _ => -2).Should().Be(20);
            b.Match(_ => -1, s => s.Length, _ => -2).Should().Be(3);
            c.Match(_ => -1.0, _ => -2.0, d => d * 2).Should().Be(7.0);
        }

        [TestMethod]
        public void Switch_InvokesExactlyOneAction()
        {
            OneOf<int, string, double> one = "x";
            int i = 0, s = 0, d = 0;
            one.Switch(_ => i++, _ => s++, _ => d++);
            (i, s, d).Should().Be((0, 1, 0));
        }

        [TestMethod]
        public void AsTN_WrongSlot_Throws()
        {
            OneOf<int, string, double> one = 1;
            ((Action)(() => { _ = one.AsT1; })).Should().Throw<InvalidOperationException>();
            ((Action)(() => { _ = one.AsT2; })).Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Equality_ReflectsSlotAndValue()
        {
            OneOf<int, string, double> a = 5;
            OneOf<int, string, double> b = 5;
            OneOf<int, string, double> c = "5";
            (a == b).Should().BeTrue();
            (a == c).Should().BeFalse();
        }

        [TestMethod]
        public void ToString_ContainsIndexAndValue()
        {
            OneOf<int, string, double> one = "abc";
            one.ToString().Should().Be("T1(abc)");
        }
    }
}
