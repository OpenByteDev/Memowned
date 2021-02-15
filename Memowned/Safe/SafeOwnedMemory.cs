using System;
using System.Buffers;
using Memowned.Interfaces;

namespace Memowned {
    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> and
    /// optionally an <see cref="IDisposable"/> that represents its owner and which frees the
    /// underlying memory on disposal.
    /// This type guarantees that the owner is disposed.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public class SafeOwnedMemory<T> : SafeOwnedMemory<T, IDisposable> {
        /// <summary>
        /// Constructs a new <see cref="SafeOwnedMemory{T}"/> instance with the given <see cref="Memory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Memory{T}"/> instance.</param>
        public SafeOwnedMemory(Memory<T> memory, IDisposable? owner = default) : this(new(memory, owner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeOwnedMemory{T}"/> instance from the given <see cref="IMemoryOwner{T}"/>.
        /// The <see cref="Memory{T}"/> of the constructed instance will be from the given <see cref="IMemoryOwner{T}"/> and
        /// the owner will be the <see cref="IMemoryOwner{T}"/> itself.
        /// </summary>
        /// <param name="memoryOwner">The <see cref="IMemoryOwner{T}"/> used to construct the instance.</param>
        public SafeOwnedMemory(IMemoryOwner<T> memoryOwner) : this(new(memoryOwner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeOwnedMemory{T}"/> instance wrapping the given <see cref="OwnedMemory{T}"/>.
        /// </summary>
        /// <param name="ownedMemory">The <see cref="OwnedMemory{T}"/> instance to wrap.</param>
        public SafeOwnedMemory(OwnedMemory<T> ownedMemory) : base(ownedMemory) { }

        public static implicit operator SafeOwnedMemory<T>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(SafeOwnedMemory<T> memory) => memory.Memory;

        public static explicit operator SafeOwnedMemory<T>(OwnedMemory<T> ownedMemory) => new(ownedMemory);
    }

    /// <summary>
    /// An <see cref="IMemoryOwner{T}"/> implementation that holds a <see cref="Memory{T}"/> and
    /// optionally an <typeparamref name="O"/> that represents its owner and which frees the
    /// underlying memory on disposal.
    /// This type guarantees that the owner is disposed.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public class SafeOwnedMemory<T, O> : SafeDisposableStructWrapper<OwnedMemory<T, O>>, IMemoryOwner<T>, IReadOnlyMemoryOwner<T>
        where O : IDisposable {
        /// <summary>
        /// Constructs a new <see cref="SafeOwnedMemory{T, O}"/> instance with the given <see cref="Memory{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="memory">The <see cref="Memory{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Memory{T}"/> instance.</param>
        public SafeOwnedMemory(Memory<T> memory, O? owner = default) : this(new(memory, owner)) { }
        /// <summary>
        /// Constructs a new <see cref="SafeOwnedMemory{T, O}"/> instance wrapping the given <see cref="OwnedMemory{T, O}"/>.
        /// </summary>
        /// <param name="ownedMemory">The <see cref="OwnedMemory{T, O}"/> instance to wrap.</param>
        public SafeOwnedMemory(OwnedMemory<T, O> ownedMemory) : base(ownedMemory) { }

        /// <summary>
        /// The owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        public Memory<T> Memory => Value.Memory;
        /// <summary>
        /// The owner of <see cref="Memory"/> which manages its underlying ressources.
        /// </summary>
        public O? Owner => Value.Owner;
        /// <summary>
        /// The read-only owned memory belonging to <see cref="Owner"/>.
        /// </summary>
        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        /// <summary>
        /// Returns the <see cref="OwnedMemory{T}"/> instance wrapped by the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="OwnedMemory{T}"/> instance wraps the same data as the current instance only the returned one should
        /// be used. The current instance will be disposed after the call.
        /// </remarks>
        public OwnedMemory<T, O> AsUnsafe() => MoveValue();

        /// <summary>
        /// Returns a <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance wrapping the same memory as the current instance.
        /// </summary>
        /// <remarks>
        /// As the returned <see cref="SafeReadOnlyOwnedMemory{T, O}"/> instance wraps the same data as the current instance only the returned
        /// one should be used. The current instance will be disposed after the call.
        /// </remarks>
        public SafeReadOnlyOwnedMemory<T, O> AsReadOnly() => new(MoveValue());

        public static implicit operator SafeOwnedMemory<T, O>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(SafeOwnedMemory<T, O> memory) => memory.Memory;

        public static explicit operator SafeOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory);
    }
}
