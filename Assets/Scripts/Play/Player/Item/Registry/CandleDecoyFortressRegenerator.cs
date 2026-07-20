using DecoyFortress;

namespace Player.Item
{
    public class CandleDecoyFortressRegenerator : DecoyFortressRegenerator
    {
        public override string Id
        {
            get { return "candle_decoy_fortress_regenerator"; }
        }

        public override string Name
        {
            get { return "Candle Decoy Fortress Regenerator"; }
        }

        public override DecoyFortressSetting.DecoyFortressIDs DecoyFortressID
        {
            get { return DecoyFortressSetting.DecoyFortressIDs.Candle; }
        }

        public override void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            base.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);
        }
    }
}
