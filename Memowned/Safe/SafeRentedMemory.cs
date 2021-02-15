using System;
using System.Buffers;

namespace Memowned {
    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> rented from a <see cref="ArrayPool{T}"/>.
    /// This type guarantees that the rented memory is returnd to the pool.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public sealed class SafeRentedMemory<T> : SafeDisposableStructWrapper<RentedMemory<T>>, IMemoryOwner<T> {
        /// <summary>
        /// Constructs a new <see cref="SafeRentedMemory{T}"/> instance wrapping an <typeparamref name="T"/> array rented from the given <see cref="ArrayPool{T}"/> with at least <paramref name="minimumLength"/>.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the buffer to rent.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minimumLength"/> is invalid.</exception>
        public SafeRentedMemory(int minimumLength, ArrayPool<T>? pool = null) : this(new(minimumLength, pool)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeRentedMemory{T}"/> instance wrapping the given <see cref="RentedMemory{T}"/>.
        /// </summary>
        /// <param name="rentedMemory">The <see cref="RentedMemory{T}"/> instance to wrap.</param>
        public SafeRentedMemory(RentedMemory<T> rentedMemory) : base(rentedMemory) { }

        /// <inheritdoc/>
        public Memory<T> Memory => Value.Memory;

        /// <inheritdoc/>
        public Span<T> Span => Value.Span;

        /// <summary>
        /// The number of items in the current instance.
        /// </summary>
        public int Length => Value.Length;

        /// <summary>
        /// Returns the <see cref="RentedMemory{T}"/> instance wrapped by the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="RentedMemory{T}"/> instance wraps the same array as the current instance only the returned one should
        /// be used. The current instance will be disposed after the call.
        /// </remarks>
        public RentedMemory<T> AsUnsafe() => MoveValue();

        /// <summary>
        /// Returns a <see cref="SafeOwnedMemory{T, RentedMemory{T}}"/> instance wrapping the underlying <see cref="RentedMemory{T}"/> instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeOwnedMemory{T, RentedMemory{T}}"/> instance wraps the same array as the current instance only the returned
        /// one should be used. The current instance will be disposed after the call.
        /// </remarks>
        public SafeOwnedMemory<T, RentedMemory<T>> AsOwned() => new(Memory, MoveValue());

        public static implicit operator Memory<T>(SafeRentedMemory<T> rentedMemory) => rentedMemory.Memory;
        public static implicit operator Span<T>(SafeRentedMemory<T> rentedMemory) => rentedMemory.Span;
    }
}
