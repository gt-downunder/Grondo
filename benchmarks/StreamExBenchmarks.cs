using BenchmarkDotNet.Attributes;
using Grondo.Extensions;

namespace Grondo.Benchmarks
{
    [MemoryDiagnoser]
    public class StreamExBenchmarks
    {
        private byte[] _data = null!;

        [GlobalSetup]
        public void Setup()
        {
            _data = new byte[10_000];
            Random.Shared.NextBytes(_data);
        }

        [Benchmark]
        public async Task<byte[]> ToByteArrayAsync()
        {
            using var stream = new MemoryStream(_data);
            return await stream.ToByteArrayAsync();
        }

        [Benchmark]
        public async Task<string> ToStringAsync()
        {
            using var stream = new MemoryStream(_data);
            return await stream.ToStringAsync();
        }

        [Benchmark]
        public async Task<MemoryStream> ToMemoryStreamAsync()
        {
            using var stream = new MemoryStream(_data);
            MemoryStream result = await stream.ToMemoryStreamAsync();
            await result.DisposeAsync();
            return result;
        }
    }
}

