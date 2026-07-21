using DecoyFortress;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// DecoyFortressを復活させるアイテム
    /// </summary>
    public abstract class DecoyFortressRegenerator : IPlayerItem
    {
        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract DecoyFortressSetting.DecoyFortressIDs DecoyFortressID { get; }

        public bool CanUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            return playerBehaviour.InteractingDecoyFortresses.Count > 0;
        }

        public bool DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            // PlayerBehaviourが検知しているDecoyFortressを取得
            foreach (DecoyFortressSetting decoyFortressSetting in playerBehaviour.InteractingDecoyFortresses)
            {
                if (decoyFortressSetting != null && decoyFortressSetting.enabled && !decoyFortressSetting.GetEnable() && decoyFortressSetting.GetID() == this.DecoyFortressID)
                {
                    this.DoUse(playerItemState, playerBehaviour, decoyFortressSetting);

                    return true;
                }
            }

            return false;
        }

        public virtual void DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour, DecoyFortressSetting decoyFortressSetting)
        {
            // DecoyFortressを復活させる
            decoyFortressSetting.Build();

            Debug.Log($"Decoy Fortressが復活しました！（ID: {this.DecoyFortressID}, Instance: {decoyFortressSetting.GetInstanceID()}）");
        }
    }
}