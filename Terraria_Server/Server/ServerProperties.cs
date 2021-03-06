﻿using System;
using System.IO;
using Terraria_Server.Misc;

namespace Terraria_Server
{
    public class ServerProperties : PropertiesFile
    {
        private const bool DEFAULT_AUTOMATIC_UPDATES = true;
        private const bool DEFAULT_DEBUG_MODE = false;
        private const String DEFAULT_GREETING = "Welcome to a TDSM Server!";
        private const String DEFAULT_MAP_SIZE = "small";
        private const int DEFAULT_MAX_PLAYERS = 8;
        private const bool DEFAULT_NPC_DOOR_OPEN_CANCEL = false;
        private const int DEFAULT_PORT = 7777;
        private const int DEFAULT_SEED = -1;
        private const String DEFAULT_SERVER_IP = "127.0.0.1";
        private const bool DEFAULT_USE_CUSTOM_TILES = false;
        private const bool DEFAULT_USE_CUSTOM_GEN_OPTS = false;
        private const bool DEFAULT_WHITE_LIST = false;
        private const String DEFAULT_WORLD = "world1.wld";

        private const String AUTOMATIC_UPDATES = "allowupdates";
        private const String DEBUG_MODE = "debugmode";
        private const String DUNGEON_AMOUNT = "opt-numdungeons";
        private const String FLOATING_ISLAND_AMOUNT = "opt-num-floating-islands";
        private const String GREETING = "greeting";
        private const String MAP_SIZE = "opt-mapsize";
        private const String MAX_PLAYERS = "maxplayers";
        private const String MAX_TILES_X = "opt-maxtilesx";
        private const String MAX_TILES_Y = "opt-maxtilesy";
        private const String NPC_DOOR_OPEN_CANCEL = "npc-cancelopendoor";
        private const String PASSWORD = "server-password";
        private const String PORT = "port";
        private const String SEED = "opt-seed";
        private const String SERVER_IP = "serverip";
        private const String USE_CUSTOM_TILES = "opt-usecustomtiles";
        private const String USE_CUSTOM_GEN_OPTS = "opt-custom-worldgen";
        private const String WHITE_LIST = "whitelist";
        private const String WORLD_PATH = "worldpath";

        public ServerProperties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = MaxPlayers;
            temp = ServerIP;
            temp = Port;
            temp = Greeting;
            temp = InitialWorldPath;
            temp = Password;
            temp = AutomaticUpdates;
            temp = NPCDoorOpenCancel;
            temp = Seed;
            temp = MapSize;
            temp = UseCustomTiles;
            temp = MaxTilesX;
            temp = MaxTilesY;
            temp = UseCustomGenOpts;
            temp = DungeonAmount;
            temp = FloatingIslandAmount;
            temp = DebugMode;
        }

        public int MaxPlayers
        {
            get
            {
                return getValue(MAX_PLAYERS, DEFAULT_MAX_PLAYERS);
            }
        }

        public int Port
        {
            get
            {
                return getValue(PORT, DEFAULT_PORT);
            }
        }

        public string Greeting
        {
            get
            {
                return getValue(GREETING, DEFAULT_GREETING);
            }
        }

        public string ServerIP
        {
            get
            {
                return getValue(SERVER_IP, DEFAULT_SERVER_IP);
            }
        }


        public string InitialWorldPath
        {
            get
            {
                return getValue(WORLD_PATH, Statics.WorldPath + Path.DirectorySeparatorChar + DEFAULT_WORLD);
            }
        }

        public string Password
        {
            get
            {
                return getValue(PASSWORD, String.Empty);
            }
        }

        public int Seed
        {
            get
            {
                return getValue(SEED, DEFAULT_SEED);
            }
        }

        public int MaxTilesX
        {
            get
            {
                return getValue(MAX_TILES_X, World.MAP_SIZE.SMALL_X);
            }
        }

        public int MaxTilesY
        {
            get
            {
                return getValue(MAX_TILES_Y, World.MAP_SIZE.SMALL_Y);
            }
        }

        private int getValue(String key, World.MAP_SIZE mapSize)
        {
            return getValue(key, (int)mapSize);
        }

        public bool UseCustomTiles
        {
            get
            {
                return getValue(USE_CUSTOM_TILES, DEFAULT_USE_CUSTOM_TILES);
            }
        }

        public int[] getMapSizes()
        {
            string CustomTiles = base.getValue(MAP_SIZE);

            if (CustomTiles == null)
            {
                return null;
            }
            switch (CustomTiles.Trim().ToLower())
            {
                case "small":
                    {
                        return new int[] { (int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y };
                    }
                case "medium":
                    {
                        return new int[] { (int)World.MAP_SIZE.MEDIUM_X, (int)World.MAP_SIZE.MEDIUM_Y };
                    }
                case "large":
                    {
                        return new int[] { (int)World.MAP_SIZE.LARGE_X, (int)World.MAP_SIZE.LARGE_Y };
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public string MapSize
        {
            get
            {
                string CustomTiles = getValue(MAP_SIZE, DEFAULT_MAP_SIZE);
                if (CustomTiles == null)
                {
                    return "small";
                }
                switch (CustomTiles.Trim().ToLower())
                {
                    case "small":
                        {
                            return "small";
                        }
                    case "medium":
                        {
                            return "medium";
                        }
                    case "large":
                        {
                            return "large";
                        }
                    default:
                        {
                            return "small";
                        }
                }
            }
        }

        public bool UseWhiteList
        {
            get
            {
                return getValue(WHITE_LIST, DEFAULT_WHITE_LIST);
            }
        }

        public bool NPCDoorOpenCancel
        {
            get
            {
                return getValue(NPC_DOOR_OPEN_CANCEL, DEFAULT_NPC_DOOR_OPEN_CANCEL);
            }
        }

        public int DungeonAmount
        {
            get
            {
                int amount = getValue(DUNGEON_AMOUNT, -1);
                if(amount <= 0)
                {
                    amount = 1;
                    setValue(DUNGEON_AMOUNT, amount);
                }
                else if(amount > 10)
                {
                    amount = 10;
                    setValue(DUNGEON_AMOUNT, amount);
                }
                return amount;
            }
        }

        public bool UseCustomGenOpts
        {
            get
            {
                return getValue(USE_CUSTOM_GEN_OPTS, DEFAULT_USE_CUSTOM_GEN_OPTS);
            }
        }

        public int FloatingIslandAmount
        {
            get
            {
                int amount = getValue(FLOATING_ISLAND_AMOUNT, -1);
                if (amount <= 0)
                {
                    amount = (int)((double)Main.maxTilesX * 0.0008);
                    setValue(FLOATING_ISLAND_AMOUNT, amount);
                }
                else if (amount > (int)((double)Main.maxTilesX * 0.0008) * 3)
                {
                    amount = (int)((double)Main.maxTilesX * 0.0008) * 3;
                    setValue(FLOATING_ISLAND_AMOUNT, amount);
                }
                return amount;
            }
        }

        public bool DebugMode
        {
            get
            {
                return getValue(DEBUG_MODE, DEFAULT_DEBUG_MODE);
            }
        }

        public bool AutomaticUpdates
        {
            get
            {
                return getValue(AUTOMATIC_UPDATES, DEFAULT_AUTOMATIC_UPDATES);
            }
        }
    }
}