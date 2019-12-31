using Masterpiece.Code.Cache;
using Masterpiece.Code.Common;
using Masterpiece.Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Bll
{
    public abstract class BllBase
    {
        protected MasterpieceDbContextFactory contextFactory;

        public BllBase(MasterpieceDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected string tranId;
        public void BeginTran()
        {
            tranId = Guid.NewGuid().ToString();
        }

        public void CommitTran()
        {
            CacheHelper.NotifyRefreshCacheForTranCommit(tranId);
            tranId = "";
        }

        public void RollbackTran()
        {
            tranId = "";
        }

        private void Log(string log)
        {
            LogHelper.WriteLog(log);
        }
    }
}
