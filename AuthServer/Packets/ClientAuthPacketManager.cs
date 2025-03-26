using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using AuthServer.Packets;
using Server.Data.ClientAuth;


class ClientAuthPacketManager
{
	#region Singleton
	static ClientAuthPacketManager _instance = new ClientAuthPacketManager();
	public static ClientAuthPacketManager Instance { get { return _instance; } }
	#endregion

	ClientAuthPacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler;

	public void Register()
	{
				
		_onRecv.Add((ushort)ClientAuthPacketId.CaServerState, MakePacket<CaServerState>);
		_handler.Add((ushort)ClientAuthPacketId.CaServerState, ClientAuthPacketHandler.CaServerStateHandler);		
		_onRecv.Add((ushort)ClientAuthPacketId.CaLogin, MakePacket<CaLogin>);
		_handler.Add((ushort)ClientAuthPacketId.CaLogin, ClientAuthPacketHandler.CaLoginHandler);		
		_onRecv.Add((ushort)ClientAuthPacketId.CaWorldList, MakePacket<CaWorldList>);
		_handler.Add((ushort)ClientAuthPacketId.CaWorldList, ClientAuthPacketHandler.CaWorldListHandler);		
		_onRecv.Add((ushort)ClientAuthPacketId.CaEnterWorld, MakePacket<CaEnterWorld>);
		_handler.Add((ushort)ClientAuthPacketId.CaEnterWorld, ClientAuthPacketHandler.CaEnterWorldHandler);
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