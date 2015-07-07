using System;
using Shouldly;
using Xunit;

namespace Options.Tests
{
    public class OptionTests
    { 
        [Fact]
        public void can_create_some_option()
        {
            var option = Option<object>.Some(new object());

            option.IsSome.ShouldBe(true);
            option.IsNone.ShouldBe(false);
        }

        [Fact]
        public void can_create_none_option()
        {
            var option = Option<object>.None();

            option.IsNone.ShouldBe(true);
            option.IsSome.ShouldBe(false);
        }

        [Fact]
        public void some_void_match_calls_correct_branch()
        {
            var some = Option<object>.Some(new object());
            bool someResult = false, noneResult = true; // noneResult is true to show it actually doesn't change and isn't default

            some.Match(someAction: value => someResult = true, noneAction: () => noneResult = false);
            someResult.ShouldBe(true);
            noneResult.ShouldBe(true);
        }

        [Fact]
        public void none_void_match_calls_correct_branch()
        {
            var some = Option<object>.None();
            bool noneResult = false, someResult = true; // someResult is true to show it actually doesn't change and isn't default

            some.Match(someAction: value => someResult = false, noneAction: () => noneResult = true);
            noneResult.ShouldBe(true);
            someResult.ShouldBe(true);
        }

        [Fact]
        public void some_none_func_match_calls_correct_some_branch()
        {
            var some = Option<object>.Some(new object());
            var someResult = false;

            var noneResult = some.Match(someAction: value => someResult = true, noneFunc: () => true); // noneFunc returns true to show that it does not get overwritten
            someResult.ShouldBe(true);
            noneResult.ShouldBe(false);
        }

        [Fact]
        public void none_none_func_match_calls_correct_some_branch()
        {
            var none = Option<object>.None();
            var someResult = false;

            var noneResult = none.Match(someAction: value => someResult = true,noneFunc: () => true);
            noneResult.ShouldBe(true);
            someResult.ShouldBe(false);
        }

        [Fact]
        public void some_some_func_match_calls_correct_branch()
        {
            var some = Option<object>.Some(new object());
            var noneResult = true;

            var someResult = some.Match(x => true, noneAction: () => noneResult = false);
            someResult.ShouldBe(true);
            noneResult.ShouldBe(true);
        }

        [Fact]
        public void none_some_func_match_calls_correct_branch()
        {
            var none = Option<object>.None();
            var noneResult = true;

            var someResult = none.Match(x => true, noneAction: () => noneResult = false);
            noneResult.ShouldBe(false);
            someResult.ShouldBe(false);
        }

        [Fact]
        public void some_both_func_match_calls_correct_branch()
        {
            var some = Option<object>.Some(new object());

            var result = some.Match(x => true, () => false);
            result.ShouldBe(true);
        }

        [Fact]
        public void none_both_func_match_calls_correct_branch()
        {
            var none = Option<object>.None();

            var result = none.Match(x => false, () => true);
            result.ShouldBe(true);
        }
    
        [Fact]
        public void can_retrieve_value_for_some_option_type()
        {
            var value = new object();
            var option = Option<object>.Some(value);

            option.Unwrap().ShouldBeSameAs(value);
        }

        [Fact]
        public void throws_invalid_operation_exception_when_retrieving_value_for_none_option_type()
        {
            Should.Throw<InvalidOperationException>(() => Option<object>.None().Unwrap());
        }
    }
}