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
{0}

class {1}PacketManager
{{
	#region Singleton
	static {1}PacketManager _instance = new {1}PacketManager();
	public static {1}PacketManager Instance {{ get {{ return _instance; }} }}
	#endregion

	{1}PacketManager()
	{{
		Register();
	}}

	Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler;

	public void Register()
	{{
		{2}
	}}

	public void OnRecvPacket(PacketSession session, ReadOnlyMemory<byte> buffer)
	{{
		ushort count = 0;
        var span = buffer.Span;

        ushort size = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;
        ushort id = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;

		Action<PacketSession, ReadOnlyMemory<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}}

	void MakePacket<T>(PacketSession session, ReadOnlyMemory<byte> buffer, ushort id) where T : IMessage, new()
	{{
		T pkt = new T();       

        pkt.MergeFrom(buffer.Span.Slice(4)); // Skip 4바이트 (size + id)

		if(CustomHandler != null)
		{{
			CustomHandler.Invoke(session, pkt, id);
		}}
		else
		{{
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }}		
	}}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}}
}}";

        // {0} MsgId
        // {1} 패킷 이름
        // {1} 패킷 핸들러 이름
        public static string managerRegisterFormat =
@"		
		_onRecv.Add((ushort){0}.{1}, MakePacket<{1}>);
		_handler.Add((ushort){0}.{1}, {2}PacketHandler.{1}Handler);";
    }
}
