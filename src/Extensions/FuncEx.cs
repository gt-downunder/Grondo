using System.Collections.Concurrent;

namespace Grondo.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Func{T}"/> and <see cref="Action"/> delegates.
    /// </summary>
    public static class FuncEx
    {
        extension<T, TResult>(Func<T, TResult> func) where T : notnull
        {
            /// <summary>
            /// Memoizes a function, caching results by input.
            /// Thread-safe implementation using ConcurrentDictionary.
            /// </summary>
            /// <returns>A memoized version of the function.</returns>
            public Func<T, TResult> Memoize()
            {
                ArgumentNullException.ThrowIfNull(func);

                var cache = new ConcurrentDictionary<T, TResult>();
                return arg => cache.GetOrAdd(arg, func);
            }
        }

        extension(Action action)
        {
            /// <summary>
            /// Debounces an action, ensuring it's only called after a delay with no new calls.
            /// Useful for scenarios like search-as-you-type where you want to wait for the user to stop typing.
            /// </summary>
            /// <param name="delay">The delay to wait after the last call.</param>
            /// <returns>A debounced version of the action.</returns>
            public Action Debounce(TimeSpan delay)
            {
                ArgumentNullException.ThrowIfNull(action);

                CancellationTokenSource? cts = null;
                var lockObj = new object();

                return () =>
                {
                    lock (lockObj)
                    {
                        cts?.Cancel();
                        cts?.Dispose();
                        cts = new CancellationTokenSource();

                        Task.Delay(delay, cts.Token)
                            .ContinueWith(t =>
                            {
                                if (!t.IsCanceled)
                                    action();
                            }, TaskScheduler.Default);
                    }
                };
            }

            /// <summary>
            /// Throttles an action, ensuring it's called at most once per interval.
            /// Useful for rate-limiting scenarios like API calls or UI updates.
            /// </summary>
            /// <param name="interval">The minimum interval between calls.</param>
            /// <returns>A throttled version of the action.</returns>
            public Action Throttle(TimeSpan interval)
            {
                ArgumentNullException.ThrowIfNull(action);

                var lastRun = DateTime.MinValue;
                var lockObj = new object();

                return () =>
                {
                    lock (lockObj)
                    {
                        var now = DateTime.UtcNow;
                        if (now - lastRun >= interval)
                        {
                            action();
                            lastRun = now;
                        }
                    }
                };
            }
        }

        extension<T>(Func<T> factory)
        {
            /// <summary>
            /// Converts a function to a lazy initializer.
            /// The function will only be called once, on first access.
            /// </summary>
            /// <returns>A Lazy instance wrapping the factory function.</returns>
            public Lazy<T> ToLazy()
            {
                ArgumentNullException.ThrowIfNull(factory);
                return new Lazy<T>(factory);
            }
        }

        extension<T>(Action<T> action)
        {
            /// <summary>
            /// Debounces an action with a parameter.
            /// </summary>
            /// <param name="delay">The delay to wait after the last call.</param>
            /// <returns>A debounced version of the action.</returns>
            public Action<T> Debounce(TimeSpan delay)
            {
                ArgumentNullException.ThrowIfNull(action);

                CancellationTokenSource? cts = null;
                var lockObj = new object();
                T? lastArg = default;

                return arg =>
                {
                    lock (lockObj)
                    {
                        lastArg = arg;
                        cts?.Cancel();
                        cts?.Dispose();
                        cts = new CancellationTokenSource();

                        Task.Delay(delay, cts.Token)
                            .ContinueWith(t =>
                            {
                                if (!t.IsCanceled && lastArg != null)
                                    action(lastArg);
                            }, TaskScheduler.Default);
                    }
                };
            }

            /// <summary>
            /// Throttles an action with a parameter.
            /// </summary>
            /// <param name="interval">The minimum interval between calls.</param>
            /// <returns>A throttled version of the action.</returns>
            public Action<T> Throttle(TimeSpan interval)
            {
                ArgumentNullException.ThrowIfNull(action);

                var lastRun = DateTime.MinValue;
                var lockObj = new object();

                return arg =>
                {
                    lock (lockObj)
                    {
                        var now = DateTime.UtcNow;
                        if (now - lastRun >= interval)
                        {
                            action(arg);
                            lastRun = now;
                        }
                    }
                };
            }
        }
    }
}

