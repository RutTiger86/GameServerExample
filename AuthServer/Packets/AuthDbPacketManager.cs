using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using AuthServer.Packets;
using Server.Data.AuthDb;


class AuthDbPacketManager
{
	#region Singleton
	static AuthDbPacketManager _instance = new AuthDbPacketManager();
	public static AuthDbPacketManager Instance { get { return _instance; } }
	#endregion

	AuthDbPacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler;

	public void Register()
	{
				
		_onRecv.Add((ushort)AuthDbPacketId.DaServerState, MakePacket<DaServerState>);
		_handler.Add((ushort)AuthDbPacketId.DaServerState, AuthDbPacketHandler.DaServerStateHandler);		
		_onRecv.Add((ushort)AuthDbPacketId.DaGetAccountVerifyInfo, MakePacket<DaGetAccountVerifyInfo>);
		_handler.Add((ushort)AuthDbPacketId.DaGetAccountVerifyInfo, AuthDbPacketHandler.DaGetAccountVerifyInfoHandler);
	}

	public void OnRecvPacket(PacketSession session, ReadOnlyMemory<byte> buffer)
	{
		ushort count = 0;
        var span = buffer.Span;

        ushort size = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;
        ushort id = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;

		Action<PacketSession, ReadOnlyMemory<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ReadOnlyMemory<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();       

        pkt.MergeFrom(buffer.Span.Slice(4)); // Skip 4바이트 (size + id)

		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }		
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}