using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class ItemInfoMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ITEM_INFO;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float x = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vX = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vY = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            byte stackSize = readBuffer[num++];

            string itemName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            Item item = Main.item[(int)itemIndex];
            if (Main.netMode == 1)
            {
                if (itemName == "0")
                {
                    item.Active = false;
                    return;
                }

                ItemSetup(item, itemName, stackSize, x, y, vX, vY);
                item.Wet = Collision.WetCollision(item.Position, item.Width, item.Height);
            }
            else if (itemName == "0")
            {
                if (itemIndex < 200)
                {
                    item.Active = false;
                    NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                }
            }
            else
            {
                bool isNewItem = false;
                if (itemIndex == 200)
                {
                    isNewItem = true;
                    Item newItem = new Item();
                    newItem.SetDefaults(itemName);
                    itemIndex = (short)Item.NewItem((int)x, (int)y, newItem.Width, newItem.Height, newItem.Type, (int)stackSize, true);
                    item = Main.item[(int)itemIndex];
                }

                ItemSetup(item, itemName, stackSize, x, y, vX, vY);
                item.Owner = Main.myPlayer;

                if (isNewItem)
                {
                    NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                    item.OwnIgnore = whoAmI;
                    item.OwnTime = 100;
                    item.FindOwner((int)itemIndex);
                    return;
                }
                NetMessage.SendData(21, -1, whoAmI, "", (int)itemIndex);
            }
        }

        private void ItemSetup(Item item, String itemName, int stackSize, float x, float y, float vX, float vY)
        {
            item.SetDefaults(itemName);
            item.Stack = (int)stackSize;
            item.Position.X = x;
            item.Position.Y = y;
            item.Velocity.X = vX;
            item.Velocity.Y = vY;
            item.Active = true;
        }
    }
}