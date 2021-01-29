﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Memowned {
    public abstract class SafeDisposableStructWrapper<T> : IDisposable
        where T : struct, IDisposable {
        private readonly T _value;
        private int _disposed;

        protected T Value {
            get {
                ThrowIfDisposed();
                return _value;
            }
        }
        public bool IsDisposed => _disposed == 1;

        protected SafeDisposableStructWrapper(T value) {
            _value = value;
        }

        protected void ThrowIfDisposed() {
            if (IsDisposed)
                ThrowObjectDisposedException();

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().Name);
        }

        protected bool MarkDisposed() => Interlocked.Exchange(ref _disposed, 1) == 0;

        protected T MoveValue() {
            MarkDisposed();
            GC.SuppressFinalize(this);
            return _value;
        }

        protected virtual void Dispose(bool disposing) {
            if (MarkDisposed()) {
                _value.Dispose();
            }
        }

        ~SafeDisposableStructWrapper() => Dispose(disposing: false);

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}