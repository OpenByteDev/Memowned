using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Memowned {
    /// <summary>
    /// An struct that holds a <see cref="ReadOnlySpan{T}"/> and optionally an <see cref="IDisposable"/>
    /// that represents its owner and which frees the underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    public readonly ref struct ReadOnlyOwnedSpan<T> {
        /// <summary>
        /// The owned span belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly ReadOnlySpan<T> Span { get; }
        /// <summary>
        /// The owner of <see cref="Span"/> which manages its underlying ressources.
        /// </summary>
        public readonly IDisposable? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T}"/> instance with the given <see cref="ReadOnlySpan{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlySpan{T}"/> instance.</param>
        public ReadOnlyOwnedSpan(ReadOnlySpan<T> span, IDisposable? owner = default) {
            Span = span;
            Owner = owner;
        }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Span"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Creates a <see cref="OwnedSpan{T}"/> from the current instace.
        /// This may produce unexpected results if the underlying span is not writeable.
        /// </summary>
        public OwnedSpan<T> DangerousAsOwnedSpan() => new(MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(Span), Span.Length), Owner);

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T}"/> instance without an owner from the given <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedSpan{T}"/> instance.</returns>
        public static ReadOnlyOwnedSpan<T, IDisposable> Unowned(ReadOnlySpan<T> span) => new(span, null);
        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T}"/> instance from the given <see cref="ReadOnlySpan{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <typeparam name="O">The type of the owner of the <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="span"/>.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedSpan{T}"/> instance.</returns>
        public static ReadOnlyOwnedSpan<T, O> Owned<O>(ReadOnlySpan<T> span, O owner) where O : IDisposable => new(span, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => false;

        /// <inheritdoc/>
        public bool Equals(ReadOnlyOwnedSpan<T> other) =>
            Span == other.Span && Owner?.Equals(other.Owner) == true;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"ReadOnlyOwnedSpan<{typeof(T)}>[Span={Span.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="ReadOnlyOwnedSpan{T}"/> instance.
        /// </summary>
        public static ReadOnlyOwnedSpan<T> Empty => default;

        public static implicit operator ReadOnlyOwnedSpan<T>(ReadOnlySpan<T> span) => new(span);
        public static implicit operator ReadOnlySpan<T>(ReadOnlyOwnedSpan<T> ownedSpan) => ownedSpan.Span;
        public static implicit operator ReadOnlyOwnedSpan<T, IDisposable>(ReadOnlyOwnedSpan<T> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(ReadOnlyOwnedSpan<T> left, ReadOnlyOwnedSpan<T> right) => left.Equals(right);
        public static bool operator !=(ReadOnlyOwnedSpan<T> left, ReadOnlyOwnedSpan<T> right) => !(left == right);
    }

    /// <summary>
    /// An struct that holds a <see cref="ReadOnlySpan{T}"/> and optionally an <typeparamref name="O"/>
    /// that represents its owner and which frees the underlying memory on disposal.
    /// </summary>
    /// <typeparam name="T">The type of items stored.</typeparam>
    /// <typeparam name="O">The type of the owner.</typeparam>
    public readonly ref struct ReadOnlyOwnedSpan<T, O>
        where O : IDisposable {
        /// <summary>
        /// The owned span belonging to <see cref="Owner"/>.
        /// </summary>
        public readonly ReadOnlySpan<T> Span { get; }
        /// <summary>
        /// The owner of <see cref="Span"/> which manages its underlying ressources.
        /// </summary>
        public readonly O? Owner { get; }

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T, O}"/> instance with the given <see cref="ReadOnlySpan{T}"/> which
        /// is optionally owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The optional owner of the given <see cref="ReadOnlySpan{T}"/> instance.</param>
        public ReadOnlyOwnedSpan(ReadOnlySpan<T> span, O? owner = default) {
            Span = span;
            Owner = owner;
        }

        /// <summary>
        /// Disposes the underlying ressources associated with <see cref="Span"/> if this instace is owned.
        /// </summary>
        public void Dispose() => Owner?.Dispose();

        /// <summary>
        /// Creates a <see cref="OwnedSpan{T, O}"/> from the current instace.
        /// This may produce unexpected results if the underlying span is not writeable.
        /// </summary>
        public OwnedSpan<T, O> DangerousAsOwnedSpan() => new(MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(Span), Span.Length), Owner);

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T, O}"/> instance without an owner from the given <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedSpan{T, O}"/> instance.</returns>
        public static ReadOnlyOwnedSpan<T, O> Unowned(ReadOnlySpan<T> span) => new(span, default);
        /// <summary>
        /// Constructs a new <see cref="ReadOnlyOwnedSpan{T, O}"/> instance from the given <see cref="ReadOnlySpan{T}"/> which is
        /// owned by the given owner.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to wrap in the created instance.</param>
        /// <param name="owner">The owner of <paramref name="span"/>.</param>
        /// <returns>The constructed <see cref="ReadOnlyOwnedSpan{T, O}"/> instance.</returns>
        public static ReadOnlyOwnedSpan<T, O> Owned(ReadOnlySpan<T> span, O owner) => new(span, owner);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? other) => false;

        /// <inheritdoc/>
        public bool Equals(ReadOnlyOwnedSpan<T, O> other) =>
            Span == other.Span && Owner?.Equals(other.Owner) == true;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() =>
            HashCode.Combine(Span.GetHashCode(), Owner?.GetHashCode());

        /// <inheritdoc/>
        public override string ToString() {
            return $"ReadOnlyOwnedSpan<{typeof(T)}, {typeof(O)}>[Span={Span.ToString()}, Owner={Owner}]";
        }

        /// <summary>
        /// Returns an empty <see cref="ReadOnlyOwnedSpan{T, O}"/> instance.
        /// </summary>
        public static ReadOnlyOwnedSpan<T, O> Empty => default;

        public static implicit operator ReadOnlyOwnedSpan<T, O>(ReadOnlySpan<T> span) => new(span);
        public static implicit operator ReadOnlySpan<T>(ReadOnlyOwnedSpan<T, O> ownedSpan) => ownedSpan.Span;
        public static implicit operator ReadOnlyOwnedSpan<T>(ReadOnlyOwnedSpan<T, O> ownedSpan) => new(ownedSpan.Span, ownedSpan.Owner);

        public static bool operator ==(ReadOnlyOwnedSpan<T, O> left, ReadOnlyOwnedSpan<T, O> right) => left.Equals(right);
        public static bool operator !=(ReadOnlyOwnedSpan<T, O> left, ReadOnlyOwnedSpan<T, O> right) => !(left == right);
    }
}
