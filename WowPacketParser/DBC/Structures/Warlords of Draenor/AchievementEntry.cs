﻿using DBFilesClient.NET;

namespace WowPacketParser.DBC.Structures
{
    [DBFileName("Achievement")]

    public sealed class AchievementEntry
    {
        public string Title;
        public string Description;
        public uint Flags;
        public string Reward;
        public short MapID;
        public ushort Supercedes;
        public ushort Category;
        public ushort UIOrder;
        public ushort IconID;
        public ushort SharesCriteria;
        public ushort CriteriaTree;
        public sbyte Faction;
        public byte Points;
        public byte MinimumCriteria;
        public uint ID;
    }
}
