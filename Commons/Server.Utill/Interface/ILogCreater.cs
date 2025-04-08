﻿using Server.Core;
using Server.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utill.Interface
{
    public interface ILogCreater<TSession>
    {
       abstract static TSession Create(ILogFactory logFactory, IPacketManager packetManager , X509Certificate2? cert = null);
    }

}
