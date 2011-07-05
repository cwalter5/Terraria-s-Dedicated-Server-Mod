using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class SendTileLoadingMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_TILE_LOADING_MESSAGE;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short width = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int y = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;

            for (int x = left; x < left + (int)width; x++)
            {
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }

                Tile tile = Main.tile[x, y];

                byte bitFlags = readBuffer[num++];
                bool active = tile.Active;
                tile.Active = ((bitFlags & Tile.ACTIVE_BIT) == Tile.ACTIVE_BIT);

                if ((bitFlags & Tile.LIGHT_BIT) == Tile.LIGHT_BIT)
                {
                    tile.lighted = true;
                }

                if ((bitFlags & Tile.WALL_BIT) == Tile.WALL_BIT)
                {
                    tile.wall = 1;
                }
                else
                {
                    tile.wall = 0;
                }

                if ((bitFlags & Tile.LIQUID_BIT) == Tile.LIQUID_BIT)
                {
                    tile.liquid = 1;
                }
                else
                {
                    tile.liquid = 0;
                }

                if (tile.Active)
                {
                    int type = (int)tile.type;
                    tile.type = readBuffer[num++];

                    if (Main.tileFrameImportant[(int)tile.type])
                    {
                        tile.frameX = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                        tile.frameY = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                    }
                    else if (!active || (int)tile.type != type)
                    {
                        tile.frameX = -1;
                        tile.frameY = -1;
                    }
                }

                if (tile.wall > 0)
                {
                    tile.wall = readBuffer[num++];
                }

                if (tile.liquid > 0)
                {
                    tile.liquid = readBuffer[num++];
                    byte lavaFlag = readBuffer[num++];
                    tile.lava = (lavaFlag == 1);
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)width, (float)left, (float)y);
            }
        }
    }
}
