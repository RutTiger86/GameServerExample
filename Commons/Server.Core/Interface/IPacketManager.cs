namespace Server.Core.Interface
{
    public  interface IPacketManager
    {
        public void OnRecvPacket(PacketSession session, ReadOnlyMemory<byte> buffer);
    }
}
