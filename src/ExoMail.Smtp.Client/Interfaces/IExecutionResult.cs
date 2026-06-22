using ExoMail.Smtp.Client.Enums;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal interface IExecutionResult
    {
        ResponseResult ResponseResult { get; }
        int ResponseCode { get; }
        bool IsSuccessfull { get; }
        string Response { get; }
    }
}