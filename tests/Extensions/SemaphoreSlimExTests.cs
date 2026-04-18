using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class SemaphoreSlimExTests : BaseExtensionTest
    {
        [TestMethod]
        public async Task LockAsync_AcquiresAndReleases()
        {
            using var semaphore = new SemaphoreSlim(1, 1);

            await using (await semaphore.LockAsync(TestContext.CancellationToken))
            {
                semaphore.CurrentCount.Should().Be(0);
            }

            semaphore.CurrentCount.Should().Be(1);
        }

        [TestMethod]
        public async Task LockAsync_PreventsConurrentAccess()
        {
            using var semaphore = new SemaphoreSlim(1, 1);
            int counter = 0;
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await using IAsyncDisposable _ = await semaphore.LockAsync(TestContext.CancellationToken);
                    int before = counter;
                    await Task.Delay(10, TestContext.CancellationToken);
                    counter = before + 1;
                }, TestContext.CancellationToken));
            }

            await Task.WhenAll(tasks);
            counter.Should().Be(10);
        }

        [TestMethod]
        public async Task LockAsync_SupportsCancellation()
        {
            using var semaphore = new SemaphoreSlim(0, 1); // starts locked
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

            Func<Task> act = async () => await semaphore.LockAsync(cts.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [TestMethod]
        public async Task LockAsync_Null_ThrowsArgumentNullException()
        {
            SemaphoreSlim semaphore = null!;

            Func<Task> act = async () => await semaphore.LockAsync(TestContext.CancellationToken);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        public TestContext TestContext { get; set; } = null!;
    }
}

