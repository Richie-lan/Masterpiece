using Masterpiece.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.MasterResource
{
    public class MasterErrorMsg
    {
        public const string ConstWrong = "错误";

        /// <summary>
        /// 根据错误枚举获取错误提示
        /// </summary>
        /// <param name="e">错误枚举值</param>
        /// <param name="para">替换参数</param>
        /// <returns></returns>
        public static string GetErrorMsgByEnum(ErrorCodeEnum e, params string[] para)
        {
            switch (e)
            {
                case ErrorCodeEnum.Wrong: return ConstWrong;
                default: return "未知错误";
            }
        }
    }
}
