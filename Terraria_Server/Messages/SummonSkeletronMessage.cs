using System;

namespace Terraria_Server.Messages
{
    public class SummonSkeletronMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SUMMON_SKELETRON;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (readBuffer[num] == 1)
            {
                NPC.SpawnSkeletron();
            }
        }
    }
}
