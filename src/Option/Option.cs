using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Options
{
    [DebuggerDisplay("{ToString()}")]
    public struct Option
    {
        private readonly object _value;
        public object Value {
            get
            {
                if (IsNone)
                    throw new InvalidOperationException("Cannot get value of a none Option type");
                return _value;
            }
        }
        private readonly OptionType _optionType;

        /// <summary>
        /// Gets if option type is Some
        /// </summary>
        public bool IsSome { get { return _optionType == OptionType.Some; } }

        /// <summary>
        /// Gets if option type is None
        /// </summary>
        public bool IsNone { get { return _optionType == OptionType.None; } }

        /// <summary>
        /// Creates a Some option type with a wrapped <param name="value"/>
        /// </summary>
        /// <param name="value">The value to wrap in the Some option</param>
        /// <returns>An <see cref="Option"/> wrapping <param name="value" /> </returns>
        public static Option Some(object value)
        {
            return new Option(OptionType.Some, value);
        }

        /// <summary>
        /// Creates a some option type with a wrapped value of type <typeparam name="T"/>
        /// Used for easy construction of the some type without newing up an object
        /// </summary>
        /// <typeparam name="T">The type of value to wrap</typeparam>
        /// <returns>An <see cref="Option" /> wrapping an instance of <typeparam name="T" /></returns>
        public static Option Some<T>()
            where T : new()
        {
            return Some(new T());
        }
 
        /// <summary>
        /// Creates a none option that is treated as no value
        /// </summary>
        /// <returns>An empty <see cref="Option" /></returns>
        public static Option None()
        {
            return new Option(OptionType.None);
        }

        private Option(OptionType optionType, object value = null)
            :this()
        {
            _optionType = optionType;
            _value = value;
        }

        /// <summary>
        /// Matches the option type to the corresponding action to call.
        /// </summary>
        /// <typeparam name="TValue">Generic parameter of value for hard type casting</typeparam>
        /// <param name="someAction">The action to perform if <see cref="Option" /> is Some</param>
        /// <param name="noneAction">The action to perform if <see cref="Option" /> is None</param>
        public void Match<TValue>(Action<TValue> someAction, Action noneAction)
        {
            if (IsSome)
            {
                Verify<TValue>();
                someAction((TValue) _value);
            }
            else
                noneAction();
        }

        /// <summary>
        /// Matches the option type to the corresponding action to call.
        /// </summary>
        /// <typeparam name="TValue">Generic parameter of value for hard type casting</typeparam>
        /// <typeparam name="TReturn">Return type of the <param name="noneFunc" /></typeparam>
        /// <param name="someAction">The action to perform if <see cref="Option" /> is Some</param>
        /// <param name="noneFunc">The func to execute and return if <see cref="Option" /> is None</param>
        /// <returns>Some value of type <typeparam name="TReturn" /> from noneFunc, else default(<typeparam name="TReturn" />)</returns>
        public TReturn Match<TValue, TReturn>(Action<TValue> someAction, Func<TReturn> noneFunc)
        {
            if (IsSome)
            {
                Verify<TValue>();
                someAction((TValue) _value);
            }
            else
                return noneFunc();

            return default(TReturn);
        }

        /// <summary>
        /// Matches the option type to the corresponding action to call.
        /// </summary>
        /// <typeparam name="TValue">Generic parameter of value for hard type casting</typeparam>
        /// <typeparam name="TReturn">Return type of the <param name="someFunc" /></typeparam>
        /// <param name="someFunc">The func to execute and return if <see cref="Option" /> is Some</param>
        /// <param name="noneAction">The action to perform if <see cref="Option" /> is None</param>
        /// <returns>Some value of type <typeparam name="TReturn" /> from someFunc, else default(<typeparam name="TReturn" />)</returns>
        public TReturn Match<TValue, TReturn>(Func<TValue, TReturn> someFunc, Action noneAction)
        {
            if (IsSome)
            {
                Verify<TValue>();
                return someFunc((TValue) _value);
            }

            noneAction();
            return default(TReturn);
        }

        /// <summary>
        /// Matches the option type to the corresponding action to call.
        /// </summary>
        /// <typeparam name="TValue">Generic parameter of value for hard type casting</typeparam>
        /// <typeparam name="TReturn">Return type of the <param name="noneFunc" /></typeparam>
        /// <param name="someFunc">The func to execute and return if <see cref="Option" /> is Some</param>
        /// <param name="noneFunc">The func to execute and return if <see cref="Option" /> is None</param>
        /// <returns>Some value of type <typeparam name="TReturn" /> from either func depending on option type</returns>
        public TReturn Match<TValue, TReturn>(Func<TValue, TReturn> someFunc, Func<TReturn> noneFunc)
        {
            if (IsSome)
            {
                Verify<TValue>();
                return someFunc((TValue) _value);
            }
            return noneFunc();
        }

        public static object ToObject(Option option)
        {
            return option._value;
        }

        public static Option FromObject(object value)
        {
            return value == null ? None() : Some(value);
        }

        public static bool operator ==(Option one, Option two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Option one, Option two)
        {
            return !one.Equals(two);
        }

        public bool Equals(Option other)
        {
            return Equals(_value, other._value) && _optionType == other._optionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Option && Equals((Option) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_value != null ? _value.GetHashCode() : 0)*397) ^ (int) _optionType;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", _optionType, IsSome ? _value.ToString() : string.Empty);
        }

        private void Verify<TValue>()
        {
            if (!(_value is TValue))
                throw new ArgumentException(string.Format("The type of {0} does not match the wrapped type {1}",
                    typeof (TValue).Name, _value.GetType().Name));
        }
    }

    public struct Option<T>
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (IsNone)
                    throw new InvalidOperationException("Cannot get value of a none Option type");
                return _value;
            }
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
            return option.Value;
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
