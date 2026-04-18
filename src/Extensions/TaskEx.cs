namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Task"/> and <see cref="Task{TResult}"/>.
    /// </summary>
    public static class TaskEx
    {
        /// <summary>
        /// Executes a sequence of task factories one at a time, in order.
        /// Each task is awaited before starting the next.
        /// </summary>
        /// <param name="taskFactories">The ordered sequence of task factories to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskFactories"/> is <c>null</c>.</exception>
        public static async Task WhenAllSequentialAsync(params IEnumerable<Func<Task>> taskFactories)
        {
            ArgumentNullException.ThrowIfNull(taskFactories);

            foreach (Func<Task> factory in taskFactories)
            {
                await factory().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes a sequence of task factories one at a time, in order, collecting results.
        /// Each task is awaited before starting the next.
        /// </summary>
        /// <typeparam name="T">The result type of each task.</typeparam>
        /// <param name="taskFactories">The ordered sequence of task factories to execute.</param>
        /// <returns>An array of results in the order the tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskFactories"/> is <c>null</c>.</exception>
        public static async Task<T[]> WhenAllSequentialAsync<T>(params IEnumerable<Func<Task<T>>> taskFactories)
        {
            ArgumentNullException.ThrowIfNull(taskFactories);

            var results = new List<T>();
            foreach (Func<Task<T>> factory in taskFactories)
            {
                results.Add(await factory().ConfigureAwait(false));
            }

            return [.. results];
        }

        /// <summary>
        /// Retries the specified asynchronous operation up to the specified number of times with a delay between attempts.
        /// Uses exponential backoff by default (delay doubles after each retry).
        /// </summary>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <param name="operation">The asynchronous operation to retry.</param>
        /// <param name="maxRetries">The maximum number of retry attempts. Defaults to 3.</param>
        /// <param name="delay">The initial delay between retries. Defaults to 1 second.</param>
        /// <param name="exponentialBackoff">Whether to double the delay after each retry. Defaults to <c>true</c>.</param>
        /// <param name="exceptionFilter">An optional predicate to determine whether a given exception should be retried. If <c>null</c>, all exceptions are retried.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is negative.</exception>
        public static async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, TimeSpan? delay = null, bool exponentialBackoff = true, Func<Exception, bool>? exceptionFilter = null)
        {
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentOutOfRangeException.ThrowIfNegative(maxRetries);

            TimeSpan currentDelay = delay ?? TimeSpan.FromSeconds(1);

            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    return await operation().ConfigureAwait(false);
                }
                catch (Exception ex) when (attempt < maxRetries && (exceptionFilter?.Invoke(ex) ?? true))
                {
                    await Task.Delay(currentDelay).ConfigureAwait(false);
                    if (exponentialBackoff)
                        currentDelay *= 2;
                }
            }
        }

        /// <summary>
        /// Retries the specified asynchronous operation up to the specified number of times with a delay between attempts.
        /// Uses exponential backoff by default (delay doubles after each retry).
        /// </summary>
        /// <param name="operation">The asynchronous operation to retry.</param>
        /// <param name="maxRetries">The maximum number of retry attempts. Defaults to 3.</param>
        /// <param name="delay">The initial delay between retries. Defaults to 1 second.</param>
        /// <param name="exponentialBackoff">Whether to double the delay after each retry. Defaults to <c>true</c>.</param>
        /// <param name="exceptionFilter">An optional predicate to determine whether a given exception should be retried. If <c>null</c>, all exceptions are retried.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is negative.</exception>
        public static async Task RetryAsync(Func<Task> operation, int maxRetries = 3, TimeSpan? delay = null, bool exponentialBackoff = true, Func<Exception, bool>? exceptionFilter = null)
        {
            await RetryAsync<object?>(async () =>
            {
                await operation().ConfigureAwait(false);
                return null;
            }, maxRetries, delay, exponentialBackoff, exceptionFilter).ConfigureAwait(false);
        }

        extension<T>(Task<T> task)
        {
            /// <summary>
            /// Applies a timeout to the specified task. If the task does not complete within the
            /// specified duration, a <see cref="TimeoutException"/> is thrown.
            /// </summary>
            /// <param name="timeout">The maximum duration to wait for the task.</param>
            /// <param name="cancellationToken">An optional cancellation token.</param>
            /// <returns>The result of the task if it completes within the timeout.</returns>
            /// <exception cref="TimeoutException">Thrown if the task does not complete within the specified timeout.</exception>
            public async Task<T> WithTimeoutAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(task);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var delayTask = Task.Delay(timeout, cts.Token);

                Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
                if (completedTask == delayTask)
                {
                    throw new TimeoutException($"The operation did not complete within {timeout}.");
                }

                await cts.CancelAsync().ConfigureAwait(false);
                return await task.ConfigureAwait(false);
            }

            /// <summary>
            /// Attaches a continuation that executes the specified action if the task faults.
            /// The original exception is re-thrown after the handler executes.
            /// </summary>
            /// <param name="onFailure">The action to execute when the task faults, receiving the exception.</param>
            /// <returns>The result of the task if it completes successfully.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is <c>null</c>.</exception>
            public async Task<T> OnFailureAsync(Action<Exception> onFailure)
            {
                ArgumentNullException.ThrowIfNull(task);
                ArgumentNullException.ThrowIfNull(onFailure);

                try
                {
                    return await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    onFailure(ex);
                    throw;
                }
            }

            /// <summary>
            /// Awaits the task while observing the specified cancellation token. If the token
            /// is signalled before the task completes, <see cref="OperationCanceledException"/> is thrown.
            /// The underlying task continues running; its exceptions are not observed here.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>The result of the task.</returns>
            public async Task<T> WithCancellationAsync(CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(task);

                if (!cancellationToken.CanBeCanceled)
                    return await task.ConfigureAwait(false);

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                using (cancellationToken.Register(static s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
                {
                    Task completed = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                    if (completed == tcs.Task)
                        throw new OperationCanceledException(cancellationToken);
                }

                return await task.ConfigureAwait(false);
            }
        }

        extension(Task task)
        {
            /// <summary>
            /// Applies a timeout to the specified task. If the task does not complete within the
            /// specified duration, a <see cref="TimeoutException"/> is thrown.
            /// </summary>
            /// <param name="timeout">The maximum duration to wait for the task.</param>
            /// <param name="cancellationToken">An optional cancellation token.</param>
            /// <exception cref="TimeoutException">Thrown if the task does not complete within the specified timeout.</exception>
            public async Task WithTimeoutAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(task);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var delayTask = Task.Delay(timeout, cts.Token);

                Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
                if (completedTask == delayTask)
                {
                    throw new TimeoutException($"The operation did not complete within {timeout}.");
                }

                await cts.CancelAsync().ConfigureAwait(false);
                await task.ConfigureAwait(false);
            }

            /// <summary>
            /// Attaches a continuation that executes the specified action if the task faults.
            /// The original exception is re-thrown after the handler executes.
            /// </summary>
            /// <param name="onFailure">The action to execute when the task faults, receiving the exception.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is <c>null</c>.</exception>
            public async Task OnFailureAsync(Action<Exception> onFailure)
            {
                ArgumentNullException.ThrowIfNull(task);
                ArgumentNullException.ThrowIfNull(onFailure);

                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    onFailure(ex);
                    throw;
                }
            }

            /// <summary>
            /// Executes the task without awaiting it, suppressing unobserved exceptions.
            /// Optionally invokes a callback when an exception occurs.
            /// </summary>
            /// <param name="onError">An optional callback to invoke if the task faults.</param>
            public void FireAndForget(Action<Exception>? onError = null)
            {
                ArgumentNullException.ThrowIfNull(task);

                _ = task.ContinueWith(
                    t =>
                    {
                        if (t.Exception?.InnerException is { } ex)
                            onError?.Invoke(ex);
                    },
                    TaskContinuationOptions.OnlyOnFaulted);
            }

            /// <summary>
            /// Awaits the task while observing the specified cancellation token. If the token
            /// is signalled before the task completes, <see cref="OperationCanceledException"/> is thrown.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            public async Task WithCancellationAsync(CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(task);

                if (!cancellationToken.CanBeCanceled)
                {
                    await task.ConfigureAwait(false);
                    return;
                }

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                using (cancellationToken.Register(static s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
                {
                    Task completed = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                    if (completed == tcs.Task)
                        throw new OperationCanceledException(cancellationToken);
                }
                await task.ConfigureAwait(false);
            }
        }
    }
}
