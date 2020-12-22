using System;
using NUnit.Framework;

namespace Memowned.Test {
    [TestFixture]
    public class SafeRentedMemoryTests {
        [Test]
        public void DoesNotAllowUseAfterDispose() {
            var size = 71;
            var memory = new SafeRentedMemory<byte>(size);
            memory.Dispose();
            Assert.Throws<ObjectDisposedException>(() => memory.Span.Clear());
            Assert.Throws<ObjectDisposedException>(() => memory.Memory.Span.Clear());
        }

        [Test]
        public void AsUnsafeDisposesInstance() {
            var size = 71;
            var memory = new SafeRentedMemory<byte>(size);
            Assert.IsFalse(memory.IsDisposed);
            memory.AsUnsafe();
            Assert.IsTrue(memory.IsDisposed);
        }
    }
}
