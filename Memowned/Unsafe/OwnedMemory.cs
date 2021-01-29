using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Memowned.Interfaces;

namespace Memowned {
    public readonly struct OwnedMemory<T> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<OwnedMemory<T>> {
        public readonly Memory<T> Memory { get; }
        public readonly IDisposable? Owner { get; }

        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        public OwnedMemory(Memory<T> memory, IDisposable? owner = default) =>
            (Memory, Owner) = (memory, owner);
        public OwnedMemory(IMemoryOwner<T> memoryOwner) : this(memoryOwner.Memory, memoryOwner) { }

        public void Dispose() => Owner?.Dispose();

        public SafeOwnedMemory<T> AsSafe() => new(this);
        public OwnedSpan<T> AsOwnedSpan() => new(Memory.Span, Owner);

        public static OwnedMemory<T, IDisposable> Unowned(Memory<T> memory) => new(memory, null);
        public static ReadOnlyOwnedMemory<T, IDisposable> Unowned(ReadOnlyMemory<T> memory) => new(memory, null);
        public static OwnedMemory<T, O> Owned<O>(Memory<T> memory, O owner) where O : IDisposable => new(memory, owner);
        public static ReadOnlyOwnedMemory<T, O> Owned<O>(ReadOnlyMemory<T> memory, O owner) where O : IDisposable => new(memory, owner);
        public static OwnedMemory<T, O> Owned<O>(O memoryOwner) where O : IMemoryOwner<T> => new(memoryOwner.Memory, memoryOwner);

        public static OwnedMemory<T> Empty => default;

        public static implicit operator OwnedMemory<T>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeOwnedMemory<T>(OwnedMemory<T> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator OwnedMemory<T, IDisposable>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator OwnedSpan<T>(OwnedMemory<T> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T>(OwnedMemory<T> ownedMemory) => Unsafe.As<OwnedMemory<T>, ReadOnlyOwnedMemory<T>>(ref ownedMemory);

        public static bool operator ==(OwnedMemory<T> left, OwnedMemory<T> right) => left.Equals(right);
        public static bool operator !=(OwnedMemory<T> left, OwnedMemory<T> right) => !(left == right);

        public override bool Equals(object? other) =>
            other is OwnedMemory<T> ownedMemory && Equals(ownedMemory);
        public bool Equals(OwnedMemory<T> other) =>
            Memory.Equals(other.Memory) && Owner == other.Owner;

        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());
    }

    public readonly struct OwnedMemory<T, O> : IMemoryOwner<T>, IReadOnlyMemoryOwner<T>, IEquatable<OwnedMemory<T, O>>
        where O : IDisposable {
        public readonly Memory<T> Memory { get; }
        public readonly O? Owner { get; }

        ReadOnlyMemory<T> IReadOnlyMemoryOwner<T>.Memory => Memory;

        public OwnedMemory(Memory<T> memory, O? owner = default) =>
            (Memory, Owner) = (memory, owner);

        public void Dispose() => Owner?.Dispose();

        public SafeOwnedMemory<T, O> AsSafe() => new(this);
        public OwnedSpan<T, O> AsOwnedSpan() => new(Memory.Span, Owner);

        public static OwnedMemory<T, O> Unowned(Memory<T> memory) => new(memory, default);
        public static ReadOnlyOwnedMemory<T, O> Unowned(ReadOnlyMemory<T> memory) => new(memory, default);
        public static OwnedMemory<T, O> Owned(Memory<T> memory, O owner) => new(memory, owner);
        public static ReadOnlyOwnedMemory<T, O> Owned(ReadOnlyMemory<T> memory, O owner) => new(memory, owner);

        public static OwnedMemory<T, O> Empty => default;

        public static implicit operator OwnedMemory<T, O>(Memory<T> memory) => new(memory);
        public static implicit operator Memory<T>(OwnedMemory<T, O> ownedMemory) => ownedMemory.Memory;
        public static implicit operator SafeOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) => ownedMemory.AsSafe();
        public static implicit operator OwnedMemory<T>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory, ownedMemory.Owner);
        public static implicit operator OwnedSpan<T, O>(OwnedMemory<T, O> ownedMemory) => new(ownedMemory.Memory.Span, ownedMemory.Owner);
        public static implicit operator ReadOnlyOwnedMemory<T, O>(OwnedMemory<T, O> ownedMemory) => Unsafe.As<OwnedMemory<T, O>, ReadOnlyOwnedMemory<T, O>>(ref ownedMemory);

        public static bool operator ==(OwnedMemory<T, O> left, OwnedMemory<T, O> right) => left.Equals(right);
        public static bool operator !=(OwnedMemory<T, O> left, OwnedMemory<T, O> right) => !(left == right);

        public override bool Equals(object? other) =>
            other is OwnedMemory<T, O> ownedMemory && Equals(ownedMemory);
        public bool Equals(OwnedMemory<T, O> other) =>
            Memory.Equals(other.Memory) && Owner?.Equals(other.Owner) == true;

        public override int GetHashCode() =>
            HashCode.Combine(Memory.GetHashCode(), Owner?.GetHashCode());
    }
}
