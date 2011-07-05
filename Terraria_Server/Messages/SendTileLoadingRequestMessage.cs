using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class SendTileLoadingRequestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_TILE_LOADING;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            Netplay.clientSock.statusMax += BitConverter.ToInt32(readBuffer, start + 1);
            Netplay.clientSock.statusText = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5);
        }
    }
}
