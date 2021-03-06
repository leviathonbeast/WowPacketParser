using WowPacketParser.Enums;
using WowPacketParser.Hotfix;

namespace WowPacketParserModule.V9_0_1_36216.Hotfix
{
    [HotfixStructure(DB2Hash.Achievement)]
    public class AchievementEntry
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public string Reward { get; set; }
        public uint ID { get; set; }
        public short InstanceID { get; set; }
        public sbyte Faction { get; set; }
        public short Supercedes { get; set; }
        public short Category { get; set; }
        public sbyte MinimumCriteria { get; set; }
        public sbyte Points { get; set; }
        public int Flags { get; set; }
        public short UiOrder { get; set; }
        public int IconFileID { get; set; }
        public int RewardItemID { get; set; }
        public uint CriteriaTree { get; set; }
        public short SharesCriteria { get; set; }
        public int CovenantID { get; set; }
    }
}
