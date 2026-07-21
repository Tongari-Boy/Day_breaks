using DecoyFortress;

namespace Player.Item
{
    public class SwordDecoyFortressRegenerator : DecoyFortressRegenerator
    {
        public override string Id
        {
            get { return "sword_decoy_fortress_regenerator"; }
        }

        public override string Name
        {
            get { return "Sword Decoy Fortress Regenerator"; }
        }

        public override DecoyFortressSetting.DecoyFortressIDs DecoyFortressID
        {
            get { return DecoyFortressSetting.DecoyFortressIDs.Sword; }
        }

        public override void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            base.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);
        }
    }
}
