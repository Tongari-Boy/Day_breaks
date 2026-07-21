using DecoyFortress;

namespace Player.Item
{
    public class StopDecoyFortressRegenerator : DecoyFortressRegenerator
    {
        public override string Id
        {
            get { return "stop_decoy_fortress_regenerator"; }
        }

        public override string Name
        {
            get { return "Stop Decoy Fortress Regenerator"; }
        }

        public override DecoyFortressSetting.DecoyFortressIDs DecoyFortressID
        {
            get { return DecoyFortressSetting.DecoyFortressIDs.Stop; }
        }

        public override void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            base.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);
        }
    }
}