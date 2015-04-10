using System;
using System.Collections.Generic;

namespace Options
{
    public struct Result<T>
    {
        private readonly T _value;
        private readonly Exception _errorException;
        private readonly ResultType _resultType;

        public bool IsOk
        {
            get { return _resultType == ResultType.Ok; }
        }

        public bool IsError
        {
            get { return _resultType == ResultType.Error; }
        }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(ResultType.Ok, value);
        }

        public static Result<T> Error(Exception exception)
        {
            return new Result<T>(ResultType.Error, ex: exception);
        }

        private Result(ResultType resultType, T value = default(T), Exception ex = null)
        {
            _resultType = resultType;
            _value = value;
            _errorException = ex;
        }

        public void Match(Action<T> okAction, Action<Exception> errorAction)
        {
            if (IsOk)
                okAction(_value);
            else
                errorAction(_errorException);
        }

        public TReturn Match<TReturn>(Action<T> okAction, Func<Exception, TReturn> errorFunc)
        {
            if (IsOk)
                okAction(_value);
            else
                return errorFunc(_errorException);
            return default(TReturn);
        }

        public TReturn Match<TReturn>(Func<T, TReturn> okFunc, Action<Exception> errorAction)
        {
            if (IsOk)
                return okFunc(_value);

            errorAction(_errorException);
            return default(TReturn);
        }

        public TReturn Match<TReturn>(Func<T, TReturn> okFunc, Func<Exception, TReturn> errorFunc)
        {
            return IsOk ? okFunc(_value) : errorFunc(_errorException);
        }

        public static explicit operator T(Result<T> result)
        {
            return result._value;
        }

        public static implicit operator Result<T>(T value)
        {
            return value == null ? Error(new ArgumentNullException("value", "Cannot use null to implicitly create a result")) : Ok(value);
        }

        public static bool operator ==(Result<T> one, Result<T> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Result<T> one, Result<T> two)
        {
            return !one.Equals(two);
        }

        public bool Equals(Result<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) && Equals(_errorException, other._errorException) && _resultType == other._resultType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result<T> && Equals((Result<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<T>.Default.GetHashCode(_value);
                hashCode = (hashCode*397) ^ (_errorException != null ? _errorException.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) _resultType;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", _resultType, IsOk? _value.ToString() : _errorException.GetType().Name);
        }
    }
}