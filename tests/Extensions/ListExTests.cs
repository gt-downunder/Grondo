using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ListExTests : BaseExtensionTest
    {
        [TestMethod]
        public void Add_EmptyStringListWithValidItems_Success()
        {
            // Arrange
            string[] itemsToAdd = ["1", "2", "3"];
            var list = new List<string>();

            // Act
            list.Add(itemsToAdd);

            // Assert
            list.Count.Should().Be(itemsToAdd.Length);
            itemsToAdd.ForEach((item, _) => list.Should().Contain(item));
        }

        [TestMethod]
        public void Add_NonEmptyStringListWithValidItems_Success()
        {
            // Arrange
            string[] itemsToAdd = ["1", "2", "3"];
            string[] existingItems = ["Hello", "World", "How are", "you today?"];
            var list = new List<string>();
            list.Add(existingItems);

            // Act
            list.Add(itemsToAdd);

            // Assert
            list.Count.Should().Be(itemsToAdd.Length + existingItems.Length);
            itemsToAdd.ForEach((item, _) => list.Should().Contain(item));
            existingItems.ForEach((item, _) => list.Should().Contain(item));
        }

        [TestMethod]
        public void Add_EmptyIntListWithValidItems_Success()
        {
            // Arrange
            int[] itemsToAdd = [1, 2, 3];
            var list = new List<int>();

            // Act
            list.Add(itemsToAdd);

            // Assert
            list.Count.Should().Be(itemsToAdd.Length);
            itemsToAdd.ForEach((item, _) => list.Should().Contain(item));
        }

        [TestMethod]
        public void Add_NonEmptyIntListWithValidItems_Success()
        {
            // Arrange
            int[] itemsToAdd = [1, 2, 3];
            int[] existingItems = [100, 200, 300, 400];
            var list = new List<int>();
            list.Add(existingItems);

            // Act
            list.Add(itemsToAdd);

            // Assert
            list.Count.Should().Be(itemsToAdd.Length + existingItems.Length);
            itemsToAdd.ForEach((item, _) => list.Should().Contain(item));
            existingItems.ForEach((item, _) => list.Should().Contain(item));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void AddIfNotNull_Boolean_Added(bool testValue)
        {
            // Arrange
            var list = new List<bool>();

            // Act
            list.AddIfNotNull(testValue);

            // Assert
            list.Should().Contain(testValue);
        }

        [TestMethod]
        [DataRow("Hello World")]
        [DataRow("The quick brown fox jumps over the lazy dog")]
        public void AddIfNotNull_String_Added(string testValue)
        {
            // Arrange
            var list = new List<string>();

            // Act
            list.AddIfNotNull(testValue);

            // Assert
            list.Should().Contain(testValue);
        }

        [TestMethod]
        [DataRow(42)]
        [DataRow(123)]
        public void AddIfNotNull_Int_Added(int testValue)
        {
            // Arrange
            var list = new List<int>();

            // Act
            list.AddIfNotNull(testValue);

            // Assert
            list.Should().Contain(testValue);
        }

        [TestMethod]
        public void AddIfNotNull_NullString_NotAdded()
        {
            // Arrange
            var list = new List<string?>();
            string? testValue = null;

            // Act
            list.AddIfNotNull(testValue);

            // Assert
            list.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("hello  world")]
        [DataRow("The quick brown fox jumps over the lazy dog")]
        [DataRow("")]
        public void AddIfNotExists_ListDoesNotContainNewValue_NewValueAdded(string existingValue)
        {
            // Arrange
            var list = new List<string> { existingValue };
            const string NewValue = "hello world";

            // Act
            list.AddIfNotExists(NewValue);

            // Assert
            list.Count.Should().Be(2);
            list.Should().Contain(existingValue);
            list.Should().Contain(NewValue);
        }

        [TestMethod]
        [DataRow("Hello world")]
        [DataRow("hello world")]
        [DataRow("HeLlO WoRlD")]
        public void AddIfNotExists_ListContainsValue_ListUnchanged(string testValue)
        {
            // Arrange
            var list = new List<string> { "HELLO WORLD" };

            // Act
            list.AddIfNotExists(testValue);

            // Assert
            list.Count.Should().Be(1);
            list.Should().Contain(list.Single());
        }

        [TestMethod]
        public void AddRangeNoDuplicates_AddSomeDuplicates_ResultIsDistinct()
        {
            // Arrange
            var newValues = new List<string>
            {
                "Hello",
                "How are",
                "you doing"
            };

            string[] existingValues = ["hello", "world", "how are", "you today"];
            var list = new List<string>();
            list.Add(existingValues);

            // Act
            list.AddRangeNoDuplicates(newValues);

            // Assert
            list.Count.Should().Be(5);
            existingValues.ForEach((value, _) => list.Should().Contain(value));
            list.Should().Contain("you doing");
        }

        [TestMethod]
        public void AddRangeNoDuplicates_AddNoDuplicates_AllValuesAdded()
        {
            // Arrange
            var newValues = new List<string>
            {
                "Hello,",
                "How are ",
                "you doing?"
            };

            string[] existingValues = ["hello", "world", "how are", "you today"];
            var list = new List<string>();
            list.Add(existingValues);

            // Act
            list.AddRangeNoDuplicates(newValues);

            // Assert
            list.Count.Should().Be(7);
            existingValues.ForEach((value, _) => list.Should().Contain(value));
            newValues.ForEach((value, _) => list.Should().Contain(value));
        }

        [TestMethod]
        public void ContainsIgnoreCase_DoesContainMatchingCase_ReturnsTrue()
        {
            new List<string> { { "Hello world" } }.ContainsIgnoreCase("Hello world").Should().BeTrue();
        }

        [TestMethod]
        [DataRow("HELLO WORLD")]
        [DataRow("hello world")]
        [DataRow("HeLlO WoRlD")]
        public void ContainsIgnoreCase_DoesContainDifferentCase_ReturnsTrue(string testValue)
        {
            // Arrange & Assert
            new List<string> { { "Hello world" } }.ContainsIgnoreCase(testValue.ToLower()).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(" HELLO WORLD")]
        [DataRow("hello  world")]
        [DataRow("HeLlO WoRlD ")]
        public void ContainsIgnoreCase_WhitespaceDifferenceOnly_ReturnsFalse(string testValue)
        {
            // Arrange & Assert
            new List<string> { { "Hello world" } }.ContainsIgnoreCase(testValue.ToLower()).Should().BeFalse();
        }

        [TestMethod]
        [DataRow("HELLO WORLD")]
        [DataRow("hello world")]
        [DataRow("HeLlO WoRlD")]
        public void ContainsIgnoreCase_ManyValuesInListWithDifferentCaseMatch_ReturnsTrue(string testValue)
        {
            // Arrange
            var list = new List<string> { { "Hello world" } };

            for (int i = 0; i < 100; i++)
            {
                list.Add(RandomFactory.GetAlphanumericString(100));
            }

            // Assert
            list.ContainsIgnoreCase(testValue.ToLower()).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("Hello world")]
        public void ContainsIgnoreCase_ManyValuesInListWithExactCaseMatch_ReturnsTrue(string testValue)
        {
            // Arrange
            var list = new List<string> { { "Hello world" } };

            for (int i = 0; i < 100; i++)
            {
                list.Add(RandomFactory.GetAlphanumericString(100));
            }

            // Assert
            list.ContainsIgnoreCase(testValue.ToLower()).Should().BeTrue();
        }
    }
}
