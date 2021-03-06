using WowPacketParser.Enums;
using WowPacketParser.Hotfix;

namespace WowPacketParserModule.V9_0_1_36216.Hotfix
{
    [HotfixStructure(DB2Hash.RewardPackXCurrencyType, HasIndexInData = false)]
    public class RewardPackXCurrencyTypeEntry
    {
        public uint CurrencyTypeID { get; set; }
        public int Quantity { get; set; }
        public uint RewardPackID { get; set; }
    }
}
