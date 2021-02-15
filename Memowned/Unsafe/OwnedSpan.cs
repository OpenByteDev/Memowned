using System;
using System.ComponentModel;

namespace Memowned {
    /// <summary>
    /// An struct that holds a <see cref="Span{T}"/> and optionally an <see cref="IDisposable"/>
    /// that represents its owner and which frees the underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly ref struct OwnedSpan<T> {
        /// <summary>
        /// The owned span belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly Span<T> Span { get; }
        /// <summary>
        /// The owner of <see cref="Span"/> which manages its underlying ressources.
        /// </summary>
        public readonly IDisposable? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T}"/> instance with the given <see cref="Span{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Span{T}"/> instance.</param>
        public OwnedSpan(Span<T> span, IDisposable? owner = default) {
            Span = span;
            Owner = owner;
        }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Span"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T}"/> instance without an owner from the given <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="OwnedSpan{T}"/> instance.</returns>
        public static OwnedSpan<T, IDisposable> Unowned(Span<T> span) => new(span, null);
        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T}"/> instance from the given <see cref="Span{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <typeparam name="O">The type of the owner of the <see cref="Span{T}"/> instance.</typeparam>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="span"/>.</param>
        /// <returns>The constructed <see cref="OwnedSpan{T}"/> instance.</returns>
        public static OwnedSpan<T, O> Owned<O>(Span<T> span, O owner) where O : IDisposable => new(span, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => false;

        /// <inheritdoc/>
        public bool Equals(OwnedSpan<T> other) =>
            Span == other.Span && Owner == other.Owner;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"OwnedSpan<{typeof(T)}>[Span={Span.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="OwnedSpan{T}"/> instance.
        /// </summary>
        public static OwnedSpan<T> Empty => default;

        public static implicit operator OwnedSpan<T>(Span<T> span) => new(span);
        public static implicit operator Span<T>(OwnedSpan<T> ownedSpan) => ownedSpan.Span;
        public static implicit operator OwnedSpan<T, IDisposable>(OwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T>(OwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(OwnedSpan<T> left, OwnedSpan<T> right) => left.Equals(right);
        public static bool operator !=(OwnedSpan<T> left, OwnedSpan<T> right) => !(left == right);
    }

    /// <summary>
    /// An struct that holds a <see cref="Span{T}"/> and optionally an <typeparamref name="O"/>
    /// that represents its owner and which frees the underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public readonly ref struct OwnedSpan<T, O>
        where O : IDisposable {
        /// <summary>
        /// The owned span belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly Span<T> Span { get; }
        /// <summary>
        /// The owner of <see cref="Span"/> which manages its underlying ressources.
        /// </summary>
        public readonly O? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T, O}"/> instance with the given <see cref="Span{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="Span{T}"/> instance.</param>
        public OwnedSpan(Span<T> span, O? owner = default) {
            Span = span;
            Owner = owner;
        }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Span"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T, O}"/> instance without an owner from the given <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="OwnedSpan{T, O}"/> instance.</returns>
        public static OwnedSpan<T, O> Unowned(Span<T> span) => new(span, default);
        /// <summary>
        /// Constructs a new <see cref="OwnedSpan{T, O}"/> instance from the given <see cref="Span{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="span"/>.</param>
        /// <returns>The constructed <see cref="OwnedSpan{T, O}"/> instance.</returns>
        public static OwnedSpan<T, O> Owned(Span<T> span, O owner) => new(span, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => false;

        /// <inheritdoc/>
        public bool Equals(OwnedSpan<T, O> other) =>
            Span == other.Span && Owner?.Equals(other.Owner) == true;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"OwnedSpan<{typeof(T)}, {typeof(O)}>[Span={Span.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="OwnedSpan{T, O}"/> instance.
        /// </summary>
        public static OwnedSpan<T, O> Empty => default;

        public static implicit operator OwnedSpan<T, O>(Span<T> span) => new(span);
        public static implicit operator Span<T>(OwnedSpan<T, O> ownedSpan) => ownedSpan.Span;
        public static implicit operator OwnedSpan<T>(OwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);
        public static implicit operator ReadOnlyOwnedSpan<T, O>(OwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(OwnedSpan<T, O> left, OwnedSpan<T, O> right) => left.Equals(right);
        public static bool operator !=(OwnedSpan<T, O> left, OwnedSpan<T, O> right) => !(left == right);
    }
}
