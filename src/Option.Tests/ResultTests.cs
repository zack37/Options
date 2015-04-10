using System;
using Shouldly;
using Xunit;

namespace Options.Tests
{
    public class ResultTests
    {
        [Fact]
        public void can_create_ok_result()
        {
            var option = Result<object>.Ok(new object());

            option.IsOk.ShouldBe(true);
            option.IsError.ShouldBe(false);
        }

        [Fact]
        public void can_create_error_result()
        {
            var option = Result<object>.Error(new Exception("I failed"));

            option.IsOk.ShouldBe(false);
            option.IsError.ShouldBe(true);
        }

        [Fact]
        public void ok_void_match_calls_correct_branch()
        {
            var ok = Result<object>.Ok(new object());
            bool okResult = false,
                errorResult = true; // set to true to show it doesn't change and isn't default

            ok.Match(okAction: value => okResult = true, errorAction: e => errorResult = false);
            okResult.ShouldBe(true);
            errorResult.ShouldBe(true);
        }

        [Fact]
        public void error_void_match_calls_correct_branch()
        {
            var error = Result<object>.Error(new Exception("I failed"));
            bool okResult = true, // set to true to show it doesn't change and isn't default
                errorResult = false;

            error.Match(okAction: value => okResult = false, errorAction: e =>
            {
                e.Message.ShouldBe("I failed");
                errorResult = true;
            });
            okResult.ShouldBe(true);
            errorResult.ShouldBe(true);
        }

        [Fact]
        public void ok_error_func_match_calls_correct_branch()
        {
            var ok = Result<object>.Ok(new object());
            var okResult = false;

            var errorResult = ok.Match(okAction: value => okResult = true, errorFunc: e => true); // errorFunc returns true to show that it does not change from default
            okResult.ShouldBe(true);
            errorResult.ShouldBe(false);
        }

        [Fact]
        public void error_error_func_match_calls_correct_branch()
        {
            var error = Result<object>.Error(new Exception("I failed"));
            var okResult = false;

            var errorResult = error.Match(okAction: value => okResult = true, errorFunc: e =>
            {
                e.Message.ShouldBe("I failed");
                return true;
            });
            okResult.ShouldBe(false);
            errorResult.ShouldBe(true);
        }

        [Fact]
        public void ok_ok_func_match_calls_correct_branch()
        {
            var ok = Result<object>.Ok(new object());
            var errorResult = true;

            var okResult = ok.Match(okFunc: value => true, errorAction: e => errorResult = false);
            errorResult.ShouldBe(true);
            okResult.ShouldBe(true);
        }

        [Fact]
        public void error_ok_func_match_calls_correct_branch()
        {
            var error = Result<object>.Error(new Exception("I failed"));
            var errorResult = false;

            var okResult = error.Match(okFunc: value => true, errorAction: e =>
            {
                e.Message.ShouldBe("I failed");
                errorResult = true;
            });
            errorResult.ShouldBe(true);
            okResult.ShouldBe(false);
        }

        [Fact]
        public void ok_both_func_match_calls_correct_branch()
        {
            var ok = Result<object>.Ok(new object());
            var result = ok.Match(okFunc: value => true, errorFunc: e => false);
            result.ShouldBe(true);
        }

        [Fact]
        public void none_both_func_match_calls_correct_branch()
        {
            var error = Result<object>.Error(new Exception("I failed"));
            var result = error.Match(okFunc: value => false, errorFunc: e =>
            {
                e.Message.ShouldBe("I failed");
                return true;
            });
            result.ShouldBe(true);
        }
    
    }
}