using System;
using System.Buffers;

namespace OwnedMemory {
    public class SafeOwnedMemory<T> : SafeOwnedMemory<T, IDisposable> {
        public SafeOwnedMemory(Memory<T> memory, IDisposable? owner = default) : base(new(memory, owner)) { }
        public SafeOwnedMemory(OwnedMemory<T> ownedMemory) : base(ownedMemory) { }
    }

    public class SafeOwnedMemory<T, O> : SafeDisposableStructWrapper<OwnedMemory<T, O>>, IMemoryOwner<T>
        where O : IDisposable {
        public SafeOwnedMemory(Memory<T> memory, O? owner = default) : this(new(memory, owner)) { }
        public SafeOwnedMemory(OwnedMemory<T, O> ownedMemory) : base(ownedMemory) { }

        public O? Owner => Value.Owner;
        public Memory<T> Memory => Value.Memory;

        public OwnedMemory<T, O> AsUnsafe() => MoveValue();

        public static implicit operator SafeOwnedMemory<T, O>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(SafeOwnedMemory<T, O> memory) => memory.Memory;
    }
}
