using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> and
    /// optionally an <see cref="IDisposable"/> that represents its owner and which frees the
    /// underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly struct OwnedMemory<T> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<OwnedMemory<T>> {
        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly Memory<T> Memory { get; }
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public readonly IDisposable? Owner { get; }
        /// <summary>
        /// The read-only owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T}"/> instance with the given <see cref="Memory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Memory{T}"/> instance.</param>
        public OwnedMemory(Memory<T> memory, IDisposable? owner = default) =>
            (Memory, Owner) = (memory, owner);
        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T}"/> instance from the given <see cref="IMemoryOwner{T}"/>.
        /// The <see cref="Memory{T}"/> of the constructed instance will be from the given <see cref="IMemoryOwner{T}"/> and
        /// the owner will be the <see cref="IMemoryOwner{T}"/> itself.
        /// </summary>
        /// <param name="memoryOwner">The <see cref="IMemoryOwner{T}"/> used to construct the instance.</param>
        public OwnedMemory(IMemoryOwner<T> memoryOwner) : this(memoryOwner.Memory, memoryOwner) { }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Memory"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Returns a <see cref="SafeOwnedMemory{T}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeOwnedMemory{T}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public SafeOwnedMemory<T> AsSafe() => new(this);
        /// <summary>
        /// Creates an <see cref="OwnedSpan{T}"/> that wraps the <see cref="Span{T}"/> of the <see cref="Memory{T}"/> associated
        /// with the current instance and the owner of the current instanc.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="OwnedSpan{T}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public OwnedSpan<T> AsOwnedSpan() => new(Memory.Span, Owner);

        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T}"/> instance without an owner from the given <see cref="Memory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="OwnedMemory{T}"/> instance.</returns>
        public static OwnedMemory<T, IDisposable> Unowned(Memory<T> memory) => new(memory, null);
        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T}"/> instance from the given <see cref="Memory{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <typeparam name="O">The type of the owner of the <see cref="Memory{T}"/> instance.</typeparam>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="memory"/>.</param>
        /// <returns>The constructed <see cref="OwnedMemory{T}"/> instance.</returns>
        public static OwnedMemory<T, O> Owned<O>(Memory<T> memory, O owner) where O : IDisposable => new(memory, owner);
        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T}"/> instance from the given <see cref="IMemoryOwner{T}"/>.
        /// The <see cref="Memory{T}"/> of the constructed instance will be from the given <see cref="IMemoryOwner{T}"/> and
        /// the owner will be the <see cref="IMemoryOwner{T}"/> itself.
        /// </summary>
        /// <typeparam name="O">The subtype of the <see cref="IMemoryOwner{T}"/> instance.</typeparam>
        /// <param name="memoryOwner">The <see cref="IMemoryOwner{T}"/> used to construct the instance.</param>
        /// <returns>The constructed <see cref="OwnedMemory{T}"/> instance.</returns>
        public static OwnedMemory<T, O> Owned<O>(O memoryOwner) where O : IMemoryOwner<T> => new(memoryOwner.Memory, memoryOwner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) =>
            other is OwnedMemory<T> ownedMemory && Equals(ownedMemory);

        /// <inheritdoc/>
        public bool Equals(OwnedMemory<T> other) =>
            Memory.Equals(other.Memory) && Owner == other.Owner;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"OwnedMemory<{typeof(T)}>[Memory={Memory.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="OwnedMemory{T}"/> instance.
        /// </summary>
        public static OwnedMemory<T> Empty => default;

        public static implicit operator OwnedMemory<T>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T> ownedMemory) => ownedMemory.Memory;
        public static implicit operator OwnedMemory<T, IDisposable>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T>(OwnedMemory<T> ownedMemory) {
#if NETSTANDARD2_1
            return new(ownedMemory.Memory, ownedMemory.Owner);
#else
            return Unsafe.As<OwnedMemory<T>, ReadOnlyOwnedMemory<T>>(ref ownedMemory);
#endif
        }

        public static explicit operator OwnedSpan<T>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);

        public static bool operator ==(OwnedMemory<T> left, OwnedMemory<T> right) => left.Equals(right);
        public static bool operator !=(OwnedMemory<T> left, OwnedMemory<T> right) => !(left == right);
    }

    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> and
    /// optionally an <typeparamref name="O"/> that represents its owner and which frees the
    /// <see cref="Memory{T}"/> on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public readonly struct OwnedMemory<T, O> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<OwnedMemory<T, O>>
        where O : IDisposable {
        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly Memory<T> Memory { get; }
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public readonly O? Owner { get; }
        /// <summary>
        /// The read-only owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T, O}"/> instance with the given <see cref="Memory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Memory{T}"/> instance.</param>
        public OwnedMemory(Memory<T> memory, O? owner = default) =>
            (Memory, Owner) = (memory, owner);

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Memory"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Returns a <see cref="SafeOwnedMemory{T, O}"/> instance wrapping the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeOwnedMemory{T, O}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public SafeOwnedMemory<T, O> AsSafe() => new(this);
        /// <summary>
        /// Creates an <see cref="OwnedSpan{T, O}"/> that wraps the <see cref="Span{T}"/> of the <see cref="Memory{T}"/> associated
        /// with the current instance and the owner of the current instanc.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="OwnedSpan{T, O}"/> instance wraps the same data as the current instance only the
        /// returned one should be used and disposed.
        /// </remarks>
        public OwnedSpan<T, O> AsOwnedSpan() => new(Memory.Span, Owner);

        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T, O}"/> instance without an owner from the given <see cref="Memory{T}"/>.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="OwnedMemory{T, O}"/> instance.</returns>
        public static OwnedMemory<T, O> Unowned(Memory<T> memory) => new(memory, default);
        /// <summary>
        /// Constructs a new <see cref="OwnedMemory{T, O}"/> instance from the given <see cref="Memory{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="memory"/>.</param>
        /// <returns>The constructed <see cref="OwnedMemory{T, O}"/> instance.</returns>
        public static OwnedMemory<T, O> Owned(Memory<T> memory, O owner) => new(memory, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) =>
            other is OwnedMemory<T, O> ownedMemory && Equals(ownedMemory);

        /// <inheritdoc/>
        public bool Equals(OwnedMemory<T, O> other) =>
            Memory.Equals(other.Memory) && Owner?.Equals(other.Owner) == true;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"OwnedMemory<{typeof(T)}, {typeof(O)}>[Memory={Memory.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="OwnedMemory{T, O}"/> instance.
        /// </summary>
        public static OwnedMemory<T, O> Empty => default;

        public static implicit operator OwnedMemory<T, O>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T, O> ownedMemory) => ownedMemory.Memory;
        public static implicit operator OwnedMemory<T>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) {
#if NETSTANDARD2_1
            return new(ownedMemory.Memory, ownedMemory.Owner);
#else
            return Unsafe.As<OwnedMemory<T, O>, ReadOnlyOwnedMemory<T, O>>(ref ownedMemory);
#endif
        }

        public static explicit operator OwnedSpan<T, O>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);

        public static bool operator ==(OwnedMemory<T, O> left, OwnedMemory<T, O> right) => left.Equals(right);
        public static bool operator !=(OwnedMemory<T, O> left, OwnedMemory<T, O> right) => !(left == right);
    }
}
