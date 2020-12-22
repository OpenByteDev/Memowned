using System;
using Memowned.Interfaces;

namespace Memowned {
    public class SafeReadOnlyOwnedMemory<T> : SafeReadOnlyOwnedMemory<T, IDisposable> {
        public SafeReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, IDisposable? owner = default) : base(new(memory, owner)) { }
        public SafeReadOnlyOwnedMemory(ReadOnlyOwnedMemory<T> ownedMemory) : base(ownedMemory) { }

        public static implicit operator SafeReadOnlyOwnedMemory<T>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator SafeReadOnlyOwnedMemory<T>(ReadOnlyOwnedMemory<T> ownedMemory) => new(ownedMemory);
        public static implicit operator ReadOnlyMemory<T>(SafeReadOnlyOwnedMemory<T> memory) => memory.Memory;
    }

    public class SafeReadOnlyOwnedMemory<T, O> : SafeDisposableStructWrapper<ReadOnlyOwnedMemory<T, O>>, IReadOnlyMemoryOwner<T>
        where O : IDisposable {
        public SafeReadOnlyOwnedMemory(ReadOnlyMemory<T> memory, O? owner = default) : this(new(memory, owner)) { }
        public SafeReadOnlyOwnedMemory(ReadOnlyOwnedMemory<T, O> ownedMemory) : base(ownedMemory) { }

        public O? Owner => Value.Owner;
        public ReadOnlyMemory<T> Memory => Value.Memory;

        public ReadOnlyOwnedMemory<T, O> AsUnsafe() => MoveValue();

        public static implicit operator SafeReadOnlyOwnedMemory<T, O>(ReadOnlyMemory<T> memory) => new(memory);
        public static implicit operator SafeReadOnlyOwnedMemory<T, O>(ReadOnlyOwnedMemory<T, O> ownedMemory) => new(ownedMemory);
        public static implicit operator ReadOnlyMemory<T>(SafeReadOnlyOwnedMemory<T, O> memory) => memory.Memory;
    }
}
