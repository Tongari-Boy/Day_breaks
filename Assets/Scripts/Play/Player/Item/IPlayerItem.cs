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
        string Name { get; }

        /// <summary>
        /// アイテムのスプライト
        /// </summary>
        Sprite Sprite { get;  }

        /// <summary>
        /// アイテムのGameObject
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// アイテムを使用したときに実行されるアクション
        /// </summary>
        void Use(IPlayerContext playerContext);
    }
}
