using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.Entity
{
    public interface ICaching<T> where T:class
    {
        Func<T, Boolean> CheckInLocal();
    }
}
