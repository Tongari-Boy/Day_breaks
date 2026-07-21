using DecoyFortress;
using UnityEngine;

namespace Player.Item
{
    public class BombDecoyFortressRegenerator : DecoyFortressRegenerator
    {
        public override string Id
        {
            get { return "bomb_decoy_fortress_regenerator"; }
        }

        public override string Name
        {
            get { return "Bomb Decoy Fortress Regenerator"; }
        }

        public override DecoyFortressSetting.DecoyFortressIDs DecoyFortressID
        {
            get { return DecoyFortressSetting.DecoyFortressIDs.Bomb; }
        }

        public override void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            base.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);
        }
    }
}
