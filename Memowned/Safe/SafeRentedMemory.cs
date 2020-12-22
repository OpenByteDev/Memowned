using System;
using System.Buffers;

namespace Memowned {
    public sealed class SafeRentedMemory<T> : SafeDisposableStructWrapper<RentedMemory<T>>, IMemoryOwner<T> {
        public SafeRentedMemory(int minimumLength, ArrayPool<T>? pool = null) : this(new(minimumLength, pool)) { }
        public SafeRentedMemory(RentedMemory<T> memory) : base(memory) { }

        public Memory<T> Memory => Value.Memory;
        public Span<T> Span => Value.Span;
        public int Length => Value.Length;

        public RentedMemory<T> AsUnsafe() => MoveValue();
        public SafeOwnedMemory<T, RentedMemory<T>> AsOwned() => new(Memory, MoveValue());

        public static implicit operator Memory<T>(SafeRentedMemory<T> rentedMemory) => rentedMemory.Memory;
        public static implicit operator Span<T>(SafeRentedMemory<T> rentedMemory) => rentedMemory.Span;
    }
}
