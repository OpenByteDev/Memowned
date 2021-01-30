using System;
using System.Buffers;
using System.ComponentModel;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that wraps a <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly struct UnownedMemory<T> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<UnownedMemory<T>> {
        /// <inheritdoc/>
        public Memory<T> Memory { get; }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> wrapping the rented memory.
        /// </summary>
        public Span<T> Span => Memory.Span;

        /// <summary>
        /// The number of items in the current instance.
        /// </summary>
        public int Length => Memory.Length;

        /// <inheritdoc/>
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        /// <summary>
        /// Constructs a new <see cref="UnownedMemory{T}"/> instance wrapping the given <see cref="Memory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap.</param>
        public UnownedMemory(Memory<T> memory) =>
            Memory = memory;

        /// <inheritdoc/>
        public void Dispose() { }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => other is UnownedMemory<T> memory && Equals(memory);

        /// <inheritdoc/>
        public bool Equals(UnownedMemory<T> other) => Memory.Equals(other.Memory);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => Memory.GetHashCode();

        /// <summary>
        /// For <see cref="Memory{char}"/>, returns a new instance of string that represents the characters pointed to by the memory.
        /// Otherwise, returns a <see cref="string"/> with the name of the type and the number of elements.
        /// </summary>
        public override string ToString() {
            // Special case string-like memeory.
            if (typeof(T) == typeof(char))
                return Memory.ToString();

            // Same representation used in Memory<T>
            return $"UnownedMemory<{typeof(T)}>[{Length}]";
        }

        /// Returns an empty <see cref="UnownedMemory{T}"/> instance.
        public static UnownedMemory<T> Empty => default;

        public static implicit operator Memory<T>(UnownedMemory<T> memory) => memory.Memory;
        public static implicit operator Span<T>(UnownedMemory<T> memory) => memory.Span;
        public static implicit operator OwnedMemory<T, UnownedMemory<T>>(UnownedMemory<T> memory) => OwnedMemory<T>.Owned(memory);
        public static implicit operator OwnedSpan<T, UnownedMemory<T>>(UnownedMemory<T> memory) => OwnedSpan<T>.Owned(memory.Span, memory);
        public static bool operator ==(UnownedMemory<T> left, UnownedMemory<T> right) => left.Equals(right);
        public static bool operator !=(UnownedMemory<T> left, UnownedMemory<T> right) => !(left == right);
    }
}
