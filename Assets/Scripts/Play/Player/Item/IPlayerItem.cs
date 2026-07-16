using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// プレイヤーが使用するアイテムを定義する
    /// </summary>
    public interface IPlayerItem
    {
        /// <summary>
        /// アイテムのID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// アイテムの名前
        /// </summary>
        string Name
        {
            get { return this.Id; }
        }

        /// <summary>
        /// このアイテムを使用するのに必要な個数
        /// </summary>
        public int Cost
        {
            get { return 1; }
        }

        /// <summary>
        /// アイテムを使用できるかどうか
        /// </summary>
        bool CanUse(PlayerItemState playerItemState, GameObject playerObject)
        {
            return true;
        }

        /// <summary>
        /// アイテムを使用したときのアクション
        /// </summary>
        void DoUse(PlayerItemState playerItemState, GameObject playerObject);
    }
}