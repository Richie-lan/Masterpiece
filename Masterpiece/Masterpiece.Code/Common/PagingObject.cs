using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Common
{
    public class PagingObject
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public PagingObject()
        {

        }

        public PagingObject(int pageIndex,int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
