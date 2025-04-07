using AuthDBServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Data.AuthDb;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AuthDBServer.Packets
{
    public class AuthDbPacketHandler
    {
        private readonly ILog log;

        public AuthDbPacketHandler(IServiceProvider service,  ILogFactory logFactory)
        {
            this.log = logFactory.CreateLogger<AuthDBServer>();
        }

        public void AdServerStateHandler(PacketSession session, IMessage packet)
        {
        }

        public void AdGetAccountVerifyInfoHandler(PacketSession session, IMessage packet)
        {
            AdGetAccountVerifyInfo adGetAccountVerifyInfo = packet as AdGetAccountVerifyInfo;
            AuthSession authSession = session as AuthSession;

        }
    }
}
