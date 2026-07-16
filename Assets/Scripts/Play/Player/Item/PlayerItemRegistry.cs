using System.Collections.Generic;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムが登録されるレジストリ
    /// </summary>
    public class PlayerItemRegistry
    {
        /// <summary>
        /// このクラスのインスタンス
        /// </summary>
        public static readonly PlayerItemRegistry INSTANCE = new();

        public readonly Dictionary<string, IPlayerItem> playerItems;

        private PlayerItemRegistry()
        {
            this.playerItems = new();
        }

        /// <summary>
        /// 新しいアイテムを登録する
        /// </summary>
        public void Register(IPlayerItem playerItem)
        {
            this.playerItems.Add(playerItem.Id, playerItem);
        }
    }
}