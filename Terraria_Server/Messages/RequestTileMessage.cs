using System;

namespace Terraria_Server.Messages
{
    public class RequestTileMessage : IMessage
    {
        private const int BOUNDS = 10;
        private const int DEFAULT_STATUS_MAX = 1350;

        public Packet GetPacket()
        {
            return Packet.REQUEST_TILE_BLOCK;
        }

        public int? GetRequiredNetMode()
        {
            return 2;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int left = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int top = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            bool outOfBounds = !(left == -1
                || left < BOUNDS
                || left > Main.maxTilesX - BOUNDS 
                || top == -1
                || top < BOUNDS
                || top > Main.maxTilesY - BOUNDS);
            
            int statusMax = DEFAULT_STATUS_MAX;
            if (outOfBounds)
            {
                statusMax *= 2;
            }

            ServerSock serverSock = Netplay.serverSock[whoAmI];
            if (serverSock.state == 2)
            {
                serverSock.state = 3;
            }

            NetMessage.SendData(9, whoAmI, -1, "Receiving tile data", statusMax);
            serverSock.statusText2 = "is receiving tile data";
            serverSock.statusMax += statusMax;
            int sectionX = Netplay.GetSectionX(Main.spawnTileX);
            int sectionY = Netplay.GetSectionY(Main.spawnTileY);

            for (int x = sectionX - 2; x < sectionX + 3; x++)
            {
                for (int y = sectionY - 1; y < sectionY + 2; y++)
                {
                    NetMessage.SendSection(whoAmI, x, y);
                }
            }

            if (outOfBounds)
            {
                left = Netplay.GetSectionX(left);
                top = Netplay.GetSectionY(top);
                for (int x = left - 2; x < left + 3; x++)
                {
                    for (int y = top - 1; y < top + 2; y++)
                    {
                        NetMessage.SendSection(whoAmI, x, y);
                    }
                }
                NetMessage.SendData(11, whoAmI, -1, "", left - 2, (float)(top - 1), (float)(left + 2), (float)(top + 1));
            }

            NetMessage.SendData(11, whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1));

            //Can't switch to a for each because there are 201 items.
            for (int itemIndex = 0; itemIndex < 200; itemIndex++)
            {
                if (Main.item[itemIndex].Active)
                {
                    NetMessage.SendData(21, whoAmI, -1, "", itemIndex);
                    NetMessage.SendData(22, whoAmI, -1, "", itemIndex);
                }
            }
            
            //Can't switch to a for each because there are 1001 NPCs.
            for (int npcIndex = 0; npcIndex < 1000; npcIndex++)
            {
                if (Main.npc[npcIndex].Active)
                {
                    NetMessage.SendData(23, whoAmI, -1, "", npcIndex);
                }
            }
            NetMessage.SendData(49, whoAmI);
        }
    }
}
