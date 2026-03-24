using BenchmarkDotNet.Attributes;
using Grondo.Extensions;

namespace Grondo.Benchmarks
{
    [MemoryDiagnoser]
    public class ListExBenchmarks
    {
        private List<string> _list = null!;
        private string[] _newItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            _list = [.. Enumerable.Range(1, 1_000).Select(i => $"item{i}")];
            _newItems = [.. Enumerable.Range(500, 1_000).Select(i => $"item{i}")];
        }

        [Benchmark]
        public List<string> AddRangeNoDuplicates()
        {
            var copy = new List<string>(_list);
            return copy.AddRangeNoDuplicates(_newItems);
        }

        [Benchmark]
        public List<string> AddIfNotExists()
        {
            var copy = new List<string>(_list);
            copy.AddIfNotExists("newitem");
            return copy;
        }
    }
}

