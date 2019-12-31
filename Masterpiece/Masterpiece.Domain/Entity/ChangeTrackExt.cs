using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.Entity
{
    public static class ChangeTrackExt
    {
        public static void EnableAudit(this IChangeTrack obj)
        {
            obj.EnableAudit();
        }

        public static void EnableAudit(this IEnumerable<IChangeTrack> objs)
        {
            foreach (IChangeTrack track in objs)
            {
                track.EnableAudit();
            }
        }
    }
}
