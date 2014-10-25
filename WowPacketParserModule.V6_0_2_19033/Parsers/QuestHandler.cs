using System;
using System.Collections.Generic;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParserModule.V6_0_2_19033.Parsers
{
    public static class QuestHandler
    {
        [Parser(Opcode.CMSG_QUEST_QUERY)]
        public static void HandleQuestQuery(Packet packet)
        {
            packet.ReadInt32("Entry");
            packet.ReadPackedGuid128("Guid");
        }

        [Parser(Opcode.CMSG_QUESTGIVER_QUERY_QUEST)]
        public static void HandleQuestGiverQueryQuest(Packet packet)
        {
            packet.ReadPackedGuid128("QuestGiverGUID");
            packet.ReadUInt32("QuestID");
            packet.ReadBit("RespondToGiver");
        }

        [Parser(Opcode.CMSG_QUESTGIVER_COMPLETE_QUEST)]
        public static void HandleQuestGiverCompleteQuest(Packet packet)
        {
            packet.ReadPackedGuid128("QuestGiverGUID");
            packet.ReadUInt32("QuestID");
            packet.ReadBit("FromScript");
        }

        [Parser(Opcode.CMSG_QUEST_NPC_QUERY)]
        public static void HandleQuestNpcQuery(Packet packet)
        {
            packet.ReadUInt32("Count");

            for (var i = 0; i < 50; i++)
                packet.ReadEntry<Int32>(StoreNameType.Quest, "Quest ID", i);
        }

        [Parser(Opcode.SMSG_QUEST_POI_QUERY_RESPONSE)]
        public static void HandleQuestPoiQueryResponse(Packet packet)
        {
            packet.ReadInt32("NumPOIs");
            var int4 = packet.ReadInt32("QuestPOIData");

            for (var i = 0; i < int4; ++i)
            {
                var questId = packet.ReadUInt32("QuestID", i);
                packet.ReadUInt32("NumBlobs", i);

                var int2 = packet.ReadInt32("QuestPOIBlobData", i);

                for (var j = 0; j < int2; ++j)
                {
                    var questPoi = new QuestPOI();
                    var idx = packet.ReadInt32("BlobIndex", i, j);
                    questPoi.ObjectiveIndex = packet.ReadInt32("ObjectiveIndex", i, j);
                    packet.ReadInt32("QuestObjectiveID", i, j);
                    packet.ReadInt32("QuestObjectID", i, j);
                    questPoi.Map = (uint)packet.ReadInt32("MapID", i, j);
                    questPoi.WorldMapAreaId = (uint)packet.ReadInt32("WorldMapAreaID", i, j);
                    questPoi.FloorId = (uint)packet.ReadInt32("Floor", i, j);
                    packet.ReadInt32("Priority", i, j);
                    packet.ReadInt32("Flags", i, j);
                    packet.ReadInt32("WorldEffectID", i, j);
                    packet.ReadInt32("PlayerConditionID", i, j);
                    packet.ReadInt32("NumPoints", i, j);
                    packet.ReadInt32("Int12", i, j);

                    var int13 = packet.ReadInt32("QuestPOIBlobPoint", i, j);
                    questPoi.Points = new List<QuestPOIPoint>(int13);
                    for (var k = 0u; k < int13; ++k)
                    {
                        var questPoiPoint = new QuestPOIPoint
                        {
                            Index = k,
                            X = packet.ReadInt32("X", i, j, (int)k),
                            Y = packet.ReadInt32("Y", i, j, (int)k)
                        };
                        questPoi.Points.Add(questPoiPoint);
                    }

                    Storage.QuestPOIs.Add(new Tuple<uint, uint>((uint)questId, (uint)idx), questPoi, packet.TimeSpan);
                }
            }
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_QUEST_QUERY_RESPONSE)]
        public static void HandleQuestQueryResponse(Packet packet)
        {
            packet.ReadInt32("Entry");

            var hasData = packet.ReadBit("Has Data");
            if (!hasData)
                return; // nothing to do

            var quest = new QuestTemplateWod();

            var id = packet.ReadEntry("Quest ID");
            quest.QuestType = packet.ReadEnum<QuestMethod>("QuestType", TypeCode.Int32);
            quest.QuestLevel = packet.ReadInt32("QuestLevel");
            quest.QuestPackageID = packet.ReadInt32("QuestPackageID");
            quest.MinLevel = packet.ReadInt32("QuestMinLevel");
            quest.QuestSortID = (QuestSort)packet.ReadInt32("QuestSortID");
            quest.QuestInfoID = packet.ReadEnum<QuestType>("QuestInfoID", TypeCode.Int32);
            quest.SuggestedGroupNum = packet.ReadInt32("SuggestedGroupNum");
            quest.RewardNextQuest = packet.ReadInt32("RewardNextQuest");
            quest.RewardXPDifficulty = packet.ReadInt32("RewardXPDifficulty");

            quest.Float10 = packet.ReadSingle("Float10");

            quest.RewardOrRequiredMoney = packet.ReadInt32("RewardMoney");
            quest.RewardMoneyMaxLevel = packet.ReadInt32("RewardMoneyDifficulty");

            quest.Float13 = packet.ReadSingle("Float13");

            quest.RewardBonusMoney = packet.ReadInt32("RewardBonusMoney");
            quest.RewardDisplaySpell = packet.ReadInt32("RewardDisplaySpell");
            quest.RewardSpell = packet.ReadInt32("RewardSpell");
            quest.RewardHonor = packet.ReadInt32("RewardHonor");

            quest.RewardKillHonor = packet.ReadSingle("RewardKillHonor");

            quest.StartItem = packet.ReadInt32("StartItem");
            quest.Flags = packet.ReadEnum<QuestFlags>("Flags", TypeCode.UInt32);
            quest.FlagsEx = packet.ReadInt32("FlagsEx");

            quest.RewardItems = new int[4];
            quest.RewardAmount = new int[4];
            quest.ItemDrop = new int[4];
            quest.ItemDropQuantity = new int[4];
            for (var i = 0; i < 4; ++i)
            {
                quest.RewardItems[i] = packet.ReadInt32("RewardItems", i);
                quest.RewardAmount[i] = packet.ReadInt32("RewardAmount", i);
                quest.ItemDrop[i] = packet.ReadInt32("ItemDrop", i);
                quest.ItemDropQuantity[i] = packet.ReadInt32("ItemDropQuantity", i);
            }

            quest.ItemID = new int[6];
            quest.Quantity = new int[6];
            quest.DisplayID = new int[6];
            for (var i = 0; i < 6; ++i) // CliQuestInfoChoiceItem
            {
                quest.ItemID[i] = packet.ReadInt32("ItemID", i);
                quest.Quantity[i] = packet.ReadInt32("Quantity", i);
                quest.DisplayID[i] = packet.ReadInt32("DisplayID", i);
            }

            quest.POIContinent = packet.ReadInt32("POIContinent");

            quest.POIx = packet.ReadSingle("POIx");
            quest.POIy = packet.ReadSingle("POIy");

            quest.POIPriority = packet.ReadInt32("POIPriority");
            quest.RewardTitle = packet.ReadInt32("RewardTitle");
            quest.RewardTalents = packet.ReadInt32("RewardTalents");
            quest.RewardArenaPoints = packet.ReadInt32("RewardArenaPoints");
            quest.RewardSkillLineID = packet.ReadInt32("RewardSkillLineID");
            quest.RewardNumSkillUps = packet.ReadInt32("RewardNumSkillUps");
            quest.PortraitGiver = packet.ReadInt32("PortraitGiver");
            quest.PortraitTurnIn = packet.ReadInt32("PortraitTurnIn");

            quest.RewardFactionID = new int[5];
            quest.RewardFactionValue = new int[5];
            quest.RewardFactionOverride = new int[5];
            for (var i = 0; i < 5; ++i)
            {
                quest.RewardFactionID[i] = packet.ReadInt32("RewardFactionID", i);
                quest.RewardFactionValue[i] = packet.ReadInt32("RewardFactionValue", i);
                quest.RewardFactionOverride[i] = packet.ReadInt32("RewardFactionOverride", i);
            }

            quest.RewardFactionFlags = packet.ReadInt32("RewardFactionFlags");

            quest.RewardCurrencyID = new int[4];
            quest.RewardCurrencyQty = new int[4];
            for (var i = 0; i < 4; ++i)
            {
                quest.RewardCurrencyID[i] = packet.ReadInt32("RewardCurrencyID");
                quest.RewardCurrencyQty[i] = packet.ReadInt32("RewardCurrencyQty");
            }

            quest.AcceptedSoundKitID = packet.ReadInt32("AcceptedSoundKitID");
            quest.CompleteSoundKitID = packet.ReadInt32("CompleteSoundKitID");
            quest.AreaGroupID = packet.ReadInt32("AreaGroupID");
            quest.TimeAllowed = packet.ReadInt32("TimeAllowed");
            var int2946 = packet.ReadInt32("CliQuestInfoObjective");
            quest.Int2950 = packet.ReadInt32("Int2950");

            for (var i = 0; i < int2946; ++i)
            {
                packet.ReadInt32("Id", i);
                packet.ReadByte("Type", i);
                packet.ReadByte("StorageIndex", i);
                packet.ReadInt32("ObjectID", i);
                packet.ReadInt32("Amount", i);
                packet.ReadInt32("Flags", i);
                packet.ReadSingle("Float5", i);
                var int280 = packet.ReadInt32("VisualEffects", i);
                for (var j = 0; j < int280; ++j)
                    packet.ReadInt32("VisualEffectId", i, j);

                packet.ResetBitReader();

                var bits6 = packet.ReadBits(8);
                packet.ReadWoWString("Description", bits6, i);
            }

            packet.ResetBitReader();

            var bits26 = packet.ReadBits(9);
            var bits154 = packet.ReadBits(12);
            var bits904 = packet.ReadBits(12);
            var bits1654 = packet.ReadBits(9);
            var bits1789 = packet.ReadBits(10);
            var bits2045 = packet.ReadBits(8);
            var bits2109 = packet.ReadBits(10);
            var bits2365 = packet.ReadBits(8);
            var bits2429 = packet.ReadBits(11);

            quest.LogTitle = packet.ReadWoWString("LogTitle", bits26);
            quest.LogDescription = packet.ReadWoWString("LogDescription", bits154);
            quest.QuestDescription = packet.ReadWoWString("QuestDescription", bits904);
            quest.AreaDescription = packet.ReadWoWString("AreaDescription", bits1654);
            quest.PortraitGiverText = packet.ReadWoWString("PortraitGiverText", bits1789);
            quest.PortraitGiverName = packet.ReadWoWString("PortraitGiverName", bits2045);
            quest.PortraitTurnInText = packet.ReadWoWString("PortraitTurnInText", bits2109);
            quest.PortraitTurnInName = packet.ReadWoWString("PortraitTurnInName", bits2365);
            quest.QuestCompletionLog = packet.ReadWoWString("QuestCompletionLog", bits2429);

            Storage.QuestTemplatesWod.Add((uint)id.Key, quest, packet.TimeSpan);
        }

        [Parser(Opcode.CMSG_QUEST_POI_QUERY)]
        public static void HandleQuestPoiQuery(Packet packet)
        {
            var count = packet.ReadUInt32("Count");

            for (var i = 0; i < count; i++)
                packet.ReadEntry<Int32>(StoreNameType.Quest, "Quest ID", i);
        }
    }
}
