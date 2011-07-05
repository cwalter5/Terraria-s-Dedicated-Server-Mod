﻿using System;

namespace Terraria_Server.Messages
{
    public class PlayerManaUpdateMessage : IMessage
    {

        public Packet GetPacket()
        {
            return Packet.PLAYER_MANA_UPDATE;
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

            int statMana = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int statManaMax = (int)BitConverter.ToInt16(readBuffer, num);

            Player player = Main.players[playerIndex];
            player.statMana = statMana;
            player.statManaMax = statManaMax;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(42, -1, whoAmI, "", playerIndex);
            }
        }
    }
}
