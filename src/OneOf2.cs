using System.Diagnostics;

namespace Grondo
{
    /// <summary>
    /// A discriminated union that holds exactly one value of type <typeparamref name="T1"/>
    /// or <typeparamref name="T2"/>. Unlike <see cref="Either{TLeft, TRight}"/> no slot is
    /// preferred; both are equal citizens.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <remarks>
    /// Implicit conversions from <typeparamref name="T1"/> and <typeparamref name="T2"/> are
    /// provided. If the generic arguments are instantiated with the same type, those
    /// conversions are ambiguous at call sites; use the explicit <c>FromT0</c>/<c>FromT1</c>
    /// factories in that case.
    /// </remarks>
    [DebuggerDisplay("T{Index}({Value})")]
    public readonly struct OneOf<T1, T2> : IEquatable<OneOf<T1, T2>>
    {
        private readonly T1? _value1;
        private readonly T2? _value2;

        private OneOf(T1 value) { _value1 = value; _value2 = default; Index = 0; }

        private OneOf(T2 value) { _value1 = default; _value2 = value; Index = 1; }

        /// <summary>Gets the zero-based index of the slot that holds the value.</summary>
        public int Index { get; }

        /// <summary>Gets a value indicating whether the instance holds a <typeparamref name="T1"/>.</summary>
        public bool IsT0 => Index == 0;

        /// <summary>Gets a value indicating whether the instance holds a <typeparamref name="T2"/>.</summary>
        public bool IsT1 => Index == 1;

        /// <summary>Gets the underlying value as <see cref="object"/>.</summary>
        public object Value => Index switch
        {
            0 => _value1!,
            1 => _value2!,
            _ => throw new InvalidOperationException("OneOf in invalid state.")
        };

        /// <summary>Gets the <typeparamref name="T1"/> value.</summary>
        /// <exception cref="InvalidOperationException">The instance does not hold a <typeparamref name="T1"/>.</exception>
        public T1 AsT0 => IsT0
            ? _value1!
            : throw new InvalidOperationException($"Cannot access AsT0: OneOf holds T{Index}.");

        /// <summary>Gets the <typeparamref name="T2"/> value.</summary>
        /// <exception cref="InvalidOperationException">The instance does not hold a <typeparamref name="T2"/>.</exception>
        public T2 AsT1 => IsT1
            ? _value2!
            : throw new InvalidOperationException($"Cannot access AsT1: OneOf holds T{Index}.");

        /// <summary>Creates a OneOf holding a <typeparamref name="T1"/>.</summary>
        public static OneOf<T1, T2> FromT0(T1 value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OneOf<T1, T2>(value);
        }

        /// <summary>Creates a OneOf holding a <typeparamref name="T2"/>.</summary>
        public static OneOf<T1, T2> FromT1(T2 value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OneOf<T1, T2>(value);
        }

        /// <summary>Implicitly wraps a <typeparamref name="T1"/> value.</summary>
        public static implicit operator OneOf<T1, T2>(T1 value) => FromT0(value);

        /// <summary>Implicitly wraps a <typeparamref name="T2"/> value.</summary>
        public static implicit operator OneOf<T1, T2>(T2 value) => FromT1(value);

        /// <summary>Pattern matches on the current slot and returns a value.</summary>
        public TResult Match<TResult>(Func<T1, TResult> f0, Func<T2, TResult> f1)
        {
            ArgumentNullException.ThrowIfNull(f0);
            ArgumentNullException.ThrowIfNull(f1);
            return Index switch
            {
                0 => f0(_value1!),
                1 => f1(_value2!),
                _ => throw new InvalidOperationException("OneOf in invalid state.")
            };
        }

        /// <summary>Pattern matches on the current slot and executes a side effect.</summary>
        public void Switch(Action<T1> a0, Action<T2> a1)
        {
            ArgumentNullException.ThrowIfNull(a0);
            ArgumentNullException.ThrowIfNull(a1);
            switch (Index)
            {
                case 0: a0(_value1!); break;
                case 1: a1(_value2!); break;
                default: throw new InvalidOperationException("OneOf in invalid state.");
            }
        }

        /// <summary>Transforms the <typeparamref name="T1"/> slot if present.</summary>
        public OneOf<TResult, T2> MapT0<TResult>(Func<T1, TResult> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsT0 ? OneOf<TResult, T2>.FromT0(mapper(_value1!)) : OneOf<TResult, T2>.FromT1(_value2!);
        }

        /// <summary>Transforms the <typeparamref name="T2"/> slot if present.</summary>
        public OneOf<T1, TResult> MapT1<TResult>(Func<T2, TResult> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            return IsT1 ? OneOf<T1, TResult>.FromT1(mapper(_value2!)) : OneOf<T1, TResult>.FromT0(_value1!);
        }

        /// <summary>Indicates whether the current instance is equal to another instance of the same type.</summary>
        public bool Equals(OneOf<T1, T2> other) => Index == other.Index && Index switch
        {
            0 => EqualityComparer<T1>.Default.Equals(_value1!, other._value1!),
            1 => EqualityComparer<T2>.Default.Equals(_value2!, other._value2!),
            _ => false
        };

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is OneOf<T1, T2> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => Index switch
        {
            0 => HashCode.Combine(Index, _value1),
            1 => HashCode.Combine(Index, _value2),
            _ => 0
        };

        /// <summary>Determines whether two <see cref="OneOf{T1, T2}"/> instances are equal.</summary>
        public static bool operator ==(OneOf<T1, T2> left, OneOf<T1, T2> right) => left.Equals(right);

        /// <summary>Determines whether two <see cref="OneOf{T1, T2}"/> instances are not equal.</summary>
        public static bool operator !=(OneOf<T1, T2> left, OneOf<T1, T2> right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"T{Index}({Value})";
    }
}
