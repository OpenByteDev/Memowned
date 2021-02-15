using System;

namespace Memowned.Interfaces {
    /// <summary>
    /// Owner of <see cref="ReadOnlyMemory{T}"/> that is responsible for disposing the underlying memory appropriately.
    /// </summary>
    public interface IReadOnlyMemoryOwner<T> : IDisposable {
        /// <summary>
        /// Returns a <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        ReadOnlyMemory<T> Memory { get; }
    }
}
