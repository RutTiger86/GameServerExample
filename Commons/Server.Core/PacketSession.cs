using Server.Core.Interface;

namespace Server.Core
{
    public abstract class PacketSession : Session , IPacketSession
    {
        public static readonly int HeaderSize = 4; // size(2) + packetId(2)
        
        // [size(2)][packetId(2)][ ... ]
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            if (buffer.Array == null)
                return 0;

            Span<byte> span = buffer.AsSpan();

            while (true)
            {
                if (span.Length < HeaderSize)
                    break;

                // 패킷 사이즈와 패킷 ID 추출
                ushort size = BitConverter.ToUInt16(span.Slice(0, 2));
                ushort packetId = BitConverter.ToUInt16(span.Slice(2, 2));

                // 완성된 패킷인지 확인
                if (span.Length < size)
                    break;

                // [Header + Body] 전체 패킷 전달 (Span 유지)
                var packetBuffer = new ReadOnlyMemory<byte>(buffer.Array, buffer.Offset + processLen, size);
                OnRecvPacket(packetBuffer);

                processLen += size;
                span = span.Slice(size); // 남은 데이터로 이동
            }

            return processLen;
        }

        // Span 기반 패킷 처리 (Zero-Copy 가능)
        public abstract void OnRecvPacket(ReadOnlyMemory<byte> buffer);
    }

}
