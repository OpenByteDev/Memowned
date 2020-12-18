using System;
using System.Buffers;

namespace OwnedMemory {
    public readonly struct RentedMemory<T> : IMemoryOwner<T> {
        private readonly T[] _buffer;
        private readonly ArrayPool<T> _pool;

        public readonly Memory<T> Memory => _buffer.AsMemory(0, Length);
        public readonly Span<T> Span {
            get {
                // return MemoryMarshal.CreateSpan(ref _buffer[0], Length);
                return _buffer.AsSpan(0, Length);
            }
        }
        public readonly int Length { get; }

        public RentedMemory(int minimumLength, ArrayPool<T>? pool = null) {
            _pool = pool ?? ArrayPool<T>.Shared;
            _buffer = _pool.Rent(minimumLength);
            Length = minimumLength;
        }

        public void Dispose() => _pool.Return(_buffer);

        public SafeRentedMemory<T> AsSafe() => new(this);
        public OwnedMemory<T, RentedMemory<T>> AsOwnedMemory() => new(Memory, this);
        public OwnedSpan<T, RentedMemory<T>> AsOwnedSpan() => new(Span, this);

        public static implicit operator Memory<T>(RentedMemory<T> rentedMemory) => rentedMemory.Memory;
        public static implicit operator Span<T>(RentedMemory<T> rentedMemory) => rentedMemory.Span;
        public static implicit operator OwnedMemory<T, RentedMemory<T>>(RentedMemory<T> rentedMemory) => rentedMemory.AsOwnedMemory();
        public static implicit operator OwnedSpan<T, RentedMemory<T>>(RentedMemory<T> rentedMemory) => rentedMemory.AsOwnedSpan();
        public static implicit operator SafeRentedMemory<T>(RentedMemory<T> rentedMemory) => rentedMemory.AsSafe();
    }
}
