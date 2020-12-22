using System.Buffers;
using System.Collections.Generic;
using NUnit.Framework;

namespace Memowned.Test {
    [TestFixture]
    public class RentedMemoryTests {
        [Test]
        public void RespectsRequestedSize() {
            var size = 71;
            using var memory = new RentedMemory<byte>(size);
            Assert.GreaterOrEqual(memory.DangerousGetArray().Length, size, "The rented array has insufficient size.");
        }

        [Test]
        public void UsesGivenPool() {
            var pool = new MockArrayPool<byte>();
            var memory = new RentedMemory<byte>(42, pool);
            Assert.AreEqual(memory.Pool, pool, "Provided pool was not stored.");

            var array = memory.DangerousGetArray();
            Assert.IsTrue(pool.Rented.Contains(array), "Provided pool was not used for renting.");

            memory.Dispose();
            Assert.IsFalse(pool.Rented.Contains(array), "Provided pool was not used for returnig.");
        }

        [Test]
        public void LengthIsCorrect() {
            var size = 71;
            using var memory = new RentedMemory<byte>(size);
            Assert.AreEqual(memory.Length, size, ".Length returns incorrect value.");
        }

        [Test]
        public void MemoryLengthIsCorrect() {
            var size = 71;
            using var memory = new RentedMemory<byte>(size);
            Assert.AreEqual(memory.Memory.Length, size, ".Memory has incorrect size.");
        }

        [Test]
        public void SpanLengthIsCorrect() {
            var size = 71;
            using var memory = new RentedMemory<byte>(size);
            Assert.AreEqual(memory.Span.Length, size, ".Span has incorrect size.");
        }


        private class MockArrayPool<T> : ArrayPool<T> {
            public readonly HashSet<T[]> Rented = new();
            public override T[] Rent(int minimumLength) {
                var array = new T[minimumLength];
                Rented.Add(array);
                return array;
            }

            public override void Return(T[] array, bool clearArray = false) {
                Rented.Remove(array);
            }
        }
    }
}
