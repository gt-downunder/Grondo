using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class DictionaryExTests : BaseExtensionTest
    {
        [TestMethod]
        public void HasAnyKey_ShouldReturnTrue_WhenKeyIsInDictionary()
        {
            new Dictionary<string, object>() { { "one", 1 } }.HasAnyKey(["one"]).Should().BeTrue();
            new Dictionary<string, object>() { { "one", 1 } }.HasAnyKey(["one", "two"]).Should().BeTrue();

            const int Size = 10000;
            Dictionary<string, object> hugeDictionary = GetRandomStringObjectDictionary(Size);

            string[] searchArray = new string[10];
            for (int i = 0; i < 10; i++)
            {
                searchArray[i] = hugeDictionary.ElementAt(RandomFactory.GetInteger(0, Size)).Key;
            }

            hugeDictionary.HasAnyKey(searchArray).Should().BeTrue();
            hugeDictionary.HasAnyKey([Guid.NewGuid().ToString()]).Should().BeFalse();
        }

        [TestMethod]
        public void HasAnyKey_ShouldReturnFalse_WhenKeyIsNotInDictionary()
        {
            new Dictionary<string, object> { { "one", 1 } }.HasAnyKey(["two", "three", "four"]).Should().BeFalse();
        }

        [TestMethod]
        public void HasAnyKey_ShouldReturnFalse_WhenOneKeyIsNull()
        {
            GetRandomStringObjectDictionary(10).HasAnyKey([null!]).Should().BeFalse();
        }

        [TestMethod]
        public void HasAnyKey_ShouldReturnFalse_WhenKeyListIsNull()
        {
            GetRandomStringObjectDictionary(10).HasAnyKey(null).Should().BeFalse();
        }

        [TestMethod]
        public void TryGetValue_ShouldReturnValue_WhenKeyIsInDictionary()
        {
            new Dictionary<string, object>() { { "one", 1 } }.TryGetValue("one", out object? tinyValue).Should().BeTrue();
            tinyValue.Should().Be(1);

            const int Size = 10000;
            Dictionary<string, object> hugeDictionary = GetRandomStringObjectDictionary(Size);

            for (int i = 0; i < 10; i++)
            {
                KeyValuePair<string, object> entry = hugeDictionary.ElementAt(RandomFactory.GetInteger(0, Size));

                hugeDictionary.TryGetValue(entry.Key, out object? actual).Should().BeTrue();
                actual.Should().Be(entry.Value);
            }
        }

        [TestMethod]
        public void TryGetValue_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            FluentActions.Invoking(() => GetRandomStringObjectDictionary(10).TryGetValue(null!, out _))
                .Should().Throw<ArgumentNullException>();
        }

        private static Dictionary<string, object> GetRandomStringObjectDictionary(int size)
        {
            var dict = new Dictionary<string, object>();

            for (int i = 0; dict.Count < size; i++)
            {
                dict.TryAdd(RandomFactory.GetAlphanumericString(50), i);
            }

            return dict;
        }

        // --- Merge ---

        [TestMethod]
        public void Merge_Overwrite_MergesAndOverwrites()
        {
            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var other = new Dictionary<string, int> { { "b", 99 }, { "c", 3 } };

            dict.Merge(other);

            dict["a"].Should().Be(1);
            dict["b"].Should().Be(99);
            dict["c"].Should().Be(3);
        }

        [TestMethod]
        public void Merge_NoOverwrite_PreservesExisting()
        {
            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var other = new Dictionary<string, int> { { "b", 99 }, { "c", 3 } };

            dict.Merge(other, overwrite: false);

            dict["b"].Should().Be(2); // preserved
            dict["c"].Should().Be(3); // added
        }

        [TestMethod]
        public void Merge_EmptyOther_NoChange()
        {
            var dict = new Dictionary<string, int> { { "a", 1 } };
            dict.Merge(new Dictionary<string, int>());
            dict.Should().HaveCount(1);
        }

        // --- GetOrAdd ---

        [TestMethod]
        public void GetOrAdd_KeyExists_ReturnsExisting()
        {
            new Dictionary<string, int> { { "a", 42 } }.GetOrAdd("a", () => 999).Should().Be(42);
        }

        [TestMethod]
        public void GetOrAdd_KeyNotExists_CreatesAndAdds()
        {
            var dict = new Dictionary<string, int>();
            dict.GetOrAdd("a", () => 42).Should().Be(42);
            dict["a"].Should().Be(42);
        }

        [TestMethod]
        public void GetOrAdd_FactoryCalledOnlyOnce()
        {
            var dict = new Dictionary<string, int>();
            int callCount = 0;

            dict.GetOrAdd("a", () => { callCount++; return 42; });
            dict.GetOrAdd("a", () => { callCount++; return 99; });

            callCount.Should().Be(1);
        }

        [TestMethod]
        public void AddOrUpdate_NewKey_Adds()
        {
            var dict = new Dictionary<string, int>();
            int result = dict.AddOrUpdate("a", 1, (_, existing) => existing + 10);
            result.Should().Be(1);
            dict["a"].Should().Be(1);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingKey_Updates()
        {
            var dict = new Dictionary<string, int> { ["a"] = 5 };
            int result = dict.AddOrUpdate("a", 1, (_, existing) => existing + 10);
            result.Should().Be(15);
            dict["a"].Should().Be(15);
        }

        [TestMethod]
        public void RemoveWhere_RemovesMatchingEntries_ReturnsCount()
        {
            var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
            int removed = dict.RemoveWhere(kv => kv.Value > 1);
            removed.Should().Be(2);
            dict.Should().ContainKey("a").And.NotContainKey("b").And.NotContainKey("c");
        }
    }
}
