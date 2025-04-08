using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketManagerGenerator
{
    public class PacketManagerFormat
    {
        // {0} using 등록
		// {1} 패킷매니저명 등록
		// {2} 패킷 등록
        public static string managerFormat =
@"using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using Server.Core.Interface;
using Server.Utill;
using log4net;
{0}

public class {1}PacketManager : IPacketManager
{{
    private readonly ILog log;
	private readonly {1}PacketHandler packetHandler;

	public {1}PacketManager(ILogFactory logFactory, {1}PacketHandler packetHandler)
	{{
		log = logFactory.CreateLogger<{1}PacketManager>();
		this.packetHandler = packetHandler;
		Register();
	}}

	private Dictionary<ushort, Action<ISession, ReadOnlyMemory<byte>, ushort>> onRecv = [];
	private Dictionary<ushort, Action<ISession, IMessage>> handler = [];

	public Action<ISession, IMessage, ushort>? CustomHandler;

	private void Register()
	{{
		{2}
	}}

	public void OnRecvPacket(ISession session, ReadOnlyMemory<byte> buffer)
	{{
		ushort count = 0;
        var span = buffer.Span;

        ushort size = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;
        ushort id = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;

		Action<ISession, ReadOnlyMemory<byte>, ushort> action = null;
		if (onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}}

	private void MakePacket<T>(ISession session, ReadOnlyMemory<byte> buffer, ushort id) where T : IMessage, new()
	{{
		T pkt = new T();       

        pkt.MergeFrom(buffer.Span.Slice(4)); // Skip 4바이트 (size + id)

		if(CustomHandler != null)
		{{
			CustomHandler.Invoke(session, pkt, id);
		}}
		else
		{{
            Action<ISession, IMessage> action = null;
            if (handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }}		
	}}
}}";

        // {0} MsgId
        // {1} 패킷 이름
        // {1} 패킷 핸들러 이름
        public static string managerRegisterFormat =
@"		
		onRecv.Add((ushort){0}.{1}, MakePacket<{1}>);
		handler.Add((ushort){0}.{1}, packetHandler.{1}Handler);";
    }
}
