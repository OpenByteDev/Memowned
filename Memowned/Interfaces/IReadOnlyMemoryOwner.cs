using System;

namespace Memowned.Interfaces {
    public interface IReadOnlyMemoryOwner<T> : IDisposable {
        ReadOnlyMemory<T> Memory { get; }
    }
}
