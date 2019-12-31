using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.Entity
{
    public interface IChangeTrack
    {
        List<string> ChangedFields();

        void EnableAudit();

        bool IsEnableAudit();
    }
}
