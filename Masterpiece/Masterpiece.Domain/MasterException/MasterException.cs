using Masterpiece.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.MasterException
{
    public class MasterException : Exception
    {
        private int errorCode;
        public int ErrorCode
        {
            set
            {
                errorCode = value;
            }
            get
            {
                return errorCode;
            }
        }

        public ErrorCodeEnum errorCodeEnum;
        public ErrorCodeEnum ErrorCodeEnum
        {
            set
            {
                errorCodeEnum = value;
                errorCode = (int)(value);
            }
            get
            {
                return errorCodeEnum;
            }
        }


        public MasterException(ErrorCodeEnum errorEnum, string errorMsg) : this((int)errorEnum, errorMsg)
        {

        }

        public MasterException(ErrorCodeEnum errorEnum, string errorMsg, params object[] inputParams)
            : this(errorEnum, string.Format(errorMsg, inputParams))
        {

        }

        public MasterException(int errorCode, string errorMsg)
            : base(errorMsg)
        {
            ErrorCode = errorCode;
        }
    }
}
