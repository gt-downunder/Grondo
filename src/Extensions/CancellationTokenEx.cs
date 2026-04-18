namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="CancellationToken"/>.
    /// </summary>
    public static class CancellationTokenEx
    {
        extension(CancellationToken token)
        {
            /// <summary>
            /// Returns a task that completes when the token is cancelled.
            /// The returned task is faulted with <see cref="OperationCanceledException"/> on cancellation.
            /// </summary>
            /// <returns>A task that completes when cancellation is requested.</returns>
            public Task AsTask()
            {
                if (!token.CanBeCanceled)
                    return Task.Delay(Timeout.Infinite);

                if (token.IsCancellationRequested)
                    return Task.FromCanceled(token);

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                CancellationTokenRegistration registration = token.Register(static s =>
                {
                    (TaskCompletionSource<bool>? source, CancellationToken ct) = ((TaskCompletionSource<bool>, CancellationToken))s!;
                    source.TrySetCanceled(ct);
                }, (tcs, token));

                tcs.Task.ContinueWith(
                    static (_, state) => ((CancellationTokenRegistration)state!).Dispose(),
                    registration,
                    TaskContinuationOptions.ExecuteSynchronously);

                return tcs.Task;
            }

            /// <summary>
            /// Creates a new <see cref="CancellationTokenSource"/> linked to this token, with an additional timeout.
            /// The caller owns the returned source and must dispose it.
            /// </summary>
            /// <param name="timeout">The timeout after which the linked source will cancel.</param>
            /// <returns>A new linked <see cref="CancellationTokenSource"/>.</returns>
            public CancellationTokenSource WithTimeout(TimeSpan timeout)
            {
                var linked = CancellationTokenSource.CreateLinkedTokenSource(token);
                linked.CancelAfter(timeout);
                return linked;
            }
        }
    }
}
