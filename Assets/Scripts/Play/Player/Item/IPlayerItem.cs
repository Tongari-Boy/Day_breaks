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
        /// アイテムのスプライト
        /// </summary>
        public Sprite Sprite
        {
            get { return null; }
        }

        /// <summary>
        /// アイテムを使用できるかどうか
        /// </summary>
        bool CanUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            return true;
        }

        /// <summary>
        /// アイテムを使用したときのアクション
        /// </summary>
        bool DoUse(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour);
    }
}