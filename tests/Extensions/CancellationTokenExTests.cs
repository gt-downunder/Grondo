using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class CancellationTokenExTests : BaseExtensionTest
    {
        [TestMethod]
        public async Task AsTask_AlreadyCancelled_ReturnsCancelledTask()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Task task = cts.Token.AsTask();

            await FluentActions.Invoking(async () => await task)
                .Should().ThrowAsync<OperationCanceledException>();
        }

        [TestMethod]
        public async Task AsTask_CompletesWhenCancelled()
        {
            using var cts = new CancellationTokenSource();
            Task task = cts.Token.AsTask();
            task.IsCompleted.Should().BeFalse();

            await cts.CancelAsync();

            await FluentActions.Invoking(async () => await task)
                .Should().ThrowAsync<OperationCanceledException>();
        }

        [TestMethod]
        public async Task WithTimeout_CancelsAfterTimeout()
        {
            using var outer = new CancellationTokenSource();
            using CancellationTokenSource linked = outer.Token.WithTimeout(TimeSpan.FromMilliseconds(50));

            await Task.Delay(150);

            linked.Token.IsCancellationRequested.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithTimeout_CancelsOnOuterCancel()
        {
            using var outer = new CancellationTokenSource();
            using CancellationTokenSource linked = outer.Token.WithTimeout(TimeSpan.FromMinutes(5));

            await outer.CancelAsync();

            linked.Token.IsCancellationRequested.Should().BeTrue();
        }
    }
}
