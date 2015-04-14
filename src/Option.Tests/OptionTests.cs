﻿using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Options.Tests
{
    public class OptionTests
    {
        [Fact]
        public void can_create_some_option()
        {
            var option = Option.Some(new object());

            option.IsSome.ShouldBe(true);
            option.IsNone.ShouldBe(false);
        }

        [Fact]
        public void can_create_some_option_with_empty_arguments()
        {
            var option = Option.Some<object>();

            option.IsSome.ShouldBe(true);
            option.IsNone.ShouldBe(false);
        }

        [Fact]
        public void can_create_none_option()
        {
            var option = Option.None();

            option.IsSome.ShouldBe(false);
            option.IsNone.ShouldBe(true);
        }

        [Fact]
        public void some_void_match_calls_correct_branch()
        {
            var some = Option.Some<object>();
            bool someResult = false, noneResult = true; // noneResult is true to show it actually doesn't change and isn't default

            some.Match<object>(value => someResult = true, () => noneResult = false);
            someResult.ShouldBe(true);
            noneResult.ShouldBe(true);
        }

        [Fact]
        public void none_void_match_calls_correct_branch()
        {
            var none = Option.None();
            bool noneResult = false, someResult = true; // someResult is true to show it actually doesn't change and isn't default

            none.Match<object>(value => someResult = false, () => noneResult = true);
            noneResult.ShouldBe(true);
            someResult.ShouldBe(true);
        }

        [Fact]
        public void some_none_func_match_calls_correct_some_branch()
        {
            var some = Option.Some<object>();
            var someResult = false;

            var noneResult = some.Match<object, bool>(someAction: value => someResult = true, noneFunc: () => true); // noneFunc returns true to show that it does not get overwritten
            someResult.ShouldBe(true);
            noneResult.ShouldBe(false);
        }

        [Fact]
        public void none_none_func_match_calls_correct_some_branch()
        {
            var none = Option.None();
            var someResult = false;

            var noneResult = none.Match<object, bool>(someAction: value => someResult = true, noneFunc: () => true);
            noneResult.ShouldBe(true);
            someResult.ShouldBe(false);
        }

        [Fact]
        public void some_some_func_match_calls_correct_branch()
        {
            var some = Option.Some<object>();
            var noneResult = true;

            var someResult = some.Match<object, bool>(x => true, noneAction: () => noneResult = false);
            someResult.ShouldBe(true);
            noneResult.ShouldBe(true);
        }

        [Fact]
        public void none_some_func_match_calls_correct_branch()
        {
            var none = Option.None();
            var noneResult = true;

            var someResult = none.Match<object, bool>(x => true, noneAction: () => noneResult = false);
            noneResult.ShouldBe(false);
            someResult.ShouldBe(false);
        }

        [Fact]
        public void some_both_func_match_calls_correct_branch()
        {
            var some = Option.Some<object>();

            var result = some.Match<object, bool>(x => true, () => false);
            result.ShouldBe(true);
        }

        [Fact]
        public void none_both_func_match_calls_correct_branch()
        {
            var none = Option.None();

            var result = none.Match<object, bool>(x => false, () => true);
            result.ShouldBe(true);
        }

        [Fact]
        public void can_retrieve_value_for_some_option_type()
        {
            var value = new object();
            var option = Option.Some(value);

            option.Value.ShouldBeSameAs(value);
        }

        [Fact]
        public void throws_invalid_optation_exception_when_retrieving_value_for_none_option_type()
        {
            Should.Throw<InvalidOperationException>(() => Task.Run(() => Option.None().Value));
        }

        [Fact]
        public void throws_argument_exception_when_matching_a_different_type_than_created()
        {
            Should.Throw<ArgumentException>(() => Task.Run(() => Option.Some(new object()).Match<Guid>(value => { }, () => { })));
        }
    }
}
