using System;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IReadOnlyMemoryOwner{T}"/> implementation that holds a <see cref="ReadOnlyMemory{T}"/> and
    /// optionally an <see cref="IDisposable"/> that represents its owner and which frees the
    /// underlying memory on disposal.
    /// This type guarantees that the owner is disposed.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public class SafeReadOnlyOwnedMemory<T> : SafeReadOnlyOwnedMemory<T, IDisposable> {
        /// <summary>
        /// Constructs a new <see cref="SafeReadOnlyOwnedMemory{T}"/> instance with the given <see cref="ReadOnlyMemory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlyMemory{T}"/> instance.</param>
        public SafeReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, IDisposable? owner = default) : this(new(memory, owner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeReadOnlyOwnedMemory{T}"/> instance from the given <see cref="IReadOnlyMemoryOwner{T}"/>.
        /// The <see cref="Memory{T}"/> of the constructed instance will be from the given <see cref="IReadOnlyMemoryOwner{T}"/> and
        /// the owner will be the <see cref="IReadOnlyMemoryOwner{T}"/> itself.
        /// </summary>
        /// <param name="memoryOwner">The <see cref="IReadOnlyMemoryOwner{T}"/> used to construct the instance.</param>
        public SafeReadOnlyOwnedMemory(IReadOnlyMemoryOwner<T> memoryOwner) : this(new(memoryOwner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeReadOnlyOwnedMemory{T}"/> instance wrapping the given <see cref="ReadOnlyOwnedMemory{T}"/>.
        /// </summary>
        /// <param name="ownedMemory">The <see cref="ReadOnlyOwnedMemory{T}"/> instance to wrap.</param>
        public SafeReadOnlyOwnedMemory(ReadOnlyOwnedMemory<T> ownedMemory) : base(ownedMemory) { }

        public static implicit operator SafeReadOnlyOwnedMemory<T>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(SafeReadOnlyOwnedMemory<T> memory) => memory.Memory;

        public static explicit operator SafeReadOnlyOwnedMemory<T>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory);
    }

    /// <summary>
    /// An <see cref="IReadOnlyMemoryOwner{T}"/> implementation that holds a <see cref="ReadOnlyMemory{T}"/> and
    /// optionally an <typeparamref name="O"/> that represents its owner and which frees the
    /// underlying memory on disposal.
    /// This type guarantees that the owner is disposed.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public class SafeReadOnlyOwnedMemory<T, O> : SafeDisposableStructWrapper<ReadOnlyOwnedMemory<T, O>>, IReadOnlyMemoryOwner<T>
        where O : IDisposable {
        /// <summary>
        /// Constructs a new <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance with the given <see cref="ReadOnlyMemory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlyMemory{T}"/> instance.</param>
        public SafeReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, O? owner = default) : this(new(memory, owner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance wrapping the given <see cref="ReadOnlyOwnedMemory{T, O}"/>.
        /// </summary>
        /// <param name="ownedMemory">The <see cref="ReadOnlyOwnedMemory{T, O}"/> instance to wrap.</param>
        public SafeReadOnlyOwnedMemory(ReadOnlyOwnedMemory<T, O> ownedMemory) : base(ownedMemory) { }

        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public ReadOnlyMemory<T> Memory => Value.Memory;
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public O? Owner => Value.Owner;

        /// <summary>
        /// Returns the <see cref="ReadOnlyOwnedMemory{T}"/> instance wrapped by the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="ReadOnlyOwnedMemory{T}"/> instance wraps the same data as the current instance only the returned one should
        /// be used. The current instance will be disposed after the call.
        /// </remarks>
        public ReadOnlyOwnedMemory<T, O> AsUnsafe() => MoveValue();

        public static implicit operator SafeReadOnlyOwnedMemory<T, O>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(SafeReadOnlyOwnedMemory<T, O> memory) => memory.Memory;

        public static explicit operator SafeReadOnlyOwnedMemory<T, O>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory);
    }
}
