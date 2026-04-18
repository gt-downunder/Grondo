using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ValidationTests : BaseTest
    {
        // --- ToMaybe ---

        [TestMethod]
        public void ToMaybe_Valid_ReturnsSome()
        {
            Validation<int>.Valid(9).ToMaybe().HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void ToMaybe_Invalid_ReturnsNone()
        {
            Validation<int>.Invalid("bad").ToMaybe().HasNoValue.Should().BeTrue();
        }

        // --- ToEither ---

        [TestMethod]
        public void ToEither_Valid_IsRight()
        {
            var e = Validation<int>.Valid(3).ToEither();
            e.IsRight.Should().BeTrue();
            e.Right.Should().Be(3);
        }

        [TestMethod]
        public void ToEither_Invalid_IsLeftWithAllErrors()
        {
            var e = Validation<int>.Invalid("a", "b").ToEither();
            e.IsLeft.Should().BeTrue();
            e.Left.Should().BeEquivalentTo(["a", "b"]);
        }

        // --- Apply ---

        [TestMethod]
        public void Apply_BothValid_AppliesFunction()
        {
            var vf = Validation<Func<int, int>>.Valid(x => x + 10);
            Validation<int> v = Validation<int>.Valid(5).Apply(vf);
            v.IsValid.Should().BeTrue();
            v.Value.Should().Be(15);
        }

        [TestMethod]
        public void Apply_FuncInvalid_AccumulatesFuncErrors()
        {
            var vf = Validation<Func<int, int>>.Invalid("no-func");
            Validation<int> v = Validation<int>.Valid(5).Apply(vf);
            v.IsInvalid.Should().BeTrue();
            v.Errors.Should().BeEquivalentTo(["no-func"]);
        }

        [TestMethod]
        public void Apply_ValueInvalid_AccumulatesValueErrors()
        {
            var vf = Validation<Func<int, int>>.Valid(x => x);
            Validation<int> v = Validation<int>.Invalid("no-val").Apply(vf);
            v.IsInvalid.Should().BeTrue();
            v.Errors.Should().BeEquivalentTo(["no-val"]);
        }

        [TestMethod]
        public void Apply_BothInvalid_AccumulatesAllErrors()
        {
            var vf = Validation<Func<int, int>>.Invalid("e1");
            Validation<int> v = Validation<int>.Invalid("e2", "e3").Apply(vf);
            v.IsInvalid.Should().BeTrue();
            v.Errors.Should().BeEquivalentTo(["e1", "e2", "e3"]);
        }
    }
}
