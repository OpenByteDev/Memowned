using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> rented from a <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly struct RentedMemory<T> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<RentedMemory<T>> {
        /// <summary>
        /// The underlying rented buffer.
        /// </summary>
        private readonly T[] _buffer;

        /// <summary>
        /// The <see cref="ArrayPool{T}"/> instance used to rent <see cref="_buffer"/>.
        /// </summary>
        public readonly ArrayPool<T> Pool { get; }

        /// <inheritdoc/>
        public readonly Memory<T> Memory => _buffer.AsMemory(0, Length);

        /// <inheritdoc/>
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        /// <summary>
        /// Gets a <see cref="Span{T}"/> wrapping the rented memory.
        /// </summary>
        public readonly Span<T> Span {
            get {
#if NET5_0
                return MemoryMarshal.CreateSpan(ref DangerousGetReference(), Length);
#else
                return _buffer.AsSpan(0, Length);
#endif
            }
        }

        /// <summary>
        /// The number of items in the current instance.
        /// </summary>
        public readonly int Length { get; }

        /// <summary>
        /// Indicates whether the current instance is empty.
        /// </summary>
        public readonly bool IsEmpty => Length == 0;

        /// <summary>
        /// Constructs a new <see cref="RentedMemory{T}"/> instance wrapping an <typeparamref name="T"/> array rented from the given <see cref="ArrayPool{T}"/> with at least <paramref name="minimumLength"/>.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the buffer to rent.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minimumLength"/> is invalid.</exception>
        public RentedMemory(int minimumLength, ArrayPool<T>? pool = null) {
            Pool = pool ?? ArrayPool<T>.Shared;
            _buffer = Pool.Rent(minimumLength);
            Length = minimumLength;
        }

        /// <summary>
        /// Returns a reference to specified element of the rented memory.
        /// </summary>
        /// <param name="index"></param>
        public ref T this[int index] => ref this[Index.FromStart(index)];

        /// <summary>
        /// Returns a reference to specified element of the rented memory.
        /// </summary>
        /// <param name="index"></param>
        public ref T this[Index index] => ref _buffer[index.GetOffset(Length)];

        /// <summary>
        /// Returns a <see cref="Span{T}"/> over the given <see cref="Range"/> of rented memory.
        /// </summary>
        /// <param name="range"></param>
        public Span<T> this[Range range] {
            get {
                var (offset, count) = range.GetOffsetAndLength(Length);
                return _buffer.AsSpan(offset, count);
            }
        }

        /// <summary>
        /// Gets the underlying rented <typeparamref name="T"/> array.
        /// </summary>
        /// <remarks>
        /// This method is only meant to be used when working with APIs that only accept an array as input, and should be used with caution.
        /// As array is rented from a pool it should not be used after the current <see cref="RentedMemory{T}"/> instance is disposed.
        /// </remarks>
        public T[] DangerousGetArray() => _buffer;

        /// <summary>
        /// Gets an <see cref="ArraySegment{T}"/> instance wrapping the underlying rented <typeparamref name="T"/> array.
        /// </summary>
        /// <remarks>
        /// This method is only meant to be used when working with APIs that only accept an array as input, and should be used with caution.
        /// As array is rented from a pool it should not be used after the current <see cref="RentedMemory{T}"/> instance is disposed.
        /// </remarks>
        public ArraySegment<T> DangerousGetArraySegment() => new(_buffer, 0, Length);

#if NET5_0
        /// <summary>
        /// Returns a reference to the first element of the rented memory.
        /// </summary>
        public ref T DangerousGetReference() => ref MemoryMarshal.GetArrayDataReference(_buffer);
#endif

        /// <summary>
        /// Returns a <see cref="SafeRentedMemory{T}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeRentedMemory{T}"/> instance wraps the same array as the current instance only the returned one should
        /// be used and disposed.
        /// </remarks>
        public SafeRentedMemory<T> AsSafe() => new(this);

        /// <summary>
        /// Returns a <see cref="OwnedMemory{T, RentedMemory{T}}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="OwnedMemory{T, RentedMemory{T}}"/> instance wraps the same array as the current instance only the returned one should
        /// be used and disposed.
        /// </remarks>
        public OwnedMemory<T, RentedMemory<T>> AsOwnedMemory() => new(Memory, this);

        /// <summary>
        /// Returns an <see cref="OwnedSpan{T, RentedMemory{T}}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="OwnedSpan{T, RentedMemory{T}}"/> instance wraps the same array as the current instance only the returned one should
        /// be used and disposed.
        /// </remarks>
        public OwnedSpan<T, RentedMemory<T>> AsOwnedSpan() => new(Span, this);

        /// <inheritdoc/>
        public void Dispose() => Pool?.Return(_buffer);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => other is RentedMemory<T> rentedMemory && Equals(rentedMemory);

        /// <inheritdoc/>
        public bool Equals(RentedMemory<T> other) => _buffer == other._buffer;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => HashCode.Combine(_buffer.GetHashCode(), Pool.GetHashCode(), Length.GetHashCode());

        /// <summary>
        /// For <see cref="Memory{Char}"/>, returns a new instance of string that represents the characters pointed to by the memory.
        /// Otherwise, returns a <see cref="string"/> with the name of the type and the number of elements.
        /// </summary>
        public override string ToString() {
            // Special case string-like memeory.
            if (typeof(T) == typeof(char) && _buffer is char[] chars)
                return new string(chars, 0, Length);

            // Same representation used in Memory<T>
            return $"RentedMemory<{typeof(T)}>[{Length.ToString()}]";
        }

        /// <summary>
        /// Returns an empty <see cref="RentedMemory{T}"/> instance.
        /// </summary>
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
