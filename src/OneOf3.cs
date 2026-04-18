using System.Diagnostics;

namespace Grondo
{
    /// <summary>
    /// A discriminated union that holds exactly one value out of
    /// <typeparamref name="T1"/>, <typeparamref name="T2"/>, or <typeparamref name="T3"/>.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    [DebuggerDisplay("T{Index}({Value})")]
    public readonly struct OneOf<T1, T2, T3> : IEquatable<OneOf<T1, T2, T3>>
    {
        private readonly T1? _value1;
        private readonly T2? _value2;
        private readonly T3? _value3;

        private OneOf(T1 v) { _value1 = v; _value2 = default; _value3 = default; Index = 0; }

        private OneOf(T2 v) { _value1 = default; _value2 = v; _value3 = default; Index = 1; }

        private OneOf(T3 v) { _value1 = default; _value2 = default; _value3 = v; Index = 2; }

        /// <summary>Gets the zero-based index of the slot that holds the value.</summary>
        public int Index { get; }

        /// <summary>Gets a value indicating whether the instance holds a <typeparamref name="T1"/>.</summary>
        public bool IsT0 => Index == 0;

        /// <summary>Gets a value indicating whether the instance holds a <typeparamref name="T2"/>.</summary>
        public bool IsT1 => Index == 1;

        /// <summary>Gets a value indicating whether the instance holds a <typeparamref name="T3"/>.</summary>
        public bool IsT2 => Index == 2;

        /// <summary>Gets the underlying value as <see cref="object"/>.</summary>
        public object Value => Index switch
        {
            0 => _value1!,
            1 => _value2!,
            2 => _value3!,
            _ => throw new InvalidOperationException("OneOf in invalid state.")
        };

        /// <summary>Gets the <typeparamref name="T1"/> value.</summary>
        public T1 AsT0 => IsT0 ? _value1! : throw new InvalidOperationException($"Cannot access AsT0: OneOf holds T{Index}.");

        /// <summary>Gets the <typeparamref name="T2"/> value.</summary>
        public T2 AsT1 => IsT1 ? _value2! : throw new InvalidOperationException($"Cannot access AsT1: OneOf holds T{Index}.");

        /// <summary>Gets the <typeparamref name="T3"/> value.</summary>
        public T3 AsT2 => IsT2 ? _value3! : throw new InvalidOperationException($"Cannot access AsT2: OneOf holds T{Index}.");

        /// <summary>Creates a OneOf holding a <typeparamref name="T1"/>.</summary>
        public static OneOf<T1, T2, T3> FromT0(T1 value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OneOf<T1, T2, T3>(value);
        }

        /// <summary>Creates a OneOf holding a <typeparamref name="T2"/>.</summary>
        public static OneOf<T1, T2, T3> FromT1(T2 value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OneOf<T1, T2, T3>(value);
        }

        /// <summary>Creates a OneOf holding a <typeparamref name="T3"/>.</summary>
        public static OneOf<T1, T2, T3> FromT2(T3 value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OneOf<T1, T2, T3>(value);
        }

        /// <summary>Implicitly wraps a <typeparamref name="T1"/> value.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T1 value) => FromT0(value);

        /// <summary>Implicitly wraps a <typeparamref name="T2"/> value.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T2 value) => FromT1(value);

        /// <summary>Implicitly wraps a <typeparamref name="T3"/> value.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T3 value) => FromT2(value);

        /// <summary>Pattern matches on the current slot and returns a value.</summary>
        public TResult Match<TResult>(Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2)
        {
            ArgumentNullException.ThrowIfNull(f0);
            ArgumentNullException.ThrowIfNull(f1);
            ArgumentNullException.ThrowIfNull(f2);
            return Index switch
            {
                0 => f0(_value1!),
                1 => f1(_value2!),
                2 => f2(_value3!),
                _ => throw new InvalidOperationException("OneOf in invalid state.")
            };
        }

        /// <summary>Pattern matches on the current slot and executes a side effect.</summary>
        public void Switch(Action<T1> a0, Action<T2> a1, Action<T3> a2)
        {
            ArgumentNullException.ThrowIfNull(a0);
            ArgumentNullException.ThrowIfNull(a1);
            ArgumentNullException.ThrowIfNull(a2);
            switch (Index)
            {
                case 0: a0(_value1!); break;
                case 1: a1(_value2!); break;
                case 2: a2(_value3!); break;
                default: throw new InvalidOperationException("OneOf in invalid state.");
            }
        }

        /// <summary>Indicates whether the current instance is equal to another instance of the same type.</summary>
        public bool Equals(OneOf<T1, T2, T3> other) => Index == other.Index && Index switch
        {
            0 => EqualityComparer<T1>.Default.Equals(_value1!, other._value1!),
            1 => EqualityComparer<T2>.Default.Equals(_value2!, other._value2!),
            2 => EqualityComparer<T3>.Default.Equals(_value3!, other._value3!),
            _ => false
        };

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is OneOf<T1, T2, T3> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => Index switch
        {
            0 => HashCode.Combine(Index, _value1),
            1 => HashCode.Combine(Index, _value2),
            2 => HashCode.Combine(Index, _value3),
            _ => 0
        };

        /// <summary>Determines whether two <see cref="OneOf{T1, T2, T3}"/> instances are equal.</summary>
        public static bool operator ==(OneOf<T1, T2, T3> left, OneOf<T1, T2, T3> right) => left.Equals(right);

        /// <summary>Determines whether two <see cref="OneOf{T1, T2, T3}"/> instances are not equal.</summary>
        public static bool operator !=(OneOf<T1, T2, T3> left, OneOf<T1, T2, T3> right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"T{Index}({Value})";
    }
}
