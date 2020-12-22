using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Memowned.Interfaces;

namespace Memowned {
    public readonly struct RentedMemory<T> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<RentedMemory<T>> {
        private readonly T[] _buffer;
        public readonly ArrayPool<T> Pool { get; }
        public readonly Memory<T> Memory => _buffer.AsMemory(0, Length);
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;
        public readonly Span<T> Span => MemoryMarshal.CreateSpan(ref DangerousGetReference(), Length);
        public readonly int Length { get; }

        public RentedMemory(int minimumLength, ArrayPool<T>? pool = null) {
            Pool = pool ?? ArrayPool<T>.Shared;
            _buffer = Pool.Rent(minimumLength);
            Length = minimumLength;
        }

        public T[] DangerousGetArray() => _buffer;
        public ref T DangerousGetReference() => ref MemoryMarshal.GetArrayDataReference(_buffer);

        public SafeRentedMemory<T> AsSafe() => new(this);
        public OwnedMemory<T, RentedMemory<T>> AsOwnedMemory() => new(Memory, this);
        public OwnedSpan<T, RentedMemory<T>> AsOwnedSpan() => new(Span, this);

        public void Dispose() => Pool.Return(_buffer);

        public override bool Equals(object? other) => other is RentedMemory<T> rentedMemory && Equals(rentedMemory);
        public bool Equals(RentedMemory<T> other) => _buffer == other._buffer;
        public override int GetHashCode() => HashCode.Combine(_buffer.GetHashCode(), Pool.GetHashCode(), Length.GetHashCode());

        public static RentedMemory<T> Empty => default;

        public static implicit operator Memory<T>(RentedMemory<T> rentedMemory) => rentedMemory.Memory;
        public static implicit operator Span<T>(RentedMemory<T> rentedMemory) => rentedMemory.Span;
        public static implicit operator OwnedMemory<T, RentedMemory<T>>(RentedMemory<T> rentedMemory) => rentedMemory.AsOwnedMemory();
        public static implicit operator OwnedSpan<T, RentedMemory<T>>(RentedMemory<T> rentedMemory) => rentedMemory.AsOwnedSpan();
        public static implicit operator SafeRentedMemory<T>(RentedMemory<T> rentedMemory) => rentedMemory.AsSafe();
        public static bool operator ==(RentedMemory<T> left, RentedMemory<T> right) => left.Equals(right);
        public static bool operator !=(RentedMemory<T> left, RentedMemory<T> right) => !(left == right);
    }
}
