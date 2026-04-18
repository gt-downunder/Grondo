using FluentAssertions;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ResultTEAsyncAndLinqTests : BaseTest
    {
        [TestMethod]
        public async Task MapAsync_TransformsSuccess()
        {
            Result<int, Error> r = await Result<int, Error>.Success(2)
                .MapAsync(x => Task.FromResult(x + 3));
            r.Value.Should().Be(5);
        }

        [TestMethod]
        public async Task MapAsync_PropagatesFailure()
        {
            var err = Error.NotFound();
            Result<int, Error> r = await Result<int, Error>.Failure(err)
                .MapAsync(x => Task.FromResult(x + 1));
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public async Task BindAsync_ChainsSuccess()
        {
            Result<string, Error> r = await Result<int, Error>.Success(1)
                .BindAsync(x => Task.FromResult(Result<string, Error>.Success($"#{x}")));
            r.Value.Should().Be("#1");
        }

        [TestMethod]
        public void Select_IsMap()
        {
            Result<int, Error> r = from x in Result<int, Error>.Success(4) select x * 2;
            r.Value.Should().Be(8);
        }

        [TestMethod]
        public void SelectMany_EnablesLinqQueries()
        {
            Result<int, Error> r = from a in Result<int, Error>.Success(2)
                                   from b in Result<int, Error>.Success(3)
                                   select a + b;
            r.Value.Should().Be(5);
        }

        [TestMethod]
        public void SelectMany_ShortCircuitsOnFirstFailure()
        {
            var err = Error.Validation("bad");
            Result<int, Error> r = from a in Result<int, Error>.Failure(err)
                                   from b in Result<int, Error>.Success(3)
                                   select a + b;
            r.Error.Should().Be(err);
        }

        [TestMethod]
        public void Equals_SameValue_IsEqual()
        {
            var a = Result<int, Error>.Success(1);
            var b = Result<int, Error>.Success(1);
            a.Should().Be(b);
            (a == b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentError_NotEqual()
        {
            var a = Result<int, Error>.Failure(Error.NotFound("a"));
            var b = Result<int, Error>.Failure(Error.NotFound("b"));
            (a != b).Should().BeTrue();
        }

        [TestMethod]
        public void Tap_InvokesOnSuccessOnly()
        {
            int hits = 0;
            Result<int, Error>.Success(1).Tap(_ => hits++);
            Result<int, Error>.Failure(Error.NotFound()).Tap(_ => hits++);
            hits.Should().Be(1);
        }

        [TestMethod]
        public void TapError_InvokesOnFailureOnly()
        {
            int hits = 0;
            Result<int, Error>.Success(1).TapError(_ => hits++);
            Result<int, Error>.Failure(Error.NotFound()).TapError(_ => hits++);
            hits.Should().Be(1);
        }

        [TestMethod]
        public void GetValueOrDefault_OnFailure_ReturnsFallback()
        {
            Result<int, Error>.Failure(Error.NotFound()).GetValueOrDefault(99).Should().Be(99);
        }

        [TestMethod]
        public void ToString_DescribesState()
        {
            Result<int, Error>.Success(1).ToString().Should().Contain("Success");
            Result<int, Error>.Failure(Error.NotFound("m")).ToString().Should().Contain("Failure");
        }
    }
}
