using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace OwnedMemory {
    public readonly struct OwnedMemory<T> : IMemoryOwner<T> {
        public readonly Memory<T> Memory { get; }
        public readonly IDisposable? Owner { get; }

        public OwnedMemory(Memory<T> memory, IDisposable? owner = default) =>
            (Memory, Owner) = (memory, owner);

        public void Dispose() => Owner?.Dispose();

        public SafeOwnedMemory<T> AsSafe() => new(this);
        public OwnedSpan<T> AsOwnedSpan() => new(Memory.Span, Owner);

        public static OwnedMemory<T, IDisposable> Unowned(Memory<T> memory) => new(memory, null);
        public static ReadOnlyOwnedMemory<T, IDisposable> Unowned(ReadOnlyMemory<T> memory) => new(memory, null);
        public static OwnedMemory<T, O> Owned<O>(Memory<T> memory, O owner) where O : IDisposable => new(memory, owner);
        public static ReadOnlyOwnedMemory<T, O> Owned<O>(ReadOnlyMemory<T> memory, O owner) where O : IDisposable => new(memory, owner);

        public static implicit operator OwnedMemory<T>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeOwnedMemory<T>(OwnedMemory<T> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator OwnedMemory<T, IDisposable>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator OwnedSpan<T>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T>(OwnedMemory<T> ownedMemory) => Unsafe.As<OwnedMemory<T>, ReadOnlyOwnedMemory<T>>(ref ownedMemory);
    }

    public readonly struct OwnedMemory<T, O> : IMemoryOwner<T>
        where O : IDisposable {
        public readonly Memory<T> Memory { get; }
        public readonly O? Owner { get; }

        public OwnedMemory(Memory<T> memory, O? owner = default) =>
            (Memory, Owner) = (memory, owner);

        public void Dispose() => Owner?.Dispose();

        public SafeOwnedMemory<T, O> AsSafe() => new(this);
        public OwnedSpan<T, O> AsOwnedSpan() => new(Memory.Span, Owner);

        public static OwnedMemory<T, O> Unowned(Memory<T> memory) => new(memory, default);
        public static ReadOnlyOwnedMemory<T, O> Unowned(ReadOnlyMemory<T> memory) => new(memory, default);
        public static OwnedMemory<T, O> Owned(Memory<T> memory, O owner) => new(memory, owner);
        public static ReadOnlyOwnedMemory<T, O> Owned(ReadOnlyMemory<T> memory, O owner) => new(memory, owner);

        public static implicit operator OwnedMemory<T, O>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T, O> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator OwnedMemory<T>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator OwnedSpan<T, O>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) => Unsafe.As<OwnedMemory<T, O>, ReadOnlyOwnedMemory<T, O>>(ref ownedMemory);
    }
}
