using System.Text;
using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class ByteArrayExTests : BaseExtensionTest
    {
        // --- ToHexString ---

        [TestMethod]
        public void ToHexString_ReturnsLowercaseHex()
        {
            byte[] data = [0xDE, 0xAD, 0xBE, 0xEF];
            data.ToHexString().Should().Be("deadbeef");
        }

        [TestMethod]
        public void ToHexString_EmptyArray_ReturnsEmpty() => Array.Empty<byte>().ToHexString().Should().BeEmpty();

        // --- ToBase64 ---

        [TestMethod]
        public void ToBase64_ReturnsBase64String()
        {
            byte[] data = Encoding.UTF8.GetBytes("Hello");
            string result = data.ToBase64();
            result.Should().Be(Convert.ToBase64String(data));
        }

        // --- ComputeSha256 ---

        [TestMethod]
        public void ComputeSha256_Returns32Bytes()
        {
            byte[] data = Encoding.UTF8.GetBytes("test");
            byte[] hash = data.ComputeSha256();
            hash.Should().HaveCount(32);
        }

        [TestMethod]
        public void ComputeSha256_SameInput_SameHash()
        {
            byte[] data = Encoding.UTF8.GetBytes("hello");
            byte[] hash1 = data.ComputeSha256();
            byte[] hash2 = data.ComputeSha256();
            hash1.Should().BeEquivalentTo(hash2);
        }

        // --- FromHexString ---

        [TestMethod]
        public void FromHexString_RoundTrips()
        {
            byte[] original = [0xCA, 0xFE, 0xBA, 0xBE];
            string hex = original.ToHexString();
            byte[] result = hex.FromHexString();
            result.Should().BeEquivalentTo(original);
        }

        [TestMethod]
        public void FromHexString_UpperCase_Works()
        {
            byte[] result = "DEADBEEF".FromHexString();
            result.Should().BeEquivalentTo(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF });
        }

        // --- FromBase64ToBytes ---

        [TestMethod]
        public void FromBase64ToBytes_RoundTrips()
        {
            byte[] original = Encoding.UTF8.GetBytes("Hello, World!");
            string base64 = original.ToBase64();
            byte[] result = base64.FromBase64ToBytes();
            result.Should().BeEquivalentTo(original);
        }

        [TestMethod]
        public void FromBase64ToBytes_InvalidBase64_ThrowsFormatException()
        {
            Action act = () => "not-valid-base64!!!".FromBase64ToBytes();
            act.Should().Throw<FormatException>();
        }
    }
}

