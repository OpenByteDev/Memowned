using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Memowned {
    /// <summary>
    /// A wrapper for structs implmenting <see cref="IDisposable"/> that gurantuess that it is disposed.
    /// </summary>
    /// <typeparam name="T">The type of struct that is wrapped by this type.</typeparam>
    public abstract class SafeDisposableStructWrapper<T> : IDisposable
        where T : struct, IDisposable {
        private readonly T _value;
        private int _disposed;

        /// <summary>
        /// The instance of the wrapped struct.
        /// </summary>
        protected T Value {
            get {
                ThrowIfDisposed();
                return _value;
            }
        }
        /// <summary>
        /// Is the current instance disposed.
        /// </summary>
        public bool IsDisposed => _disposed == 1;

        /// <summary>
        /// Construct a new <see cref="SafeDisposableStructWrapper{T}"/> instance wrapping the given <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value that is wrapped by the constructed instance.</param>
        protected SafeDisposableStructWrapper(T value) {
            _value = value;
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current instance is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">if the current instance is disposed.</exception>
        protected void ThrowIfDisposed() {
            if (IsDisposed)
                ThrowObjectDisposedException();

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Marks the current instance as disposed.
        /// This operation is atomic.
        /// </summary>
        /// <returns>Was the current instance already disposed before this call.</returns>
        protected bool MarkDisposed() => Interlocked.Exchange(ref _disposed, 1) == 0;

        /// <summary>
        /// Moves the wrapped <typeparamref name="T"/> out of the current instance and marks this as disposed.
        /// </summary>
        /// <returns>The instance wrapped in the current instance.</returns>
        protected T MoveValue() {
            MarkDisposed();
            Dispose();
            return _value;
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing) {
            if (MarkDisposed()) {
                _value.Dispose();
            }
        }

        ~SafeDisposableStructWrapper() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}