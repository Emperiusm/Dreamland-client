using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class UploadRetryTests
    {
        [Test]
        public void BackoffSequenceMatchesExpected()
        {
            var delays = UploadRetry.GetBackoffDelays(3);
            Assert.AreEqual(1f, delays[0]);
            Assert.AreEqual(2f, delays[1]);
            Assert.AreEqual(4f, delays[2]);
        }
    }
}
