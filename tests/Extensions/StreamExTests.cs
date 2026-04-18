using System.Text;
using FluentAssertions;
using Grondo.Extensions;

namespace Grondo.Tests.Extensions
{
    [TestClass]
    public class StreamExTests : BaseExtensionTest
    {
        [TestMethod]
        public async Task ToByteArrayAsync_ReturnsBytes()
        {
            byte[] data = "Hello, World!"u8.ToArray();
            using var stream = new MemoryStream(data);

            byte[] result = await stream.ToByteArrayAsync(TestContext.CancellationToken);
            result.Should().BeEquivalentTo(data);
        }

        [TestMethod]
        public async Task ToByteArrayAsync_NullStream_ThrowsArgumentNullException()
        {
            Stream stream = null!;
            Func<Task<byte[]>> act = async () => await stream.ToByteArrayAsync(TestContext.CancellationToken);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ToStringAsync_ReturnsStringContent()
        {
            string text = "Hello, Stream!";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            string result = await stream.ToStringAsync(cancellationToken: TestContext.CancellationToken);
            result.Should().Be(text);
        }

        [TestMethod]
        public async Task ToStringAsync_CustomEncoding_ReturnsCorrectString()
        {
            string text = "Hello ASCII";
            using var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

            string result = await stream.ToStringAsync(Encoding.ASCII, TestContext.CancellationToken);
            result.Should().Be(text);
        }

        [TestMethod]
        public async Task ToMemoryStreamAsync_ReturnsCopyAtPositionZero()
        {
            byte[] data = "copy me"u8.ToArray();
            using var source = new MemoryStream(data);

            MemoryStream copy = await source.ToMemoryStreamAsync(TestContext.CancellationToken);

            copy.Position.Should().Be(0);
            copy.ToArray().Should().BeEquivalentTo(data);
            await copy.DisposeAsync();
        }

        [TestMethod]
        public async Task ToMemoryStreamAsync_NullStream_ThrowsArgumentNullException()
        {
            Stream stream = null!;
            Func<Task<MemoryStream>> act = async () => await stream.ToMemoryStreamAsync(TestContext.CancellationToken);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ToByteArrayAsync_ResetsPosition_WhenSeekable()
        {
            byte[] data = "position test"u8.ToArray();
            using var stream = new MemoryStream(data);
            stream.Position = 5; // Move position forward

            byte[] result = await stream.ToByteArrayAsync(TestContext.CancellationToken);
            result.Should().BeEquivalentTo(data); // Still gets full content
        }

        public TestContext TestContext { get; set; } = null!;
    }
}

