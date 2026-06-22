using ExoMail.Smtp.Client.Enums;
using System;

namespace ExoMail.Smtp.Client.Interfaces
{
    internal class ExecutionResult : IExecutionResult
    {
        public bool IsSuccessfull => ResponseResult == ResponseResult.OK;
        public ResponseResult ResponseResult { get; set; }
        public int ResponseCode { get; set; }
        public string Response { get; set; }

        public ExecutionResult()
        {
        }

        public static IExecutionResult Parse(string response)
        {
            string code = response.Substring(0, 3);

            var result = new ExecutionResult();
            result.ResponseCode = Int32.Parse(code);
            result.Response = response;

            if (response.StartsWith("2"))
                result.ResponseResult = ResponseResult.OK;
            else if (response.StartsWith("3"))
                result.ResponseResult = ResponseResult.Input;
            else if (response.StartsWith("4"))
                result.ResponseResult = ResponseResult.TempFail;
            else if (response.StartsWith("5"))
                result.ResponseResult = ResponseResult.PermFail;
            else
                result.ResponseResult = ResponseResult.Invalid;

            return result;
        }
    }
}