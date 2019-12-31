using Masterpiece.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.MasterException
{
    public class MasterAdvancedException<T> : MasterException
    {
        public MasterAdvancedException(ErrorCodeEnum errorEnum, string errorMsg) : this((int)errorEnum, errorMsg)
        {

        }

        public MasterAdvancedException(ErrorCodeEnum errorEnum, string errorMsg, params object[] inputParams)
            : this(errorEnum, string.Format(errorMsg, inputParams))
        {

        }

        public MasterAdvancedException(int errorCode, string errorMsg)
            : base(errorCode, errorMsg)
        {
            ErrorCode = errorCode;
        }

        public MasterAdvancedException(int errorCode, string errorMsg, T r)
         : base(errorCode, errorMsg)
        {
            ErrorCode = errorCode;
            Result = r;
        }

        public T Result
        {
            set;
            get;
        }
    }
}
