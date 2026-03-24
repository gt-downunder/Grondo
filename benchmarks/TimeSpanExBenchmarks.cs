using BenchmarkDotNet.Attributes;
using Grondo.Extensions;

namespace Grondo.Benchmarks
{
    [MemoryDiagnoser]
    public class TimeSpanExBenchmarks
    {
        private TimeSpan _complex;
        private TimeSpan _simple;

        [GlobalSetup]
        public void Setup()
        {
            _complex = new TimeSpan(400, 5, 30, 45);
            _simple = TimeSpan.FromMinutes(90);
        }

        [Benchmark]
        public string ToHumanReadable_Complex() => _complex.ToHumanReadable();

        [Benchmark]
        public string ToHumanReadable_Simple() => _simple.ToHumanReadable();

        [Benchmark]
        public string ToHumanReadable_MaxParts() => _complex.ToHumanReadable(maxParts: 2);
    }
}

