using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masterpiece.Code.Common
{
    public class JsonResponse<T> : JsonSimpleResponse
    {
        public T Data
        {
            get;set;
        }
    }

    public class JsonSimpleResponse
    {
        public int ErrorCode
        {
            set; get;
        }

        public string ErrorMsg
        {
            set; get;
        }

        public bool State
        {
            set; get;
        }
    }
}