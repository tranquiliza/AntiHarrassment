namespace AntiHarassment.Core
{
    public enum ResultState
    {
        Failure = 0,
        Success = 1,
        NoContent = 2,
        AccessDenied = 3
    }

    public interface IResult
    {
        ResultState State { get; }
        string FailureReason { get; }
    }

    public interface IResult<T> : IResult
    {
        T Data { get; }
    }
}
