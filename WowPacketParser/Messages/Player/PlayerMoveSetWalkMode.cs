using WowPacketParser.Messages.Cli;

namespace WowPacketParser.Messages.Player
{
    public unsafe struct PlayerMoveSetWalkMode
    {
        public CliMovementStatus Status;
    }
}