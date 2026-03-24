using BenchmarkDotNet.Attributes;
using Grondo.Extensions;

namespace Grondo.Benchmarks
{
    [MemoryDiagnoser]
    public class DictionaryExBenchmarks
    {
        private Dictionary<string, string> _dictionary = null!;
        private Dictionary<string, string> _otherDictionary = null!;

        [GlobalSetup]
        public void Setup()
        {
            _dictionary = Enumerable.Range(1, 100)
                .ToDictionary(i => $"key{i}", i => $"value{i}");
            _otherDictionary = Enumerable.Range(50, 100)
                .ToDictionary(i => $"key{i}", i => $"newvalue{i}");
        }

        [Benchmark]
        public bool HasAnyKey() => _dictionary.HasAnyKey(["key1", "key50", "key999"]);

        [Benchmark]
        public string ToQueryString() => _dictionary.ToQueryString();

        [Benchmark]
        public void Merge()
        {
            var copy = new Dictionary<string, string>(_dictionary);
            copy.Merge(_otherDictionary);
        }

        [Benchmark]
        public string GetOrAdd()
        {
            var dict = new Dictionary<string, string>();
            return dict.GetOrAdd("key", () => "value");
        }
    }
}

