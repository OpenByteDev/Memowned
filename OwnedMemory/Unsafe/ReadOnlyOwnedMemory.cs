using System;
using System.Runtime.InteropServices;
using OwnedMemory.Interfaces;

namespace OwnedMemory {
    public readonly struct ReadOnlyOwnedMemory<T> : IReadOnlyMemoryOwner<T> {
        public readonly ReadOnlyMemory<T> Memory { get; }
        public readonly IDisposable? Owner { get; }

        public ReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, IDisposable? owner = default) =>
            (Memory, Owner) = (memory, owner);

        public void Dispose() => Owner?.Dispose();

        public SafeReadOnlyOwnedMemory<T> AsSafe() => new(this);
        public ReadOnlyOwnedSpan<T> AsOwnedSpan() => new(Memory.Span, Owner);
        public OwnedMemory<T> DangerousAsOwnedMemory() => new(MemoryMarshal.AsMemory(Memory), Owner);

        public static ReadOnlyOwnedMemory<T, IDisposable> Unowned(ReadOnlyMemory<T> memory) => new(memory, null);
        public static ReadOnlyOwnedMemory<T, O> Owned<O>(ReadOnlyMemory<T> memory, O owner) where O : IDisposable => new(memory, owner);

        public static implicit operator ReadOnlyOwnedMemory<T>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(ReadOnlyOwnedMemory<T> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeReadOnlyOwnedMemory<T>(ReadOnlyOwnedMemory<T> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator ReadOnlyOwnedMemory<T, IDisposable>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
    }

    public readonly struct ReadOnlyOwnedMemory<T, O> : IReadOnlyMemoryOwner<T>
        where O : IDisposable {
        public readonly ReadOnlyMemory<T> Memory { get; }
        public readonly O? Owner { get; }

        public ReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, O? owner = default) =>
            (Memory, Owner) = (memory, owner);

        public void Dispose() => Owner?.Dispose();

        public SafeReadOnlyOwnedMemory<T, O> AsSafe() => new(this);
        public ReadOnlyOwnedSpan<T, O> AsOwnedSpan() => new(Memory.Span, Owner);
        public OwnedMemory<T, O> DangerousAsOwnedMemory() => new(MemoryMarshal.AsMemory(Memory), Owner);

        public static ReadOnlyOwnedMemory<T, O> Unowned(ReadOnlyMemory<T> memory) => new(memory, default);
        public static ReadOnlyOwnedMemory<T, O> Owned(ReadOnlyMemory<T> memory, O owner) => new(memory, owner);

        public static implicit operator ReadOnlyOwnedMemory<T, O>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator ReadOnlyMemory<T>(ReadOnlyOwnedMemory<T, O> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeReadOnlyOwnedMemory<T, O>(ReadOnlyOwnedMemory<T, O> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator ReadOnlyOwnedMemory<T>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T, O>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
    }
}
