using System;

namespace Memowned {
    public readonly ref struct OwnedSpan<T> {
        public readonly Span<T> Span { get; }
        public readonly IDisposable? Owner { get; }

        public OwnedSpan(Span<T> span, IDisposable? owner = default) {
            Span = span;
            Owner = owner;
        }

        public void Dispose() => Owner?.Dispose();

        public static OwnedSpan<T, IDisposable> Unowned(Span<T> span) => new(span, null);
        public static ReadOnlyOwnedSpan<T, IDisposable> Unowned(ReadOnlySpan<T> span) => new(span, null);
        public static OwnedSpan<T, O> Owned<O>(Span<T> span, O owner) where O : IDisposable => new(span, owner);
        public static ReadOnlyOwnedSpan<T, O> Owned<O>(ReadOnlySpan<T> span, O owner) where O : IDisposable => new(span, owner);

        public static OwnedSpan<T> Empty => default;

        public static implicit operator OwnedSpan<T>(Span<T> span) => new(span);
        public static implicit operator Span<T>(OwnedSpan<T> ownedSpan) => ownedSpan.Span;
        public static implicit operator OwnedSpan<T, IDisposable>(OwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T>(OwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(OwnedSpan<T> left, OwnedSpan<T> right) => left.Equals(right);
        public static bool operator !=(OwnedSpan<T> left, OwnedSpan<T> right) => !(left == right);

        public override bool Equals(object? other) => false;
        public bool Equals(OwnedSpan<T> other) =>
            Span == other.Span && Owner == other.Owner;

        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());
    }

    public readonly ref struct OwnedSpan<T, O>
        where O : IDisposable {
        public readonly Span<T> Span { get; }
        public readonly O? Owner { get; }

        public OwnedSpan(Span<T> span, O? owner = default) {
            Span = span;
            Owner = owner;
        }

        public void Dispose() => Owner?.Dispose();

        public static OwnedSpan<T, O> Unowned(Span<T> span) => new(span, default);
        public static ReadOnlyOwnedSpan<T, O> Unowned(ReadOnlySpan<T> span) => new(span, default);
        public static OwnedSpan<T, O> Owned(Span<T> span, O owner) => new(span, owner);
        public static ReadOnlyOwnedSpan<T, O> Owned(ReadOnlySpan<T> span, O owner) => new(span, owner);

        public static OwnedSpan<T, O> Empty => default;

        public static implicit operator OwnedSpan<T, O>(Span<T> span) => new(span);
        public static implicit operator Span<T>(OwnedSpan<T, O> ownedSpan) => ownedSpan.Span;
        public static implicit operator OwnedSpan<T>(OwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T, O>(OwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(OwnedSpan<T, O> left, OwnedSpan<T, O> right) => left.Equals(right);
        public static bool operator !=(OwnedSpan<T, O> left, OwnedSpan<T, O> right) => !(left == right);

        public override bool Equals(object? other) => false;
        public bool Equals(OwnedSpan<T, O> other) =>
            Span == other.Span && Owner?.Equals(other.Owner) == true;

        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());
    }
}
