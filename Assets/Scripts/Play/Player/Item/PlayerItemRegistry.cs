using System.Collections.Generic;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテム（IPlayerItem）を登録できるレジストリ
    /// </summary>
    public class PlayerItemRegistry
    {
        /// <summary>
        /// PlayerItemRegistryクラスのインスタンス
        /// </summary>
        public static readonly PlayerItemRegistry INSTANCE = new();

        /// <summary>
        /// アイテムのレジストリ
        /// </summary>
        private readonly Dictionary<string, IPlayerItem> playerItems = new();

        private PlayerItemRegistry() { }

        /// <summary>
        /// アイテムを定義する
        /// </summary>
        public PlayerItemBuilder Create(string id)
        {
            return new PlayerItemBuilder(id);
        }

        /// <summary>
        /// 新しいアイテムを登録する
        /// </summary>
        public IPlayerItem Register(IPlayerItem playerItem)
        {
            IPlayerItem oldPlayerItem = this.playerItems.ContainsKey(playerItem.Id) ? this.playerItems[playerItem.Id] : null;
            
            this.playerItems[playerItem.Id] = playerItem;

            return oldPlayerItem;
        }

        /// <summary>
        /// 登録されたアイテムを使用する
        /// </summary>
        public bool Use(string id, IPlayerContext playerContext)
        {
            if (this.playerItems.ContainsKey(id))
            {
                this.playerItems[id].Use(playerContext);

                return true;
            }
            else
            {
                Debug.LogWarning($"アイテム（id: {id}）は登録されていません！");
            }

                return false;
        }
    }
}