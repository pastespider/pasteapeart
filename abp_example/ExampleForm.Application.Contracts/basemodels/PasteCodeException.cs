//namespace ExampleForm.Application.Contracts
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class PasteCodeException : Exception
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public int Code { get; set; } = 500;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="code"></param>
//        public PasteCodeException(string message, ExceptionCode code = ExceptionCode.ShowInfo) : base(message)
//        {
//            Code = (int)code;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="code"></param>
//        public PasteCodeException(string message, string code) : base(message)
//        {
//            int.TryParse(code, out var _code);
//            Code = _code;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="code"></param>
//        public PasteCodeException(string message, int code) : base(message)
//        {
//            //int.TryParse(code, out var _code);
//            Code = code;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="innerException"></param>
//        public PasteCodeException(string message, Exception innerException) : base(message, innerException)
//        {

//        }

//    }

//    /// <summary>
//    /// 数据未找到
//    /// </summary>
//    public class PasteNotFoundException : PasteCodeException
//    {

//        /// <summary>
//        /// 数据未找到
//        /// </summary>
//        public PasteNotFoundException() : base("没有找到这个数据", ExceptionCode.NotFound) { }
//    }

//    /// <summary>
//    /// 参数错误
//    /// </summary>
//    public class PasteArgsException : PasteCodeException
//    {
//        /// <summary>
//        /// 参数错误
//        /// </summary>
//        /// <param name="args"></param>
//        public PasteArgsException(string args) : base($"参数{args} 输入不符合约定，请重新输入!", ExceptionCode.ModelErr) { }
//    }

//    /// <summary>
//    /// 权限不足
//    /// </summary>
//    public class PasteNotAuthException : PasteCodeException
//    {

//        /// <summary>
//        /// 权限不足
//        /// </summary>
//        /// <param name="model"></param>
//        /// <param name="role"></param>
//        public PasteNotAuthException(string model, string role) : base($"权限不足，无法继续执行，需要权限{model}-{role}!", ExceptionCode.UnRole) { }
//    }

//}
