using DecoyFortress;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// DecoyFortressを復活させるアイテム
    /// </summary>
    public class DecoyFortressRegenerator : IPlayerItem
    {
        public string Id
        {
            get { return "decoy_fortress_regenerator"; }
        }

        public string Name
        {
            get { return "Decoy Fortress Regenerator"; }
        }

        public bool CanUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            return playerBehaviour.InteractingDecoyFortresses.Count > 0;
        }

        public bool DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            // PlayerBehaviourが検知しているDecoyFortressを取得
            foreach (DecoyFortressSetting decoyFortressSetting in playerBehaviour.InteractingDecoyFortresses)
            {
                if (decoyFortressSetting != null && decoyFortressSetting.enabled && !decoyFortressSetting.GetEnable())
                {
                    // DecoyFortressを復活させる
                    decoyFortressSetting.Build();

                    Debug.Log($"Decoy Fortressが復活しました！（ID: {decoyFortressSetting.GetInstanceID()}）");

                    return true;
                }
            }

            return false;
        }
    }
}