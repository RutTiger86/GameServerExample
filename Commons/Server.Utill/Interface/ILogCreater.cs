using Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utill.Interface
{
    public interface ILogCreater<TSession>
    {
       abstract static TSession Create(ILogFactory logFactory);
    }
}
