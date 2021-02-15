using System;
using System.ComponentModel;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IReadOnlyMemoryOwner{T}"/> implementation that wraps a <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly struct ReadOnlyUnownedMemory<T> : IReadOnlyMemoryOwner<T>, IEquatable<ReadOnlyUnownedMemory<T>> {
        /// <inheritdoc/>
        public ReadOnlyMemory<T> Memory { get; }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> wrapping the rented memory.
        /// </summary>
        public ReadOnlySpan<T> Span => Memory.Span;

        /// <summary>
        /// The number of items in the current instance.
        /// </summary>
        public int Length => Memory.Length;

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyUnownedMemory{T}"/> instance wrapping the given <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap.</param>
        public ReadOnlyUnownedMemory(ReadOnlyMemory<T> memory) =>
            Memory = memory;

        /// <inheritdoc/>
        public void Dispose() { }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => other is ReadOnlyUnownedMemory<T> memory && Equals(memory);

        /// <inheritdoc/>
        public bool Equals(ReadOnlyUnownedMemory<T> other) => Memory.Equals(other.Memory);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => Memory.GetHashCode();

        /// <summary>
        /// For <see cref="ReadOnlyMemory{char}"/>, returns a new instance of string that represents the characters pointed to by the memory.
        /// Otherwise, returns a <see cref="string"/> with the name of the type and the number of elements.
        /// </summary>
        public override string ToString() {
            // Special case string-like memeory.
            if (typeof(T) == typeof(char))
                return Memory.ToString();

            // Same representation used in Memory<T>
            return $"ReadOnlyUnownedMemory<{typeof(T)}>[{Length.ToString()}]";
        }

        /// <summary>
        /// Returns an empty <see cref="ReadOnlyUnownedMemory{T}"/> instance.
        /// </summary>
        public static ReadOnlyUnownedMemory<T> Empty => default;

        public static implicit operator ReadOnlyMemory<T>(ReadOnlyUnownedMemory<T> memory) => memory.Memory;
        public static implicit operator ReadOnlySpan<T>(ReadOnlyUnownedMemory<T> memory) => memory.Span;
        public static implicit operator ReadOnlyOwnedMemory<T, ReadOnlyUnownedMemory<T>>(ReadOnlyUnownedMemory<T> memory) => ReadOnlyOwnedMemory<T>.Owned(memory);

        public static explicit operator ReadOnlyOwnedSpan<T, ReadOnlyUnownedMemory<T>>(ReadOnlyUnownedMemory<T> memory) => ReadOnlyOwnedSpan<T>.Owned(memory.Span, memory);

        public static bool operator ==(ReadOnlyUnownedMemory<T> left, ReadOnlyUnownedMemory<T> right) => left.Equals(right);
        public static bool operator !=(ReadOnlyUnownedMemory<T> left, ReadOnlyUnownedMemory<T> right) => !(left == right);
    }
}
