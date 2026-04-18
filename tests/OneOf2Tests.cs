using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class OneOf2Tests : BaseTest
    {
        [TestMethod]
        public void FromT0_SetsIndexAndFlags()
        {
            OneOf<int, string> one = OneOf<int, string>.FromT0(42);
            one.Index.Should().Be(0);
            one.IsT0.Should().BeTrue();
            one.IsT1.Should().BeFalse();
            one.AsT0.Should().Be(42);
            one.Value.Should().Be(42);
        }

        [TestMethod]
        public void FromT1_SetsIndexAndFlags()
        {
            OneOf<int, string> one = OneOf<int, string>.FromT1("hello");
            one.Index.Should().Be(1);
            one.IsT1.Should().BeTrue();
            one.AsT1.Should().Be("hello");
        }

        [TestMethod]
        public void ImplicitConversion_FromT1Value_ProducesT0Slot()
        {
            OneOf<int, string> one = 7;
            one.IsT0.Should().BeTrue();
            one.AsT0.Should().Be(7);
        }

        [TestMethod]
        public void ImplicitConversion_FromT2Value_ProducesT1Slot()
        {
            OneOf<int, string> one = "x";
            one.IsT1.Should().BeTrue();
            one.AsT1.Should().Be("x");
        }

        [TestMethod]
        public void AsT0_WhenIndexIsWrong_Throws()
        {
            OneOf<int, string> one = "x";
            var act = () => one.AsT0;
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Match_DispatchesOnIndex()
        {
            OneOf<int, string> a = 2;
            OneOf<int, string> b = "hi";
            a.Match(i => i * 10, _ => 0).Should().Be(20);
            b.Match(_ => 0, s => s.Length).Should().Be(2);
        }

        [TestMethod]
        public void Switch_InvokesMatchingAction()
        {
            OneOf<int, string> one = "x";
            int iCalls = 0, sCalls = 0;
            one.Switch(_ => iCalls++, _ => sCalls++);
            iCalls.Should().Be(0);
            sCalls.Should().Be(1);
        }

        [TestMethod]
        public void MapT0_TransformsSlotZero_PreservesSlotOne()
        {
            OneOf<int, string> a = 3;
            OneOf<int, string> b = "hi";
            OneOf<string, string> ma = a.MapT0(i => $"#{i}");
            OneOf<string, string> mb = b.MapT0(i => $"#{i}");
            ma.IsT0.Should().BeTrue();
            ma.AsT0.Should().Be("#3");
            mb.IsT1.Should().BeTrue();
            mb.AsT1.Should().Be("hi");
        }

        [TestMethod]
        public void MapT1_TransformsSlotOne_PreservesSlotZero()
        {
            OneOf<int, string> a = 3;
            OneOf<int, string> b = "hi";
            OneOf<int, int> ma = a.MapT1(s => s.Length);
            OneOf<int, int> mb = b.MapT1(s => s.Length);
            ma.AsT0.Should().Be(3);
            mb.AsT1.Should().Be(2);
        }

        [TestMethod]
        public void Equals_SameSlotAndValue_AreEqual()
        {
            OneOf<int, string> a = 5;
            OneOf<int, string> b = 5;
            (a == b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentSlots_NotEqual()
        {
            OneOf<int, string> a = 5;
            OneOf<int, string> b = "5";
            (a != b).Should().BeTrue();
        }

        [TestMethod]
        public void FromT0_NullValue_Throws()
        {
            var act = () => OneOf<string, int>.FromT0(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ToString_IncludesIndexAndValue()
        {
            OneOf<int, string> one = "hi";
            one.ToString().Should().Be("T1(hi)");
        }
    }
}
