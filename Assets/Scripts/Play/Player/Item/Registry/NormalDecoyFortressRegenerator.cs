using DecoyFortress;

namespace Player.Item
{
    public class NormalDecoyFortressRegenerator : DecoyFortressRegenerator
    {
        public override string Id
        {
            get { return "normal_decoy_fortress_regenerator"; }
        }

        public override string Name
        {
            get { return "Normal Decoy Fortress Regenerator"; }
        }

        public override DecoyFortressSetting.DecoyFortressIDs DecoyFortressID
        {
            get { return DecoyFortressSetting.DecoyFortressIDs.Normal; }
        }

        public override void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            base.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);
        }
    }
}