using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class WorldDataMessage : IMessage
    {
        private const int ORB_SMASHED_BIT = 1;
        private const int BOSS1_BIT = 2;
        private const int BOSS2_BIT = 4;
        private const int BOSS3_BIT = 8;

        public Packet GetPacket()
        {
            return Packet.WORLD_DATA;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            Main.time = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;

            Main.dayTime = (readBuffer[num++] == 1);
            Main.moonPhase = (int)readBuffer[num++];
            Main.bloodMoon = ((int)readBuffer[num++] == 1);

            Main.maxTilesX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.maxTilesY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.spawnTileX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.spawnTileY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.worldSurface = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.rockLayer = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.worldID = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte bitFlags = readBuffer[num++];
            if ((bitFlags & ORB_SMASHED_BIT) == ORB_SMASHED_BIT)
            {
                WorldGen.shadowOrbSmashed = true;
            }
            if ((bitFlags & BOSS1_BIT) == BOSS1_BIT)
            {
                NPC.downedBoss1 = true;
            }
            if ((bitFlags & BOSS2_BIT) == BOSS2_BIT)
            {
                NPC.downedBoss2 = true;
            }

            if ((bitFlags & BOSS3_BIT) == BOSS3_BIT)
            {
                NPC.downedBoss3 = true;
            }

            Main.worldName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            if (Netplay.clientSock.state == 3)
            {
                Netplay.clientSock.state = 4;
            }
        }
    }
}
