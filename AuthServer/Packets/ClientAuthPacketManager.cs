using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using Server.Core.Interface;
using Server.Utill;
using log4net;
using AuthServer.Packets;
using Server.Data.ClientAuth;


public class ClientAuthPacketManager : IPacketManager
{
    private readonly ILog log;
	private readonly ClientAuthPacketHandler packetHandler;

	public ClientAuthPacketManager(ILogFactory logFactory, ClientAuthPacketHandler packetHandler)
	{
		log = logFactory.CreateLogger<ClientAuthPacketManager>();
		this.packetHandler = packetHandler;
		Register();
	}

	private Dictionary<ushort, Action<ISession, ReadOnlyMemory<byte>, ushort>> onRecv = [];
	private Dictionary<ushort, Action<ISession, IMessage>> handler = [];

	public Action<ISession, IMessage, ushort>? CustomHandler;

	private void Register()
	{
				
		onRecv.Add((ushort)ClientAuthPacketId.CaServerState, MakePacket<CaServerState>);
		handler.Add((ushort)ClientAuthPacketId.CaServerState, packetHandler.CaServerStateHandler);		
		onRecv.Add((ushort)ClientAuthPacketId.CaLogin, MakePacket<CaLogin>);
		handler.Add((ushort)ClientAuthPacketId.CaLogin, packetHandler.CaLoginHandler);		
		onRecv.Add((ushort)ClientAuthPacketId.CaWorldList, MakePacket<CaWorldList>);
		handler.Add((ushort)ClientAuthPacketId.CaWorldList, packetHandler.CaWorldListHandler);		
		onRecv.Add((ushort)ClientAuthPacketId.CaEnterWorld, MakePacket<CaEnterWorld>);
		handler.Add((ushort)ClientAuthPacketId.CaEnterWorld, packetHandler.CaEnterWorldHandler);
	}

	public void OnRecvPacket(ISession session, ReadOnlyMemory<byte> buffer)
	{
		ushort count = 0;
        var span = buffer.Span;

        ushort size = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;
        ushort id = BitConverter.ToUInt16(span.Slice(count, 2));
        count += 2;

		Action<ISession, ReadOnlyMemory<byte>, ushort> action = null;
		if (onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	private void MakePacket<T>(ISession session, ReadOnlyMemory<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();       

        pkt.MergeFrom(buffer.Span.Slice(4)); // Skip 4바이??(size + id)

		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
            Action<ISession, IMessage> action = null;
            if (handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }		
	}
}