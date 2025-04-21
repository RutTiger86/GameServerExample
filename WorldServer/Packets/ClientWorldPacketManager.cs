using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using Server.Core.Interface;
using Server.Utill;
using log4net;
using WorldServer.Packets;
using Server.Data.ClientWorld;


public class ClientWorldPacketManager : IPacketManager
{
    private readonly ILog log;
	private readonly ClientWorldPacketHandler packetHandler;

	public ClientWorldPacketManager(ILogFactory logFactory, ClientWorldPacketHandler packetHandler)
	{
		log = logFactory.CreateLogger<ClientWorldPacketManager>();
		this.packetHandler = packetHandler;
		Register();
	}

	private Dictionary<ushort, Action<ISession, ReadOnlyMemory<byte>, ushort>> onRecv = [];
	private Dictionary<ushort, Action<ISession, IMessage>> handler = [];

	public Action<ISession, IMessage, ushort>? CustomHandler;

	private void Register()
	{
				
		onRecv.Add((ushort)ClientWorldPacketId.CwServerState, MakePacket<CwServerState>);
		handler.Add((ushort)ClientWorldPacketId.CwServerState, packetHandler.CwServerStateHandler);		
		onRecv.Add((ushort)ClientWorldPacketId.CwEnterWorld, MakePacket<CwEnterWorld>);
		handler.Add((ushort)ClientWorldPacketId.CwEnterWorld, packetHandler.CwEnterWorldHandler);
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

        pkt.MergeFrom(buffer.Span.Slice(4)); // Skip 4바이트 (size + id)

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