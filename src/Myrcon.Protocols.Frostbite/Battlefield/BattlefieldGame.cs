using Procon.Net.Shared;

namespace Myrcon.Protocols.Frostbite.Battlefield {
    public abstract class BattlefieldGame : FrostbiteGame {

        public void ServerOnLevelLoadedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 5) {
                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Packet.Words[3], out currentRound) == true && int.TryParse(request.Packet.Words[4], out totalRounds) == true) {
                    this.UpdateSettingsMap(request.Packet.Words[1], request.Packet.Words[2]);
                    this.UpdateSettingsRound(currentRound, totalRounds);
                }
            }
        }
    }
}
