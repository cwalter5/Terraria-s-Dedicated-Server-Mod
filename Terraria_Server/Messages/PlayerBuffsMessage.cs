﻿using System;

namespace Terraria_Server.Messages
{
    public class PlayerBuffsMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_BUFFS;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }
            else if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.players[playerIndex];
            for (int i = 0; i < Player.MAX_BUFF; i++)
            {
                player.buffType[i] = (int)readBuffer[num++];
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(50, -1, whoAmI, "", playerIndex);
            }
        }
    }
}
