using Google.Protobuf;
using Server.Core;
using System;
using System.Collections.Generic;
using Server.Core.Interface;
using Server.Utill;
using log4net;
using AuthServer.Packets;
using Server.Data.AuthDb;


public class AuthDbPacketManager : IPacketManager
{
    private readonly ILog log;
	private readonly AuthDbPacketHandler packetHandler;

	public AuthDbPacketManager(ILogFactory logFactory, AuthDbPacketHandler packetHandler)
	{
		log = logFactory.CreateLogger<AuthDbPacketManager>();
		this.packetHandler = packetHandler;
		Register();
	}

	private Dictionary<ushort, Action<PacketSession, ReadOnlyMemory<byte>, ushort>> onRecv = [];
	private Dictionary<ushort, Action<PacketSession, IMessage>> handler = [];

	public Action<PacketSession, IMessage, ushort>? CustomHandler;

	private void Register()
	{
				
		onRecv.Add((ushort)AuthDbPacketId.DaServerState, MakePacket<DaServerState>);
		handler.Add((ushort)AuthDbPacketId.DaServerState, packetHandler.DaServerStateHandler);		
		onRecv.Add((ushort)AuthDbPacketId.DaGetAccountVerifyInfo, MakePacket<DaGetAccountVerifyInfo>);
		handler.Add((ushort)AuthDbPacketId.DaGetAccountVerifyInfo, packetHandler.DaGetAccountVerifyInfoHandler);
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
		if (onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	private void MakePacket<T>(PacketSession session, ReadOnlyMemory<byte> buffer, ushort id) where T : IMessage, new()
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
            if (handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }		
	}
}