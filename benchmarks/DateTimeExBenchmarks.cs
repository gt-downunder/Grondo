using BenchmarkDotNet.Attributes;
using Grondo.Extensions;

namespace Grondo.Benchmarks
{
    [MemoryDiagnoser]
    public class DateTimeExBenchmarks
    {
        private DateTime _dateTime;
        private DateTime? _nullableDateTime;
        private string _formattedDate = null!;
        private string _formattedDateTime = null!;

        [GlobalSetup]
        public void Setup()
        {
            _dateTime = new DateTime(2024, 6, 15, 14, 30, 45);
            _nullableDateTime = _dateTime;
            _formattedDate = "2024-06-15";
            _formattedDateTime = "2024-06-15T14:30:45";
        }

        [Benchmark]
        public string ToFormattedDate() => _nullableDateTime.ToFormattedDate();

        [Benchmark]
        public string ToFormattedDateTime() => _nullableDateTime.ToFormattedDateTime();

        [Benchmark]
        public DateTime? FromFormattedDate() => _formattedDate.FromFormattedDate();

        [Benchmark]
        public DateTime? FromFormattedDateTime() => _formattedDateTime.FromFormattedDateTime();

        [Benchmark]
        public DateTime StartOfDay() => _dateTime.StartOfDay();

        [Benchmark]
        public DateTime EndOfDay() => _dateTime.EndOfDay();

        [Benchmark]
        public DateTime EndOfMonth() => _dateTime.EndOfMonth();

        [Benchmark]
        public string ToRelativeTime() => _dateTime.ToRelativeTime();

        [Benchmark]
        public bool IsBetween() => _dateTime.IsBetween(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));
    }
}

