using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Core
{
    public class Result : IResult
    {
        public ResultState State { get; protected set; }

        public string FailureReason { get; protected set; }

        public static Result Succeeded => new Result
        {
            FailureReason = string.Empty,
            State = ResultState.Success
        };

        public static Result Failure(string failureReason) => new Result
        {
            State = ResultState.Failure,
            FailureReason = failureReason
        };

        public static Result Unauthorized() => new Result
        {
            State = ResultState.AccessDenied
        };
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; private set; }

        public new static Result<T> Succeeded(T data) => new Result<T>
        {
            State = ResultState.Success,
            Data = data
        };

        public new static Result<T> Failure(string failureReason) => new Result<T>
        {
            FailureReason = failureReason,
            State = ResultState.Failure
        };

        public static Result<T> NoContentFound() => new Result<T>
        {
            State = ResultState.NoContent
        };

        new public static Result<T> Unauthorized() => new Result<T>
        {
            State = ResultState.AccessDenied
        };
    }
}
