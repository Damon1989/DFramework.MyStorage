﻿namespace DFramework.MyStorage.SDK
{
    /// <summary>
    ///     API响应结果
    /// </summary>
    public class ApiResult
    {
        public ApiResult()
        {
            Success = true;
            ErrorCode = 0;
        }

        public ApiResult(int errorCode, string message = null)
        {
            ErrorCode = errorCode;
            Message = message;
            Success = false;
        }

        /// <summary>
        ///     API 执行是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     ErrorCode 为0 表示执行无异常
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     当API执行有异常时，对应的错误信息
        /// </summary>
        public string Message { get; set; }
    }

    public class ApiResult<TResult> : ApiResult
    {
        public ApiResult()
        {
            Success = true;
        }

        public ApiResult(TResult result)
            : this()
        {
            Result = result;
        }

        public ApiResult(int errorCode, string message = null)
            : base(errorCode, message)
        {
        }

        /// <summary>
        ///     API执行返回的结果
        /// </summary>
        public TResult Result { get; set; }
    }
}