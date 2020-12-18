using System;
using System.Runtime.InteropServices;

namespace OwnedMemory {
    public readonly ref struct ReadOnlyOwnedSpan<T> {
        public readonly ReadOnlySpan<T> Span { get; }
        public readonly IDisposable? Owner { get; }

        public ReadOnlyOwnedSpan(ReadOnlySpan<T> span, IDisposable? owner = default) {
            Span = span;
            Owner = owner;
        }

        public void Dispose() => Owner?.Dispose();

        public OwnedSpan<T> DangerousAsOwnedSpan() => new(MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(Span), Span.Length), Owner);

        public static ReadOnlyOwnedSpan<T, IDisposable> Unowned(ReadOnlySpan<T> span) => new(span, null);
        public static ReadOnlyOwnedSpan<T, O> Owned<O>(ReadOnlySpan<T> span, O owner) where O : IDisposable => new(span, owner);

        public static implicit operator ReadOnlyOwnedSpan<T>(ReadOnlySpan<T> span) => new(span);
        public static implicit operator ReadOnlySpan<T>(ReadOnlyOwnedSpan<T> ownedSpan) => ownedSpan.Span;
        public static implicit operator ReadOnlyOwnedSpan<T, IDisposable>(ReadOnlyOwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
    }

    public readonly ref struct ReadOnlyOwnedSpan<T, O>
        where O : IDisposable {
        public readonly ReadOnlySpan<T> Span { get; }
        public readonly O? Owner { get; }

        public ReadOnlyOwnedSpan(ReadOnlySpan<T> span, O? owner = default) {
            Span = span;
            Owner = owner;
        }

        public void Dispose() => Owner?.Dispose();

        public OwnedSpan<T, O> DangerousAsOwnedSpan() => new(MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(Span), Span.Length), Owner);

        public static ReadOnlyOwnedSpan<T, O> Unowned(ReadOnlySpan<T> span) => new(span, default);
        public static ReadOnlyOwnedSpan<T, O> Owned(ReadOnlySpan<T> span, O owner) => new(span, owner);

        public static implicit operator ReadOnlyOwnedSpan<T, O>(ReadOnlySpan<T> span) => new(span);
        public static implicit operator ReadOnlySpan<T>(ReadOnlyOwnedSpan<T, O> ownedSpan) => ownedSpan.Span;
        public static implicit operator ReadOnlyOwnedSpan<T>(ReadOnlyOwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
    }
}
