using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Grondo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ResultTests : BaseTest
    {
        [TestMethod]
        public void Success_CreatesSuccessfulResult()
        {
            var result = Result<int>.Success(42);

            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public void Failure_CreatesFailedResult()
        {
            var result = Result<int>.Failure("something went wrong");

            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("something went wrong");
        }

        [TestMethod]
        public void Value_OnFailure_ThrowsInvalidOperationException()
        {
            var result = Result<int>.Failure("error");
            Func<int> act = () => result.Value;
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Error_OnSuccess_ThrowsInvalidOperationException()
        {
            var result = Result<int>.Success(1);
            Func<string> act = () => result.Error;
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void ImplicitOperator_ConvertsValueToSuccess()
        {
            Result<string> result = "hello";

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("hello");
        }

        [TestMethod]
        public void GetValueOrDefault_OnSuccess_ReturnsValue()
        {
            var result = Result<int>.Success(42);
            result.GetValueOrDefault(0).Should().Be(42);
        }

        [TestMethod]
        public void GetValueOrDefault_OnFailure_ReturnsFallback()
        {
            var result = Result<int>.Failure("err");
            result.GetValueOrDefault(99).Should().Be(99);
        }

        [TestMethod]
        public void Map_OnSuccess_TransformsValue()
        {
            var result = Result<int>.Success(5);
            Result<int> mapped = result.Map(x => x * 2);

            mapped.IsSuccess.Should().BeTrue();
            mapped.Value.Should().Be(10);
        }

        [TestMethod]
        public void Map_OnFailure_PropagatesError()
        {
            var result = Result<int>.Failure("err");
            Result<int> mapped = result.Map(x => x * 2);

            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Should().Be("err");
        }

        [TestMethod]
        public void Bind_OnSuccess_ChainsOperation()
        {
            var result = Result<int>.Success(10);
            Result<string> bound = result.Bind(x => Result<string>.Success(x.ToString()));

            bound.IsSuccess.Should().BeTrue();
            bound.Value.Should().Be("10");
        }

        [TestMethod]
        public void Bind_OnFailure_PropagatesError()
        {
            var result = Result<int>.Failure("err");
            Result<string> bound = result.Bind(x => Result<string>.Success(x.ToString()));

            bound.IsFailure.Should().BeTrue();
            bound.Error.Should().Be("err");
        }

        [TestMethod]
        public void ToResult_WrapsValueInSuccess()
        {
            var result = 42.ToResult();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public void TryExecute_SuccessfulFunc_ReturnsSuccess()
        {
            Result<int> result = ResultEx.TryExecute(() => 42);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public void TryExecute_ThrowingFunc_ReturnsFailure()
        {
            Result<int> result = ResultEx.TryExecute<int>(() => throw new InvalidOperationException("boom"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("boom");
        }

        [TestMethod]
        public async Task TryExecuteAsync_SuccessfulFunc_ReturnsSuccess()
        {
            Result<int> result = await ResultEx.TryExecuteAsync(() => Task.FromResult(42));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public async Task TryExecuteAsync_ThrowingFunc_ReturnsFailure()
        {
            Result<int> result = await ResultEx.TryExecuteAsync<int>(() => throw new InvalidOperationException("async boom"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("async boom");
        }

        // --- Match ---

        [TestMethod]
        public void Match_OnSuccess_CallsOnSuccess()
        {
            var result = Result<int>.Success(42);
            string output = result.Match(
                onSuccess: v => $"Value: {v}",
                onFailure: e => $"Error: {e}");

            output.Should().Be("Value: 42");
        }

        [TestMethod]
        public void Match_OnFailure_CallsOnFailure()
        {
            var result = Result<int>.Failure("something went wrong");
            string output = result.Match(
                onSuccess: v => $"Value: {v}",
                onFailure: e => $"Error: {e}");

            output.Should().Be("Error: something went wrong");
        }

        // --- Tap ---

        [TestMethod]
        public void Tap_OnSuccess_ExecutesAction()
        {
            int captured = 0;
            Result<int> result = Result<int>.Success(42).Tap(v => captured = v);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
            captured.Should().Be(42);
        }

        [TestMethod]
        public void Tap_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = Result<int>.Failure("err").Tap(_ => executed = true);

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        // --- MapError ---

        [TestMethod]
        public void MapError_OnFailure_TransformsError()
        {
            Result<int> result = Result<int>.Failure("not found")
                .MapError(e => $"Wrapped: {e}");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Wrapped: not found");
        }

        [TestMethod]
        public void MapError_OnSuccess_DoesNothing()
        {
            Result<int> result = Result<int>.Success(42)
                .MapError(e => $"Wrapped: {e}");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        // --- Ensure ---

        [TestMethod]
        public void Ensure_PredicatePasses_ReturnsOriginalSuccess()
        {
            Result<int> result = Result<int>.Success(10)
                .Ensure(v => v > 0, "must be positive");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(10);
        }

        [TestMethod]
        public void Ensure_PredicateFails_ReturnsFailure()
        {
            Result<int> result = Result<int>.Success(-5)
                .Ensure(v => v > 0, "must be positive");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("must be positive");
        }

        [TestMethod]
        public void Ensure_OnFailure_PropagatesOriginalError()
        {
            Result<int> result = Result<int>.Failure("original error")
                .Ensure(v => v > 0, "must be positive");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("original error");
        }

        // --- MapAsync ---

        [TestMethod]
        public async Task MapAsync_OnSuccess_TransformsValue()
        {
            Result<int> result = await Result<int>.Success(5)
                .MapAsync(v => Task.FromResult(v * 10));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(50);
        }

        [TestMethod]
        public async Task MapAsync_OnFailure_PropagatesError()
        {
            Result<int> result = await Result<int>.Failure("err")
                .MapAsync(v => Task.FromResult(v * 10));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        // --- BindAsync ---

        [TestMethod]
        public async Task BindAsync_OnSuccess_ChainsOperation()
        {
            Result<string> result = await Result<int>.Success(10)
                .BindAsync(v => Task.FromResult(Result<string>.Success($"val:{v}")));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("val:10");
        }

        [TestMethod]
        public async Task BindAsync_OnFailure_PropagatesError()
        {
            Result<string> result = await Result<int>.Failure("err")
                .BindAsync(v => Task.FromResult(Result<string>.Success($"val:{v}")));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        // --- TapAsync ---

        [TestMethod]
        public async Task TapAsync_OnSuccess_ExecutesAction()
        {
            int captured = 0;
            Result<int> result = await Result<int>.Success(42)
                .TapAsync(v => { captured = v; return Task.CompletedTask; });

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
            captured.Should().Be(42);
        }

        [TestMethod]
        public async Task TapAsync_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Result<int>.Failure("err")
                .TapAsync(_ => { executed = true; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        // --- Task<Result<T>> extensions ---

        [TestMethod]
        public async Task TaskExt_Map_OnSuccess_TransformsValue()
        {
            Result<int> result = await Task.FromResult(Result<int>.Success(5))
                .MapAsync(v => v * 2);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(10);
        }

        [TestMethod]
        public async Task TaskExt_Map_OnFailure_PropagatesError()
        {
            Result<int> result = await Task.FromResult(Result<int>.Failure("err"))
                .MapAsync(v => v * 2);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        [TestMethod]
        public async Task TaskExt_MapAsync_OnSuccess_TransformsValue()
        {
            Result<int> result = await Task.FromResult(Result<int>.Success(5))
                .MapAsync(v => Task.FromResult(v * 3));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(15);
        }

        [TestMethod]
        public async Task TaskExt_Bind_OnSuccess_ChainsOperation()
        {
            Result<string> result = await Task.FromResult(Result<int>.Success(10))
                .BindAsync(v => Result<string>.Success($"val:{v}"));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("val:10");
        }

        [TestMethod]
        public async Task TaskExt_BindAsync_OnSuccess_ChainsOperation()
        {
            Result<string> result = await Task.FromResult(Result<int>.Success(10))
                .BindAsync(v => Task.FromResult(Result<string>.Success($"val:{v}")));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("val:10");
        }

        [TestMethod]
        public async Task TaskExt_Tap_OnSuccess_ExecutesAction()
        {
            int captured = 0;
            Result<int> result = await Task.FromResult(Result<int>.Success(7))
                .TapAsync(v => captured = v);

            result.Value.Should().Be(7);
            captured.Should().Be(7);
        }

        [TestMethod]
        public async Task TaskExt_TapAsync_OnSuccess_ExecutesAction()
        {
            int captured = 0;
            Result<int> result = await Task.FromResult(Result<int>.Success(7))
                .TapAsync(v => { captured = v; return Task.CompletedTask; });

            result.Value.Should().Be(7);
            captured.Should().Be(7);
        }

        [TestMethod]
        public async Task TaskExt_Ensure_PredicateFails_ReturnsFailure()
        {
            Result<int> result = await Task.FromResult(Result<int>.Success(-1))
                .EnsureAsync(v => v > 0, "must be positive");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("must be positive");
        }

        [TestMethod]
        public async Task TaskExt_MapError_OnFailure_TransformsError()
        {
            Result<int> result = await Task.FromResult(Result<int>.Failure("not found"))
                .MapErrorAsync(e => $"Wrapped: {e}");

            result.Error.Should().Be("Wrapped: not found");
        }

        [TestMethod]
        public async Task TaskExt_Match_OnSuccess_CallsOnSuccess()
        {
            string output = await Task.FromResult(Result<int>.Success(42))
                .MatchAsync(v => $"Value: {v}", e => $"Error: {e}");

            output.Should().Be("Value: 42");
        }

        [TestMethod]
        public async Task TaskExt_MapAsync_OnFailure_PropagatesError()
        {
            Result<int> result = await Task.FromResult(Result<int>.Failure("err"))
                .MapAsync(v => Task.FromResult(v * 3));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        [TestMethod]
        public async Task TaskExt_Bind_OnFailure_PropagatesError()
        {
            Result<string> result = await Task.FromResult(Result<int>.Failure("err"))
                .BindAsync(v => Result<string>.Success($"val:{v}"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        [TestMethod]
        public async Task TaskExt_BindAsync_OnFailure_PropagatesError()
        {
            Result<string> result = await Task.FromResult(Result<int>.Failure("err"))
                .BindAsync(v => Task.FromResult(Result<string>.Success($"val:{v}")));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        [TestMethod]
        public async Task TaskExt_Tap_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Task.FromResult(Result<int>.Failure("err"))
                .TapAsync(v => executed = true);

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TaskExt_TapAsync_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Task.FromResult(Result<int>.Failure("err"))
                .TapAsync(_ => { executed = true; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TaskExt_Ensure_PredicatePasses_ReturnsOriginalSuccess()
        {
            Result<int> result = await Task.FromResult(Result<int>.Success(10))
                .EnsureAsync(v => v > 0, "must be positive");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(10);
        }

        [TestMethod]
        public async Task TaskExt_Ensure_OnFailure_PropagatesOriginalError()
        {
            Result<int> result = await Task.FromResult(Result<int>.Failure("original error"))
                .EnsureAsync(v => v > 0, "must be positive");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("original error");
        }

        [TestMethod]
        public async Task TaskExt_MapError_OnSuccess_DoesNothing()
        {
            Result<int> result = await Task.FromResult(Result<int>.Success(42))
                .MapErrorAsync(e => $"Wrapped: {e}");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public async Task TaskExt_Match_OnFailure_CallsOnFailure()
        {
            string output = await Task.FromResult(Result<int>.Failure("not found"))
                .MatchAsync(v => $"Value: {v}", e => $"Error: {e}");

            output.Should().Be("Error: not found");
        }

        [TestMethod]
        public void Bind_OnSuccess_BinderReturnsFailure_PropagatesBinderError()
        {
            Result<string> result = Result<int>.Success(10)
                .Bind(v => Result<string>.Failure("binder failed"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("binder failed");
        }

        [TestMethod]
        public async Task BindAsync_OnSuccess_BinderReturnsFailure_PropagatesBinderError()
        {
            Result<string> result = await Result<int>.Success(10)
                .BindAsync(v => Task.FromResult(Result<string>.Failure("binder failed")));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("binder failed");
        }

        [TestMethod]
        public void Combine_Collection_MultipleFailures_ReturnsFirstError()
        {
            var combined = ResultEx.Combine(new[]
            {
                Result<int>.Success(1),
                Result<int>.Failure("second failed"),
                Result<int>.Failure("third failed")
            });

            combined.IsFailure.Should().BeTrue();
            combined.Error.Should().Be("second failed");
        }

        // --- Full async pipeline ---

        [TestMethod]
        public async Task FullPipeline_ChainsMultipleOperations()
        {
            Result<string> result = await Task.FromResult(Result<int>.Success(5))
                .MapAsync(v => v * 2)
                .EnsureAsync(v => v > 0, "must be positive")
                .MapAsync(v => Task.FromResult(v.ToString()))
                .TapAsync(s => { /* side-effect */ });

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("10");
        }

        [TestMethod]
        public async Task FullPipeline_ShortCircuitsOnFailure()
        {
            bool tapped = false;
            Result<int> result = await Task.FromResult(Result<int>.Success(-5))
                .EnsureAsync(v => v > 0, "must be positive")
                .MapAsync(v => v * 2)
                .TapAsync(v => tapped = true)
                .MapErrorAsync(e => $"Pipeline failed: {e}");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Pipeline failed: must be positive");
            tapped.Should().BeFalse();
        }

        // --- TapError ---

        [TestMethod]
        public void TapError_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result<int> result = Result<int>.Failure("oops").TapError(e => captured = e);

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public void TapError_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = Result<int>.Success(42).TapError(_ => executed = true);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TapErrorAsync_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result<int> result = await Result<int>.Failure("oops")
                .TapErrorAsync(e => { captured = e; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public async Task TapErrorAsync_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Result<int>.Success(42)
                .TapErrorAsync(_ => { executed = true; return Task.CompletedTask; });

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TaskExt_TapError_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result<int> result = await Task.FromResult(Result<int>.Failure("oops"))
                .TapErrorAsync(e => captured = e);

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public async Task TaskExt_TapError_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Task.FromResult(Result<int>.Success(42))
                .TapErrorAsync(_ => executed = true);

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TaskExt_TapErrorAsync_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result<int> result = await Task.FromResult(Result<int>.Failure("oops"))
                .TapErrorAsync(e => { captured = e; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public async Task TaskExt_TapErrorAsync_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result<int> result = await Task.FromResult(Result<int>.Success(42))
                .TapErrorAsync(_ => { executed = true; return Task.CompletedTask; });

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeFalse();
        }

        // --- ToString ---

        [TestMethod]
        public void ToString_OnSuccess_ReturnsFormattedString()
        {
            var result = Result<int>.Success(42);
            result.ToString().Should().Be("Success(42)");
        }

        [TestMethod]
        public void ToString_OnFailure_ReturnsFormattedString()
        {
            var result = Result<int>.Failure("not found");
            result.ToString().Should().Be("Failure(not found)");
        }

        // --- Combine ---

        [TestMethod]
        public void Combine_TwoSuccesses_ReturnsTuple()
        {
            var combined = ResultEx.Combine(Result<int>.Success(1), Result<string>.Success("two"));

            combined.IsSuccess.Should().BeTrue();
            combined.Value.Should().Be((1, "two"));
        }

        [TestMethod]
        public void Combine_FirstFailure_ReturnsFirstError()
        {
            var first = Result<int>.Failure("first failed");
            var second = Result<string>.Success("two");

            Result<(int, string)> combined = ResultEx.Combine(first, second);

            combined.IsFailure.Should().BeTrue();
            combined.Error.Should().Be("first failed");
        }

        [TestMethod]
        public void Combine_SecondFailure_ReturnsSecondError()
        {
            var first = Result<int>.Success(1);
            var second = Result<string>.Failure("second failed");

            Result<(int, string)> combined = ResultEx.Combine(first, second);

            combined.IsFailure.Should().BeTrue();
            combined.Error.Should().Be("second failed");
        }

        [TestMethod]
        public void Combine_BothFailures_ReturnsFirstError()
        {
            var first = Result<int>.Failure("first failed");
            var second = Result<string>.Failure("second failed");

            Result<(int, string)> combined = ResultEx.Combine(first, second);

            combined.IsFailure.Should().BeTrue();
            combined.Error.Should().Be("first failed");
        }

        [TestMethod]
        public void Combine_Collection_AllSuccesses_ReturnsList()
        {
            Result<int>[] results = new[]
            {
                Result<int>.Success(1),
                Result<int>.Success(2),
                Result<int>.Success(3)
            };

            Result<IReadOnlyList<int>> combined = ResultEx.Combine(results);

            combined.IsSuccess.Should().BeTrue();
            combined.Value.Should().BeEquivalentTo([1, 2, 3], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void Combine_Collection_OneFailure_ReturnsFirstError()
        {
            Result<int>[] results = new[]
            {
                Result<int>.Success(1),
                Result<int>.Failure("second failed"),
                Result<int>.Success(3)
            };

            Result<IReadOnlyList<int>> combined = ResultEx.Combine(results);

            combined.IsFailure.Should().BeTrue();
            combined.Error.Should().Be("second failed");
        }

        [TestMethod]
        public void Combine_Collection_Empty_ReturnsEmptyList()
        {
            Result<IReadOnlyList<int>> combined = ResultEx.Combine(Array.Empty<Result<int>>());

            combined.IsSuccess.Should().BeTrue();
            combined.Value.Should().BeEmpty();
        }
    }

    [TestClass]
    [TestCategory("Unit")]
    public class NonGenericResultTests : BaseTest
    {
        // --- Creation ---

        [TestMethod]
        public void Success_CreatesSuccessfulResult()
        {
            var result = Result.Success();

            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }

        [TestMethod]
        public void Failure_CreatesFailedResult()
        {
            var result = Result.Failure("something went wrong");

            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("something went wrong");
        }

        [TestMethod]
        public void Error_OnSuccess_ThrowsInvalidOperationException()
        {
            var result = Result.Success();
            Func<string> act = () => result.Error;
            act.Should().Throw<InvalidOperationException>();
        }

        // --- Tap ---

        [TestMethod]
        public void Tap_OnSuccess_ExecutesAction()
        {
            bool executed = false;
            Result result = Result.Success().Tap(() => executed = true);

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeTrue();
        }

        [TestMethod]
        public void Tap_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result result = Result.Failure("err").Tap(() => executed = true);

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        // --- TapError ---

        [TestMethod]
        public void TapError_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result result = Result.Failure("oops").TapError(e => captured = e);

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public void TapError_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result result = Result.Success().TapError(_ => executed = true);

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeFalse();
        }

        // --- MapError ---

        [TestMethod]
        public void MapError_OnFailure_TransformsError()
        {
            Result result = Result.Failure("not found")
                .MapError(e => $"Wrapped: {e}");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Wrapped: not found");
        }

        [TestMethod]
        public void MapError_OnSuccess_DoesNothing()
        {
            Result result = Result.Success()
                .MapError(e => $"Wrapped: {e}");

            result.IsSuccess.Should().BeTrue();
        }

        // --- Ensure ---

        [TestMethod]
        public void Ensure_PredicatePasses_ReturnsOriginalSuccess()
        {
            Result result = Result.Success()
                .Ensure(() => true, "should not fail");

            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void Ensure_PredicateFails_ReturnsFailure()
        {
            Result result = Result.Success()
                .Ensure(() => false, "condition failed");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("condition failed");
        }

        [TestMethod]
        public void Ensure_OnFailure_PropagatesOriginalError()
        {
            Result result = Result.Failure("original error")
                .Ensure(() => true, "should not matter");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("original error");
        }

        // --- Match ---

        [TestMethod]
        public void Match_OnSuccess_CallsOnSuccess()
        {
            string output = Result.Success().Match(
                onSuccess: () => "ok",
                onFailure: e => $"Error: {e}");

            output.Should().Be("ok");
        }

        [TestMethod]
        public void Match_OnFailure_CallsOnFailure()
        {
            string output = Result.Failure("bad").Match(
                onSuccess: () => "ok",
                onFailure: e => $"Error: {e}");

            output.Should().Be("Error: bad");
        }

        // --- Bind ---

        [TestMethod]
        public void Bind_OnSuccess_ChainsOperation()
        {
            Result<int> bound = Result.Success().Bind(() => Result<int>.Success(42));

            bound.IsSuccess.Should().BeTrue();
            bound.Value.Should().Be(42);
        }

        [TestMethod]
        public void Bind_OnFailure_PropagatesError()
        {
            Result<int> bound = Result.Failure("err").Bind(() => Result<int>.Success(42));

            bound.IsFailure.Should().BeTrue();
            bound.Error.Should().Be("err");
        }

        // --- Async ---

        [TestMethod]
        public async Task TapAsync_OnSuccess_ExecutesAction()
        {
            bool executed = false;
            Result result = await Result.Success()
                .TapAsync(() => { executed = true; return Task.CompletedTask; });

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeTrue();
        }

        [TestMethod]
        public async Task TapAsync_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result result = await Result.Failure("err")
                .TapAsync(() => { executed = true; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TapErrorAsync_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result result = await Result.Failure("oops")
                .TapErrorAsync(e => { captured = e; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public async Task TapErrorAsync_OnSuccess_DoesNotExecuteAction()
        {
            bool executed = false;
            Result result = await Result.Success()
                .TapErrorAsync(_ => { executed = true; return Task.CompletedTask; });

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task BindAsync_OnSuccess_ChainsOperation()
        {
            Result<int> result = await Result.Success()
                .BindAsync(() => Task.FromResult(Result<int>.Success(42)));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public async Task BindAsync_OnFailure_PropagatesError()
        {
            Result<int> result = await Result.Failure("err")
                .BindAsync(() => Task.FromResult(Result<int>.Success(42)));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        // --- ToString ---

        [TestMethod]
        public void ToString_OnSuccess_ReturnsSuccess()
        {
            Result.Success().ToString().Should().Be("Success");
        }

        [TestMethod]
        public void ToString_OnFailure_ReturnsFailureWithError()
        {
            Result.Failure("not found").ToString().Should().Be("Failure(not found)");
        }

        // --- TryExecute ---

        [TestMethod]
        public void TryExecute_SuccessfulAction_ReturnsSuccess()
        {
            Result result = ResultEx.TryExecute(() => { /* no-op */ });

            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void TryExecute_ThrowingAction_ReturnsFailure()
        {
            Result result = ResultEx.TryExecute(() => throw new InvalidOperationException("boom"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("boom");
        }

        [TestMethod]
        public async Task TryExecuteAsync_SuccessfulFunc_ReturnsSuccess()
        {
            Result result = await ResultEx.TryExecuteAsync(() => Task.CompletedTask);

            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task TryExecuteAsync_ThrowingFunc_ReturnsFailure()
        {
            Result result = await ResultEx.TryExecuteAsync(
                () => throw new InvalidOperationException("async boom"));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("async boom");
        }

        // --- Task<Result> extensions ---

        [TestMethod]
        public async Task TaskExt_Tap_OnSuccess_ExecutesAction()
        {
            bool executed = false;
            Result result = await Task.FromResult(Result.Success())
                .TapAsync(() => executed = true);

            result.IsSuccess.Should().BeTrue();
            executed.Should().BeTrue();
        }

        [TestMethod]
        public async Task TaskExt_Tap_OnFailure_DoesNotExecuteAction()
        {
            bool executed = false;
            Result result = await Task.FromResult(Result.Failure("err"))
                .TapAsync(() => executed = true);

            result.IsFailure.Should().BeTrue();
            executed.Should().BeFalse();
        }

        [TestMethod]
        public async Task TaskExt_TapErrorAsync_OnFailure_ExecutesAction()
        {
            string captured = "";
            Result result = await Task.FromResult(Result.Failure("oops"))
                .TapErrorAsync(e => { captured = e; return Task.CompletedTask; });

            result.IsFailure.Should().BeTrue();
            captured.Should().Be("oops");
        }

        [TestMethod]
        public async Task TaskExt_Ensure_PredicateFails_ReturnsFailure()
        {
            Result result = await Task.FromResult(Result.Success())
                .EnsureAsync(() => false, "condition failed");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("condition failed");
        }

        [TestMethod]
        public async Task TaskExt_MapError_OnFailure_TransformsError()
        {
            Result result = await Task.FromResult(Result.Failure("not found"))
                .MapErrorAsync(e => $"Wrapped: {e}");

            result.Error.Should().Be("Wrapped: not found");
        }

        [TestMethod]
        public async Task TaskExt_Match_OnSuccess_CallsOnSuccess()
        {
            string output = await Task.FromResult(Result.Success())
                .MatchAsync(() => "ok", e => $"Error: {e}");

            output.Should().Be("ok");
        }

        [TestMethod]
        public async Task TaskExt_Match_OnFailure_CallsOnFailure()
        {
            string output = await Task.FromResult(Result.Failure("bad"))
                .MatchAsync(() => "ok", e => $"Error: {e}");

            output.Should().Be("Error: bad");
        }

        [TestMethod]
        public async Task TaskExt_Bind_OnSuccess_ChainsOperation()
        {
            Result<int> result = await Task.FromResult(Result.Success())
                .BindAsync(() => Result<int>.Success(42));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public async Task TaskExt_BindAsync_OnSuccess_ChainsOperation()
        {
            Result<int> result = await Task.FromResult(Result.Success())
                .BindAsync(() => Task.FromResult(Result<int>.Success(42)));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
        }

        [TestMethod]
        public async Task TaskExt_BindAsync_OnFailure_PropagatesError()
        {
            Result<int> result = await Task.FromResult(Result.Failure("err"))
                .BindAsync(() => Task.FromResult(Result<int>.Success(42)));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("err");
        }

        // --- Full pipeline ---

        [TestMethod]
        public async Task FullPipeline_NonGenericResult_ChainsOperations()
        {
            string log = "";
            Result<int> result = await Task.FromResult(Result.Success())
                .TapAsync(() => log += "started;")
                .EnsureAsync(() => true, "should not fail")
                .TapAsync(() => { log += "validated;"; return Task.CompletedTask; })
                .BindAsync(() => Result<int>.Success(42));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(42);
            log.Should().Be("started;validated;");
        }

        // --- ToMaybe ---

        [TestMethod]
        public void ToMaybe_Success_ReturnsSome()
        {
            var maybe = Result<int>.Success(42).ToMaybe();
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToMaybe_Failure_ReturnsNone()
        {
            var maybe = Result<int>.Failure("error").ToMaybe();
            maybe.HasNoValue.Should().BeTrue();
        }

        // --- ToEither ---

        [TestMethod]
        public void ToEither_Success_ReturnsRight()
        {
            var either = Result<int>.Success(42).ToEither();
            either.IsRight.Should().BeTrue();
            either.Right.Should().Be(42);
        }

        [TestMethod]
        public void ToEither_Failure_ReturnsLeftWithError()
        {
            var either = Result<int>.Failure("something broke").ToEither();
            either.IsLeft.Should().BeTrue();
            either.Left.Should().Be("something broke");
        }

        // --- ToValidation ---

        [TestMethod]
        public void ToValidation_Success_ReturnsValid()
        {
            var validation = Result<int>.Success(42).ToValidation();
            validation.IsValid.Should().BeTrue();
            validation.Value.Should().Be(42);
        }

        [TestMethod]
        public void ToValidation_Failure_ReturnsInvalidWithError()
        {
            var validation = Result<int>.Failure("bad input").ToValidation();
            validation.IsInvalid.Should().BeTrue();
            validation.Errors.Should().ContainSingle().Which.Should().Be("bad input");
        }
    }
}

