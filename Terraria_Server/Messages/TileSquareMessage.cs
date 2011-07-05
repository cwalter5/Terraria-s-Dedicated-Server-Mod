﻿using System;

namespace Terraria_Server.Messages
{
    public class TileSquareMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short size = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int top = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;
            for (int x = left; x < left + (int)size; x++)
            {
                for (int y = top; y < top + (int)size; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                    }
                    Tile tile = Main.tile[x, y];

                    byte bitFlags = readBuffer[num++];

                    bool wasActive = tile.Active;

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
                        int wasType = (int)tile.type;
                        tile.type = readBuffer[num++];
                        if (Main.tileFrameImportant[(int)tile.type])
                        {
                            tile.frameX = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                            tile.frameY = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                        }
                        else if (!wasActive || (int)tile.type != wasType)
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
                        tile.lava = (readBuffer[num++] == 1);
                    }
                }
            }

            WorldGen.RangeFrame(left, top, left + (int)size, top + (int)size);
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
            }
        }
    }
}
