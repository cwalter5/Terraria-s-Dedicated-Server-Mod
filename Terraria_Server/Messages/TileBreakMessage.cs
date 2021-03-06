﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;

namespace Terraria_Server.Messages
{
    public class TileBreakMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.TILE_BREAK;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            byte tileAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte tileType = readBuffer[num++];
            int style = (int)readBuffer[num];
            bool failFlag = (tileType == 1);

            Tile tile = new Tile();

            if (Main.tile[x, y] != null)
            {
                tile = WorldGen.cloneTile(Main.tile[x, y]);
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }

            tile.tileX = x;
            tile.tileY = y;

            PlayerTileChangeEvent breakEvent = new PlayerTileChangeEvent();
            breakEvent.Sender = Main.players[whoAmI];
            breakEvent.Tile = tile;
            breakEvent.Type = tileType;
            breakEvent.Position = new Vector2(x, y);
            Program.server.getPluginManager().processHook(Hooks.TILE_CHANGE, breakEvent);
            if (breakEvent.Cancelled)
            {
                NetMessage.SendTileSquare(whoAmI, x, y, 1);
                return;
            }

            if (Main.netMode == 2)
            {
                if (!failFlag)
                {
                    if (tileAction == 0 || tileAction == 2 || tileAction == 4)
                    {
                        Netplay.serverSock[whoAmI].spamDelBlock += 1f;
                    }
                    else if (tileAction == 1 || tileAction == 3)
                    {
                        Netplay.serverSock[whoAmI].spamAddBlock += 1f;
                    }
                }

                if (!Netplay.serverSock[whoAmI].tileSection[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
                {
                    failFlag = true;
                }
            }

            switch (tileAction)
            {
                case 0:
                    WorldGen.KillTile(x, y, failFlag, false, false);
                    break;
                case 1:
                    WorldGen.PlaceTile(x, y, (int)tileType, false, true, -1, style);
                    break;
                case 2:
                    WorldGen.KillWall(x, y, failFlag);
                    break;
                case 3:
                    WorldGen.PlaceWall(x, y, (int)tileType, false);
                    break;
                case 4:
                    WorldGen.KillTile(x, y, failFlag, false, true);
                    break;
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(17, -1, whoAmI, "", (int)tileAction, (float)x, (float)y, (float)tileType);
                if (tileAction == 1 && tileType == 53)
                {
                    NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }
        }
    }
}
