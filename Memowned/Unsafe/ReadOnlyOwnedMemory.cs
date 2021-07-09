using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IReadOnlyMemoryOwner{T}"/> implementation that holds a <see cref="ReadOnlyMemory{T}"/> and
    /// optionally an <see cref="IDisposable"/> that represents its owner and which frees the underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly struct ReadOnlyOwnedMemory<T> : IReadOnlyMemoryOwner<T>, IEquatable<ReadOnlyOwnedMemory<T>> {
        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly ReadOnlyMemory<T> Memory { get; }
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public readonly IDisposable? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T}"/> instance with the given <see cref="ReadOnlyMemory{T}"/>
        /// which is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlyMemory{T}"/> instance.</param>
        public ReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, IDisposable? owner = default) =>
            (Memory, Owner) = (memory, owner);
        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T}"/> instance from the given <see cref="IReadOnlyMemoryOwner{T}"/>.
        /// The <see cref="ReadOnlyMemory{T}"/> of the constructed instance will be from the given <see cref="IReadOnlyMemoryOwner{T}"/>
        /// and the owner will be the <see cref="IReadOnlyMemoryOwner{T}"/> itself.
        /// </summary>
        /// <param name="memoryOwner">The <see cref="IReadOnlyMemoryOwner{T}"/> used to construct the instance.</param>
        public ReadOnlyOwnedMemory(IReadOnlyMemoryOwner<T> memoryOwner) : this(memoryOwner.Memory, memoryOwner) { }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Memory"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Returns a <see cref="SafeReadOnlyOwnedMemory{T}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeReadOnlyOwnedMemory{T}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public SafeReadOnlyOwnedMemory<T> AsSafe() => new(this);
        /// <summary>
        /// Creates an <see cref="ReadOnlyOwnedSpan{T}"/> that wraps the <see cref="ReadOnlySpan{T}"/> of the <see cref="ReadOnlyMemory{T}"/> associated
        /// with the current instance and the owner of the current instanc.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="ReadOnlyOwnedSpan{T}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public ReadOnlyOwnedSpan<T> AsOwnedSpan() => new(Memory.Span, Owner);
        /// <summary>
        /// Creates a <see cref="OwnedMemory{T}"/> from the current instace.
        /// This may produce unexpected results if the underlying memory is not writeable.
        /// </summary>
        public OwnedMemory<T> DangerousAsOwnedMemory() => new(MemoryMarshal.AsMemory(Memory), Owner);

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T}"/> instance without an owner from the given <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedMemory{T}"/> instance.</returns>
        public static ReadOnlyOwnedMemory<T, IDisposable> Unowned(ReadOnlyMemory<T> memory) => new(memory, null);
        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T}"/> instance from the given <see cref="Memory{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <typeparam name="O">The type of the owner of the <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="memory"/>.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedMemory{T}"/> instance.</returns>
        public static ReadOnlyOwnedMemory<T, O> Owned<O>(ReadOnlyMemory<T> memory, O owner) where O : IDisposable => new(memory, owner);
        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T}"/> instance from the given <see cref="IReadOnlyMemoryOwner{T}"/>.
        /// The <see cref="ReadOnlyMemory{T}"/> of the constructed instance will be from the given <see cref="IReadOnlyMemoryOwner{T}"/> and
        /// the owner will be the <see cref="IReadOnlyMemoryOwner{T}"/> itself.
        /// </summary>
        /// <typeparam name="O">The subtype of the <see cref="IReadOnlyMemoryOwner{T}"/> instance.</typeparam>
        /// <param name="memoryOwner">The <see cref="IReadOnlyMemoryOwner{T}"/> used to construct the instance.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedMemory{T}"/> instance.</returns>
        public static ReadOnlyOwnedMemory<T, O> Owned<O>(O memoryOwner) where O : IReadOnlyMemoryOwner<T> => new(memoryOwner.Memory, memoryOwner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) =>
            obj is ReadOnlyOwnedMemory<T> ownedMemory && Equals(ownedMemory);

        /// <inheritdoc/>
        public bool Equals(ReadOnlyOwnedMemory<T> other) =>
            Memory.Equals(other.Memory) && Owner == other.Owner;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"ReadOnlyOwnedMemory<{typeof(T)}>[Memory={Memory.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="ReadOnlyOwnedMemory{T}"/> instance.
        /// </summary>
        public static ReadOnlyOwnedMemory<T> Empty => default;

        public static implicit operator ReadOnlyOwnedMemory<T>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(ReadOnlyOwnedMemory<T> ownedMemory) => ownedMemory.Memory;
        public static implicit operator ReadOnlyOwnedMemory<T, IDisposable>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);

        public static explicit operator ReadOnlyOwnedSpan<T>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);

        public static bool operator ==(ReadOnlyOwnedMemory<T> left, ReadOnlyOwnedMemory<T> right) => left.Equals(right);
        public static bool operator !=(ReadOnlyOwnedMemory<T> left, ReadOnlyOwnedMemory<T> right) => !(left == right);
    }

    /// <summary>
    /// An <see cref="IReadOnlyMemoryOwner{T}"/> implementation that holds a <see cref="ReadOnlyMemory{T}"/> and
    /// optionally an <typeparamref name="O"/> that represents its owner and which frees the
    /// <see cref="ReadOnlyMemory{T}"/> on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public readonly struct ReadOnlyOwnedMemory<T, O> : IReadOnlyMemoryOwner<T>, IEquatable<ReadOnlyOwnedMemory<T, O>>
        where O : IDisposable {
        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly ReadOnlyMemory<T> Memory { get; }
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public readonly O? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T, O}"/> instance with the given <see cref="ReadOnlyMemory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlyMemory{T}"/> instance.</param>
        public ReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, O? owner = default) =>
            (Memory, Owner) = (memory, owner);

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Memory"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Returns a <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public SafeReadOnlyOwnedMemory<T, O> AsSafe() => new(this);
        /// <summary>
        /// Creates an <see cref="ReadOnlyOwnedSpan{T, O}"/> that wraps the <see cref="ReadOnlySpan{T}"/> of the <see cref="ReadOnlyMemory{T}"/> associated
        /// with the current instance and the owner of the current instanc.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="ReadOnlyOwnedSpan{T, O}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public ReadOnlyOwnedSpan<T, O> AsOwnedSpan() => new(Memory.Span, Owner);
        /// <summary>
        /// Creates a <see cref="OwnedMemory{T, O}"/> from the current instace.
        /// This may produce unexpected results if the underlying memory is not writeable.
        /// </summary>
        public OwnedMemory<T, O> DangerousAsOwnedMemory() => new(MemoryMarshal.AsMemory(Memory), Owner);

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedMemory{T, O}"/> instance without an owner from the given <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedMemory{T, O}"/> instance.</returns>
        public static ReadOnlyOwnedMemory<T, O> Unowned(ReadOnlyMemory<T> memory) => new(memory, default);
        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T, O}"/> instance from the given <see cref="ReadOnlyMemory{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="memory"/>.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedMemory{T, O}"/> instance.</returns>
        public static ReadOnlyOwnedMemory<T, O> Owned(ReadOnlyMemory<T> memory, O owner) => new(memory, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) =>
            obj is ReadOnlyOwnedMemory<T, O> ownedMemory && Equals(ownedMemory);

        /// <inheritdoc/>
        public bool Equals(ReadOnlyOwnedMemory<T, O> other) =>
            Memory.Equals(other.Memory) && Owner?.Equals(other.Owner) == true;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"ReadOnlyOwnedMemory<{typeof(T)}, {typeof(O)}>[Memory={Memory.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="ReadOnlyOwnedMemory{T, O}"/> instance.
        /// </summary>
        public static ReadOnlyOwnedMemory<T, O> Empty => default;

        public static implicit operator ReadOnlyOwnedMemory<T, O>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(ReadOnlyOwnedMemory<T, O> ownedMemory) => ownedMemory.Memory;
        public static implicit operator ReadOnlyOwnedMemory<T>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);

        public static explicit operator ReadOnlyOwnedSpan<T, O>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);

        public static bool operator ==(ReadOnlyOwnedMemory<T, O> left, ReadOnlyOwnedMemory<T, O> right) => left.Equals(right);
        public static bool operator !=(ReadOnlyOwnedMemory<T, O> left, ReadOnlyOwnedMemory<T, O> right) => !(left == right);
    }
}
