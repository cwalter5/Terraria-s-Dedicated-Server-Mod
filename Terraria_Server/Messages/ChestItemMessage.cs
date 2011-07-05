using System;
using System.Text;
using Terraria_Server.Shops;

namespace Terraria_Server.Messages
{
    public class ChestItemMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.CHEST_ITEM;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int chestIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int contentsIndex = (int)readBuffer[num++];
            int stackSize = (int)readBuffer[num++];

            string itemName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
            if (Main.chest[chestIndex] == null)
            {
                Main.chest[chestIndex] = new Chest();
            }
            
            Chest chest = Main.chest[chestIndex];
            if (chest.contents[contentsIndex] == null)
            {
                chest.contents[contentsIndex] = new Item();
            }

            chest.contents[contentsIndex].SetDefaults(itemName);
            chest.contents[contentsIndex].Stack = stackSize;
        }
    }
}
