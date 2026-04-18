using System.Collections.ObjectModel;
using System.Net;
using FluentAssertions;
using Grondo.Exceptions;

namespace Grondo.Tests.Exceptions
{
    [TestClass]
    public class ValidationExceptionTests : BaseExceptionTest
    {
        [TestMethod]
        public void SingleFieldConstructor_ProducesExpectedShape()
        {
            var ex = new ValidationException("Email", "Email is required");

            ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            ex.MessageHeader.Should().Be("Validation Failed");
            ex.Message.Should().Be("Email: Email is required");
            ex.Errors.Should().ContainKey("Email");
            ex.Errors["Email"].Should().ContainSingle().Which.Should().Be("Email is required");
        }

        [TestMethod]
        public void SingleFieldConstructor_CustomMessageHeader_IsPreserved()
        {
            var ex = new ValidationException("Email", "Invalid", "Custom Header");
            ex.MessageHeader.Should().Be("Custom Header");
        }

        [TestMethod]
        public void MutableDictionaryConstructor_NullErrors_Throws()
        {
            Action act = () => _ = new ValidationException((IDictionary<string, string[]>)null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("errors");
        }

        [TestMethod]
        public void ReadOnlyDictionaryConstructor_NullErrors_Throws()
        {
            Action act = () => _ = new ValidationException(
                (IReadOnlyDictionary<string, IReadOnlyList<string>>)null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("errors");
        }

        [TestMethod]
        public void MutableDictionaryConstructor_CopiesDefensively_OuterMutationDoesNotAffectErrors()
        {
            var source = new Dictionary<string, string[]>
            {
                ["Email"] = ["Required"],
            };

            var ex = new ValidationException(source);

            source["Age"] = ["Too young"];
            source["Email"] = ["Overwritten"];

            ex.Errors.Should().HaveCount(1);
            ex.Errors.Should().ContainKey("Email");
            ex.Errors["Email"].Should().ContainSingle().Which.Should().Be("Required");
        }

        [TestMethod]
        public void MutableDictionaryConstructor_CopiesDefensively_InnerArrayMutationDoesNotAffectErrors()
        {
            var messages = new[] { "Required" };
            var source = new Dictionary<string, string[]> { ["Email"] = messages };

            var ex = new ValidationException(source);
            messages[0] = "Mutated";

            ex.Errors["Email"].Should().ContainSingle().Which.Should().Be("Required");
        }

        [TestMethod]
        public void ReadOnlyDictionaryConstructor_CopiesDefensively()
        {
            var mutableInner = new List<string> { "Required" };
            var mutableOuter = new Dictionary<string, IReadOnlyList<string>>
            {
                ["Email"] = mutableInner,
            };

            var ex = new ValidationException(mutableOuter);

            mutableInner.Add("Too short");
            mutableOuter["Age"] = new List<string> { "Too young" };

            ex.Errors.Should().HaveCount(1);
            ex.Errors["Email"].Should().ContainSingle().Which.Should().Be("Required");
        }

        [TestMethod]
        public void Errors_IsReadOnlyDictionary_OfReadOnlyLists()
        {
            var ex = new ValidationException("Email", "Required");

            ex.Errors.Should().BeAssignableTo<IReadOnlyDictionary<string, IReadOnlyList<string>>>();
            ex.Errors.Should().BeOfType<ReadOnlyDictionary<string, IReadOnlyList<string>>>();
            ex.Errors["Email"].Should().BeAssignableTo<IReadOnlyList<string>>();
            ex.Errors["Email"].Should().BeOfType<ReadOnlyCollection<string>>();
        }

        [TestMethod]
        public void MutableDictionaryConstructor_WithNullInnerArray_NormalisesToEmptyList()
        {
            var source = new Dictionary<string, string[]>
            {
                ["Email"] = null!,
            };

            var ex = new ValidationException(source);

            ex.Errors.Should().ContainKey("Email");
            ex.Errors["Email"].Should().BeEmpty();
        }

        [TestMethod]
        public void Throwing_ValidationException_CarriesStatusCodeAndErrors()
        {
            Action act = () => ThrowException(new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["Email"] = ["Required", "Invalid"],
                    ["Age"] = ["Too young"],
                }));

            var thrown = act.Should().Throw<ValidationException>().Which;
            thrown.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            thrown.Errors.Should().HaveCount(2);
            thrown.Errors["Email"].Should().BeEquivalentTo(new[] { "Required", "Invalid" });
            thrown.Errors["Age"].Should().BeEquivalentTo(new[] { "Too young" });
        }
    }
}
