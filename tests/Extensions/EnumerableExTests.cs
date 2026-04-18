using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class EnumerableExTests : BaseExtensionTest
    {
        [TestMethod]
        public void IsNullOrEmpty_Null_ReturnsTrue() =>
            default(IEnumerable<int>).IsNullOrEmpty().Should().BeTrue();

        [TestMethod]
        public void IsNullOrEmpty_Empty_ReturnsTrue() =>
            Array.Empty<int>().IsNullOrEmpty().Should().BeTrue();

        [TestMethod]
        public void IsNullOrEmpty_HasItems_ReturnsFalse() =>
            new[] { 1 }.IsNullOrEmpty().Should().BeFalse();

        [TestMethod]
        public void IsNotNullOrEmpty_Null_ReturnsFalse() =>
            default(IEnumerable<int>).IsNotNullOrEmpty().Should().BeFalse();

        [TestMethod]
        public void IsNotNullOrEmpty_HasItems_ReturnsTrue() =>
            new[] { 1, 2 }.IsNotNullOrEmpty().Should().BeTrue();

        [TestMethod]
        public void IsEmpty_EmptyCollection_ReturnsTrue() =>
            Array.Empty<string>().IsEmpty().Should().BeTrue();

        [TestMethod]
        public void IsEmpty_NonEmpty_ReturnsFalse() =>
            new[] { "a" }.IsEmpty().Should().BeFalse();

        [TestMethod]
        public void IsEmpty_Null_ThrowsArgumentNullException()
        {
            IEnumerable<int> source = null!;
            Func<bool> act = () => source.IsEmpty();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void IsNotEmpty_NonEmpty_ReturnsTrue() =>
            new[] { 1 }.IsNotEmpty().Should().BeTrue();

        [TestMethod]
        public void IsDeepEqualTo_EqualSequences_ReturnsTrue() =>
            new[] { 1, 2, 3 }.IsDeepEqualTo([1, 2, 3]).Should().BeTrue();

        [TestMethod]
        public void IsDeepEqualTo_DifferentSequences_ReturnsFalse() =>
            new[] { 1, 2, 3 }.IsDeepEqualTo([3, 2, 1]).Should().BeFalse();

        [TestMethod]
        public void ForEach_Action_ExecutesForEachWithIndex()
        {
            string[] items = ["a", "b", "c"];
            var collected = new List<(string item, int index)>();

            items.ForEach((item, index) => collected.Add((item, index)));

            collected.Should().HaveCount(3);
            collected[0].Should().Be(("a", 0));
            collected[2].Should().Be(("c", 2));
        }

        [TestMethod]
        public void ForEach_Func_ReturnsFalseOnEarlyBreak()
        {
            int[] items = [1, 2, 3, 4, 5];
            var processed = new List<int>();

            bool result = items.ForEach((item, _) =>
            {
                processed.Add(item);
                return item < 3;
            });

            result.Should().BeFalse();
            processed.Should().BeEquivalentTo([1, 2, 3]);
        }

        [TestMethod]
        public void ForEach_Func_ReturnsTrueWhenAllProcessed()
        {
            bool result = new[] { 1, 2, 3 }.ForEach((_, _) => true);
            result.Should().BeTrue();
        }

        // --- Batch ---

        [TestMethod]
        public void Batch_EvenSplit_ReturnsEqualBatches()
        {
            int[] items = [1, 2, 3, 4, 5, 6];
            var batches = items.Batch(3).ToList();

            batches.Should().HaveCount(2);
            batches[0].Should().BeEquivalentTo([1, 2, 3]);
            batches[1].Should().BeEquivalentTo([4, 5, 6]);
        }

        [TestMethod]
        public void Batch_UnevenSplit_LastBatchSmaller()
        {
            int[] items = [1, 2, 3, 4, 5];
            var batches = items.Batch(3).ToList();

            batches.Should().HaveCount(2);
            batches[0].Should().HaveCount(3);
            batches[1].Should().HaveCount(2);
        }

        [TestMethod]
        public void Batch_EmptySource_ReturnsEmpty() => Array.Empty<int>().Batch(5).Should().BeEmpty();

        [TestMethod]
        public void Batch_SizeZero_Throws()
        {
            Action act = () => _ = new[] { 1 }.Batch(0).ToList();
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Partition ---

        [TestMethod]
        public void Partition_SplitsCorrectly()
        {
            int[] items = [1, 2, 3, 4, 5, 6];
            (IReadOnlyList<int>? matches, IReadOnlyList<int>? nonMatches) = items.Partition(x => x % 2 == 0);

            matches.Should().BeEquivalentTo([2, 4, 6]);
            nonMatches.Should().BeEquivalentTo([1, 3, 5]);
        }

        [TestMethod]
        public void Partition_AllMatch_NonMatchesEmpty()
        {
            (IReadOnlyList<int>? matches, IReadOnlyList<int>? nonMatches) = new[] { 2, 4, 6 }.Partition(x => x % 2 == 0);
            matches.Should().HaveCount(3);
            nonMatches.Should().BeEmpty();
        }

        // --- WhereNotNull ---

        [TestMethod]
        public void WhereNotNull_FiltersNulls()
        {
            string?[] items = ["a", null, "b", null, "c"];
            var result = items.WhereNotNull().ToList();

            result.Should().BeEquivalentTo(["a", "b", "c"]);
        }

        [TestMethod]
        public void WhereNotNull_AllNull_ReturnsEmpty()
        {
            string?[] items = [null, null];
            items.WhereNotNull().Should().BeEmpty();
        }

        [TestMethod]
        public void WhereNotNull_NoNulls_ReturnsAll()
        {
            string?[] items = ["a", "b"];
            items.WhereNotNull().Should().HaveCount(2);
        }

        // --- Interleave ---

        [TestMethod]
        public void Interleave_EqualLength_AlternatesElements()
        {
            int[] first = [1, 3, 5];
            int[] second = [2, 4, 6];

            var result = first.Interleave(second).ToList();

            result.Should().BeEquivalentTo([1, 2, 3, 4, 5, 6], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void Interleave_FirstLonger_AppendsRemaining()
        {
            int[] first = [1, 3, 5, 7];
            int[] second = [2, 4];

            var result = first.Interleave(second).ToList();

            result.Should().BeEquivalentTo([1, 2, 3, 4, 5, 7], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void Interleave_SecondLonger_AppendsRemaining()
        {
            int[] first = [1];
            int[] second = [2, 4, 6];

            var result = first.Interleave(second).ToList();

            result.Should().BeEquivalentTo([1, 2, 4, 6], options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void Interleave_EmptyFirst_ReturnsSecond()
        {
            int[] first = [];
            int[] second = [1, 2, 3];

            first.Interleave(second).Should().BeEquivalentTo([1, 2, 3]);
        }

        [TestMethod]
        public void Interleave_Null_ThrowsArgumentNullException()
        {
            IEnumerable<int> source = null!;
            Func<IEnumerable<int>> act = () => source.Interleave([1]).ToList();
            act.Should().Throw<ArgumentNullException>();
        }

        // --- Scan (seeded) ---

        [TestMethod]
        public void Scan_Seeded_ReturnsRunningTotals()
        {
            int[] items = [1, 2, 3, 4];
            var result = items.Scan(0, (acc, x) => acc + x).ToList();
            result.Should().BeEquivalentTo([1, 3, 6, 10], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void Scan_Seeded_EmptySource_ReturnsEmpty() =>
            Array.Empty<int>().Scan(0, (acc, x) => acc + x).Should().BeEmpty();

        [TestMethod]
        public void Scan_Seeded_NullSource_Throws()
        {
            IEnumerable<int> source = null!;
            Func<IEnumerable<int>> act = () => source.Scan(0, (acc, x) => acc + x).ToList();
            act.Should().Throw<ArgumentNullException>();
        }

        // --- Scan (seedless) ---

        [TestMethod]
        public void Scan_Seedless_ReturnsRunningResults()
        {
            int[] items = [1, 2, 3, 4];
            var result = items.Scan((a, b) => a + b).ToList();
            result.Should().BeEquivalentTo([1, 3, 6, 10], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void Scan_Seedless_SingleElement_ReturnsSingleElement()
        {
            var result = new[] { 42 }.Scan((a, b) => a + b).ToList();
            result.Should().BeEquivalentTo([42]);
        }

        [TestMethod]
        public void Scan_Seedless_EmptySource_ThrowsInvalidOperation()
        {
            Action act = () => { _ = Array.Empty<int>().Scan((a, b) => a + b).ToList(); };
            act.Should().Throw<InvalidOperationException>();
        }

        // --- Window ---

        [TestMethod]
        public void Window_ReturnsOverlappingWindows()
        {
            int[] items = [1, 2, 3, 4, 5];
            var result = items.Window(3).ToList();
            result.Should().HaveCount(3);
            result[0].Should().BeEquivalentTo([1, 2, 3], o => o.WithStrictOrdering());
            result[1].Should().BeEquivalentTo([2, 3, 4], o => o.WithStrictOrdering());
            result[2].Should().BeEquivalentTo([3, 4, 5], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void Window_SizeLargerThanSource_ReturnsEmpty()
        {
            int[] items = [1, 2];
            items.Window(5).Should().BeEmpty();
        }

        [TestMethod]
        public void Window_SizeOne_ReturnsSingleElementWindows()
        {
            int[] items = [1, 2, 3];
            var result = items.Window(1).ToList();
            result.Should().HaveCount(3);
            result[0].Should().BeEquivalentTo([1]);
            result[2].Should().BeEquivalentTo([3]);
        }

        [TestMethod]
        public void Window_SizeZero_Throws()
        {
            Action act = () => { _ = new[] { 1 }.Window(0).ToList(); };
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Shuffle ---

        [TestMethod]
        public void Shuffle_ReturnsSameElements()
        {
            int[] items = [1, 2, 3, 4, 5];
            IEnumerable<int> result = items.Shuffle();
            result.Should().BeEquivalentTo([1, 2, 3, 4, 5]);
        }

        [TestMethod]
        public void Shuffle_WithSeed_IsDeterministic()
        {
            int[] items = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            IReadOnlyList<int> r1 = items.Shuffle(new Random(42));
            IReadOnlyList<int> r2 = items.Shuffle(new Random(42));
            r1.Should().BeEquivalentTo(r2, o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void Shuffle_EmptySource_ReturnsEmpty() =>
            Array.Empty<int>().Shuffle().Should().BeEmpty();

        // --- FallbackIfEmpty ---

        [TestMethod]
        public void FallbackIfEmpty_NonEmpty_ReturnsSource()
        {
            int[] items = [1, 2, 3];
            items.FallbackIfEmpty([99]).Should().BeEquivalentTo([1, 2, 3], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void FallbackIfEmpty_Empty_ReturnsFallback()
        {
            Array.Empty<int>().FallbackIfEmpty([10, 20]).Should().BeEquivalentTo([10, 20], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void FallbackIfEmpty_Sequence_NonEmpty_ReturnsSource()
        {
            int[] items = [1, 2];
            items.FallbackIfEmpty(new[] { 99 }.AsEnumerable()).Should().BeEquivalentTo([1, 2]);
        }

        [TestMethod]
        public void FallbackIfEmpty_Sequence_Empty_ReturnsFallback()
        {
            Array.Empty<int>().FallbackIfEmpty(new[] { 5, 6 }.AsEnumerable())
                .Should().BeEquivalentTo([5, 6], o => o.WithStrictOrdering());
        }

        // --- Pairwise ---

        [TestMethod]
        public void Pairwise_ReturnsConsecutivePairs()
        {
            int[] items = [1, 2, 3, 4];
            var result = items.Pairwise((a, b) => b - a).ToList();
            result.Should().BeEquivalentTo([1, 1, 1], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void Pairwise_SingleElement_ReturnsEmpty() =>
            new[] { 1 }.Pairwise((a, b) => a + b).Should().BeEmpty();

        [TestMethod]
        public void Pairwise_Empty_ReturnsEmpty() =>
            Array.Empty<int>().Pairwise((a, b) => a + b).Should().BeEmpty();

        // --- TagFirstLast ---

        [TestMethod]
        public void TagFirstLast_MultipleElements_TagsCorrectly()
        {
            string[] items = ["a", "b", "c"];
            var result = items.TagFirstLast((item, first, last) => $"{item}:{first}:{last}").ToList();
            result.Should().BeEquivalentTo(["a:True:False", "b:False:False", "c:False:True"], o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void TagFirstLast_SingleElement_IsBothFirstAndLast()
        {
            var result = new[] { "only" }.TagFirstLast((item, first, last) => $"{item}:{first}:{last}").ToList();
            result.Should().BeEquivalentTo(["only:True:True"]);
        }

        [TestMethod]
        public void TagFirstLast_Empty_ReturnsEmpty() =>
            Array.Empty<string>().TagFirstLast((item, first, last) => item).Should().BeEmpty();
    }
}
