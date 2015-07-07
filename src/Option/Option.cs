using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Options
{
    [DebuggerDisplay("{ToString()}")]
    public struct Option<T>
    {
        private readonly T _value;

        public T Unwrap()
        {
            if (IsNone)
                throw new InvalidOperationException("Cannot unwrap Option type None");
            return _value;
        }

        private readonly OptionType _optionType;

        public bool IsSome { get { return _optionType == OptionType.Some; } }

        public bool IsNone { get { return _optionType == OptionType.None; } }

        public static Option<T> Some(T value)
        {
            return new Option<T>(OptionType.Some, value);
        }

        public static Option<T> None()
        {
            return new Option<T>(OptionType.None);
        }

        private Option(OptionType optionType, T value = default(T))
            :this()
        {
            _optionType = optionType;
            _value = value;
        }

        public void Match(Action<T> someAction, Action noneAction)
        {
            if (IsSome)
                someAction(_value);
            else
                noneAction();
        }

        public TReturn Match<TReturn>(Action<T> someAction, Func<TReturn> noneFunc)
        {
            if (IsSome)
                someAction(_value);
            else
                return noneFunc();
            return default(TReturn);
        }

        public TReturn Match<TReturn>(Func<T, TReturn> someFunc, Action noneAction)
        {
            if (IsSome)
                return someFunc(_value);

            noneAction();

            return default(TReturn);
        }

        public TReturn Match<TReturn>(Func<T, TReturn> someFunc, Func<TReturn> noneFunc)
        {
            return IsSome ? someFunc(_value) : noneFunc();
        }

        public static explicit operator T(Option<T> option)
        {
            return option.Unwrap();
        }

        public static implicit operator Option<T>(T value)
        {
            return value == null ? None() : Some(value);
        }

        public static bool operator ==(Option<T> one, Option<T> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Option<T> one, Option<T> two)
        {
            return !one.Equals(two);
        }

        public bool Equals(Option<T> other)
        {
            return _optionType == other._optionType && EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Option<T> && Equals((Option<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _optionType*397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", _optionType, IsSome ? _value.ToString() : string.Empty);
        }
    }
}
