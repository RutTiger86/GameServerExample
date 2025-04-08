namespace Server.Core.Interface
{
    public interface IPacketManager
    {
        public void OnRecvPacket(ISession session, ReadOnlyMemory<byte> buffer);
    }

}
