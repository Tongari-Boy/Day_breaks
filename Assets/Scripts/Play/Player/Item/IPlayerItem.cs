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
        /// アイテムの画像
        /// </summary>
        Sprite Sprite { get; }

        /// <summary>
        /// アイテムを使用したときのアクション
        /// </summary>
        /// <param name="playerObject">プレイヤー</param>
        void Use(GameObject playerObject);
    }
}