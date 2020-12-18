using System;

namespace OwnedMemory.Interfaces {
    public interface IReadOnlyMemoryOwner<T> : IDisposable {
        ReadOnlyMemory<T> Memory { get; }
    }
}
