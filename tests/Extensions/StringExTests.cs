using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class StringExTests : BaseExtensionTest
    {
        [TestMethod]
        [DataRow("david.jones@contoso.com")]
        [DataRow("d.j@server1.contoso.com")]
        [DataRow("jones@ms1.contoso.com")]
        [DataRow("j@contoso.com9")]
        [DataRow("js#internal@contoso.com")]
        [DataRow("j_9@[129.126.118.1]")]
        [DataRow("js@contoso.com9")]
        [DataRow("j.s@server1.contoso.com")]
        [DataRow("js@contoso.中国")]
        public void IsWellFormedEmail_ShouldReturnTrue_WhenEmailsAreCorrect(string emailAddress) =>
            emailAddress.IsWellFormedEmailAddress().Should().BeTrue();

        [TestMethod]
        [DataRow("j.@@.contoso.com")]
        [DataRow("j@..s@contoso.com")]
        [DataRow("js*@contoso.com.@")]
        [DataRow("js@contoso.@.com")]
        public void IsWellFormedEmail_ShouldReturnFalse_WhenEmailsAreIncorrect(string emailAddress) =>
            emailAddress.IsWellFormedEmailAddress().Should().BeFalse();

        [TestMethod]
        public void EqualsIgnoreCaseWithTrim_ShouldReturnTrue_WhenStringEndsWithEmptySpace() => "Test Value".EqualsIgnoreCaseWithTrim("test value ").Should().BeTrue();

        [TestMethod]
        public void EqualsIgnoreCaseWithTrim_ShouldReturnFalse_WhenStringsAreNotEqual() => "Test Value".EqualsIgnoreCaseWithTrim("test value&%$").Should().BeFalse();

        [TestMethod]
        public void EqualsWithTrim_ShouldReturnTrue_WhenStringEndsWithEmptySpace() => "Test Value".EqualsWithTrim("Test Value ").Should().BeTrue();

        [TestMethod]
        public void EqualsWithTrim_ShouldReturnFalse_WhenStringAreNotEqual() => "Test Value".EqualsWithTrim("Test Value&%$").Should().BeFalse();

        [TestMethod]
        public void StartsWithIgnoreCaseWithTrim_ShouldReturnTrue_WhenStringStartWithEmptySpace() => "Test Value".StartsWithIgnoreCaseWithTrim(" Test").Should().BeTrue();

        [TestMethod]
        public void StartsWithIgnoreCaseWithTrim_ShouldReturnFalse_WhenStringDoNotStartWithValue() => "Test Value".StartsWithIgnoreCaseWithTrim(" Test_").Should().BeFalse();

        [TestMethod]
        public void RegexSplit_SeparatorNotInString_NotSplit()
        {
            const string Input = "This is a regular old sentence, nothing special to see here.";
            string[] result = Input.RegexSplit("fox");

            result.Length.Should().Be(1);
            result.Single().Should().Be(Input);
        }

        [TestMethod]
        public void RegexSplit_IgnoreCaseFalseOrNotPassed_NotSplit()
        {
            const string Input = "HELLO WORLD! WE ARE SO EXCITED";
            string[] falseResult = Input.RegexSplit("world", ignoreCase: false);
            string[] nullResult = Input.RegexSplit("world");

            falseResult.Length.Should().Be(1);
            falseResult.Single().Should().Be(Input);

            nullResult.Length.Should().Be(1);
            nullResult.Single().Should().Be(Input);
        }

        [TestMethod]
        public void RegexSplit_IgnoreCaseTrue_CaseInsensitiveSplit()
        {
            const string Input = "HELLO WORLD! WE ARE SO EXCITED";
            string[] result = Input.RegexSplit("world", ignoreCase: true);

            result.Length.Should().Be(2);
            result[0].Should().Be("HELLO ");
            result[^1].Should().Be("! WE ARE SO EXCITED");
        }

        // --- Truncate ---

        [TestMethod]
        public void Truncate_ShorterThanMax_ReturnsOriginal() => "hello".Truncate(10).Should().Be("hello");

        [TestMethod]
        public void Truncate_LongerThanMax_TruncatesWithSuffix() => "hello world".Truncate(8).Should().Be("hello...");

        [TestMethod]
        public void Truncate_ExactLength_ReturnsOriginal() => "hello".Truncate(5).Should().Be("hello");

        [TestMethod]
        public void Truncate_MaxLengthZero_ReturnsEmpty() => "hello".Truncate(0).Should().BeEmpty();

        [TestMethod]
        public void Truncate_NegativeMaxLength_Throws()
        {
            Action act = () => "hello".Truncate(-1);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Truncate_CustomSuffix_Uses() => "hello world".Truncate(7, "~~").Should().Be("hello~~");

        // --- ToBase64 / FromBase64 ---

        [TestMethod]
        public void ToBase64_RoundTrips()
        {
            const string Original = "Hello, World!";
            string encoded = Original.ToBase64();
            encoded.Should().NotBe(Original);
            encoded.FromBase64().Should().Be(Original);
        }

        [TestMethod]
        public void ToBase64_EmptyString_ReturnsEmpty() => "".ToBase64().Should().BeEmpty();

        // --- Mask ---

        [TestMethod]
        public void Mask_DefaultVisibleChars_ShowsLast4() => "4111111111111111".Mask().Should().Be("************1111");

        [TestMethod]
        public void Mask_CustomVisibleChars() => "secret123".Mask(3).Should().Be("******123");

        [TestMethod]
        public void Mask_ShortString_ReturnsOriginal() => "abc".Mask(4).Should().Be("abc");

        [TestMethod]
        public void Mask_CustomMaskChar() => "secret".Mask(2, '#').Should().Be("####et");

        // --- ToSlug ---

        [TestMethod]
        public void ToSlug_BasicString_ReturnsSlug() => "Hello World!".ToSlug().Should().Be("hello-world");

        [TestMethod]
        public void ToSlug_SpecialChars_Removed() => "C# is great!!!".ToSlug().Should().Be("c-is-great");

        [TestMethod]
        public void ToSlug_AlreadySlug_ReturnsSame() => "already-a-slug".ToSlug().Should().Be("already-a-slug");

        // --- IsNullOrWhiteSpace ---

        [TestMethod]
        public void IsNullOrWhiteSpace_Null_ReturnsTrue() => default(string).IsNullOrWhiteSpace().Should().BeTrue();

        [TestMethod]
        public void IsNullOrWhiteSpace_Empty_ReturnsTrue() => "".IsNullOrWhiteSpace().Should().BeTrue();

        [TestMethod]
        public void IsNullOrWhiteSpace_Whitespace_ReturnsTrue() => "   ".IsNullOrWhiteSpace().Should().BeTrue();

        [TestMethod]
        public void IsNullOrWhiteSpace_HasContent_ReturnsFalse() => "hello".IsNullOrWhiteSpace().Should().BeFalse();

        // --- IsNumeric ---

        [TestMethod]
        [DataRow("123", true)]
        [DataRow("-45.67", true)]
        [DataRow("1.5E2", true)]
        [DataRow("0", true)]
        [DataRow("abc", false)]
        [DataRow("", false)]
        [DataRow("  ", false)]
        public void IsNumeric_ReturnsExpected(string input, bool expected) =>
            input.IsNumeric().Should().Be(expected);

        [TestMethod]
        public void IsNumeric_Null_ReturnsFalse() =>
            default(string)!.IsNumeric().Should().BeFalse();

        // --- IsGuid ---

        [TestMethod]
        public void IsGuid_ValidGuid_ReturnsTrue() =>
            Guid.NewGuid().ToString().IsGuid().Should().BeTrue();

        [TestMethod]
        [DataRow("not-a-guid")]
        [DataRow("")]
        [DataRow("  ")]
        public void IsGuid_Invalid_ReturnsFalse(string input) =>
            input.IsGuid().Should().BeFalse();

        [TestMethod]
        public void IsGuid_Null_ReturnsFalse() =>
            default(string)!.IsGuid().Should().BeFalse();

        // --- ToSnakeCase ---

        [TestMethod]
        [DataRow("MyPropertyName", "my_property_name")]
        [DataRow("camelCase", "camel_case")]
        [DataRow("already_snake", "already_snake")]
        [DataRow("kebab-case", "kebab_case")]
        [DataRow("HTMLParser", "html_parser")]
        [DataRow("", "")]
        public void ToSnakeCase_ConvertsCorrectly(string input, string expected) =>
            input.ToSnakeCase().Should().Be(expected);

        // --- ToKebabCase ---

        [TestMethod]
        [DataRow("MyPropertyName", "my-property-name")]
        [DataRow("camelCase", "camel-case")]
        [DataRow("snake_case", "snake-case")]
        [DataRow("already-kebab", "already-kebab")]
        [DataRow("", "")]
        public void ToKebabCase_ConvertsCorrectly(string input, string expected) =>
            input.ToKebabCase().Should().Be(expected);

        // --- ToCamelCase ---

        [TestMethod]
        [DataRow("MyPropertyName", "myPropertyName")]
        [DataRow("already_camel", "alreadyCamel")]
        [DataRow("kebab-case", "kebabCase")]
        [DataRow("PascalCase", "pascalCase")]
        [DataRow("", "")]
        public void ToCamelCase_ConvertsCorrectly(string input, string expected) =>
            input.ToCamelCase().Should().Be(expected);

        // --- Humanize ---

        [TestMethod]
        [DataRow("MyPropertyName", "My property name")]
        [DataRow("some_variable", "Some variable")]
        [DataRow("kebab-case", "Kebab case")]
        [DataRow("camelCase", "Camel case")]
        [DataRow("", "")]
        public void Humanize_ConvertsCorrectly(string input, string expected) =>
            input.Humanize().Should().Be(expected);

        // --- Reverse ---

        [TestMethod]
        [DataRow("hello", "olleh")]
        [DataRow("a", "a")]
        [DataRow("", "")]
        [DataRow("abba", "abba")]
        [DataRow("12345", "54321")]
        public void Reverse_ReversesCorrectly(string input, string expected) =>
            input.Reverse().Should().Be(expected);

        [TestMethod]
        [DataRow("my_property_name", "MyPropertyName")]
        [DataRow("my-property-name", "MyPropertyName")]
        [DataRow("myPropertyName", "MyPropertyName")]
        [DataRow("MyPropertyName", "MyPropertyName")]
        [DataRow("", "")]
        public void ToPascalCase_ConvertsCorrectly(string input, string expected) =>
            input.ToPascalCase().Should().Be(expected);

        [TestMethod]
        public void ToTitleCase_CapitalizesEachWord() =>
            "hello world from grondo".ToTitleCase().Should().Be("Hello World From Grondo");

        [TestMethod]
        [DataRow("<p>Hello <b>world</b></p>", "Hello world")]
        [DataRow("no tags here", "no tags here")]
        [DataRow("", "")]
        public void StripHtml_RemovesTags(string input, string expected) =>
            input.StripHtml().Should().Be(expected);

        [TestMethod]
        [DataRow("one two three", 3)]
        [DataRow("  one   two  ", 2)]
        [DataRow("", 0)]
        [DataRow("   ", 0)]
        [DataRow("single", 1)]
        public void WordCount_ReturnsCount(string input, int expected) =>
            input.WordCount().Should().Be(expected);

        [TestMethod]
        [DataRow("café", "cafe")]
        [DataRow("naïve", "naive")]
        [DataRow("résumé", "resume")]
        [DataRow("hello", "hello")]
        public void RemoveDiacritics_RemovesAccents(string input, string expected) =>
            input.RemoveDiacritics().Should().Be(expected);

        [TestMethod]
        public void Repeat_RepeatsString() =>
            "ab".Repeat(3).Should().Be("ababab");

        [TestMethod]
        public void Repeat_Zero_ReturnsEmpty() =>
            "ab".Repeat(0).Should().Be(string.Empty);

        [TestMethod]
        public void Repeat_Negative_Throws() =>
            FluentActions.Invoking(() => "ab".Repeat(-1)).Should().Throw<ArgumentOutOfRangeException>();

        [TestMethod]
        public void EnsureStartsWith_AddsPrefix_WhenMissing() =>
            "world".EnsureStartsWith("hello-").Should().Be("hello-world");

        [TestMethod]
        public void EnsureStartsWith_ReturnsAsIs_WhenAlreadyPresent() =>
            "hello-world".EnsureStartsWith("hello-").Should().Be("hello-world");

        [TestMethod]
        public void EnsureEndsWith_AddsSuffix_WhenMissing() =>
            "file".EnsureEndsWith(".txt").Should().Be("file.txt");

        [TestMethod]
        public void EnsureEndsWith_ReturnsAsIs_WhenAlreadyPresent() =>
            "file.txt".EnsureEndsWith(".txt").Should().Be("file.txt");

        [TestMethod]
        public void ReplaceMultiple_AppliesAllReplacements()
        {
            var replacements = new Dictionary<string, string> { ["foo"] = "bar", ["baz"] = "qux" };
            "foo and baz".ReplaceMultiple(replacements).Should().Be("bar and qux");
        }
    }
}
