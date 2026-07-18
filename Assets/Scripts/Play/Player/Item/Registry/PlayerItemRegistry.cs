using System;
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

        /// <summary>
        /// 登録されたアイテムホルダー
        /// </summary>
        private readonly Dictionary<string, PlayerItemHolder> playerItems = new();

        private PlayerItemRegistry()
        {
            this.Initialize();
        }

        /// <summary>
        /// 新しいアイテムを登録する
        /// </summary>
        public bool Register(IPlayerItem playerItem)
        {
            if (playerItem == null)
                return false;

            if (playerItem.Id == null || playerItem.Id == "")
            {
                Debug.Log($"IDがnullもしくは空文字であるため、アイテムを登録できません…");

                return false;
            }

            try
            {
                this.playerItems.Add(playerItem.Id, new PlayerItemHolder(playerItem));

                return true;
            }
            catch (ArgumentException)
            {
                Debug.LogWarning($"アイテム（ID: {playerItem.Id}）はすでに登録されています！");
            }

            return false;
        }

        /// <summary>
        /// 登録されたアイテムを削除する
        /// </summary>
        public bool Delete(string id)
        { 
            return this.playerItems.Remove(id);
        }

        /// <summary>
        /// 登録されたアイテムを使用する
        /// </summary>
        public bool Use(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            if (playerItemState == null || playerItemState.Id == PlayerItemState.EMPTY.Id)
            {
                Debug.LogWarning($"PlayerItemStateは空であるため使用できません…");

                return false;
            }

            if (playerBehaviour == null)
            {
                Debug.LogError($"PlayerBehaviourがnullであるためアイテム（ID: {playerItemState.Id}）を使用できません…");

                return false;
            }

            if (this.playerItems.ContainsKey(playerItemState.Id))
            {
                IPlayerItem playerItem = this.playerItems[playerItemState.Id]?.playerItem;

                // アイテムを使用できるか確認→使用
                if (playerItem != null && playerItemState.Count >= playerItem.Cost && playerItem.CanUse(playerItemState, playerBehaviour) && playerItem.DoUse(playerItemState, playerBehaviour))
                {
                    // アイテム数を減らす
                    playerItemState.Count -= playerItem.Cost;

                    return true;
                }
            }
            else
            {
                Debug.LogWarning($"アイテム（ID: {playerItemState.Id}）は登録されていません！");
            }

            return false;
        }

        /// <summary>
        /// レジストリにアイテムを登録する
        /// </summary>
        public void Initialize()
        {
            // アイテムを登録する
            this.Register(new DecoyFortressRegenerator());
        }

        /// <summary>
        /// アイテムホルダー
        /// </summary>
        private class PlayerItemHolder
        {
            /// <summary>
            /// アイテム
            /// </summary>
            public IPlayerItem playerItem;

            public PlayerItemHolder(IPlayerItem playerItem)  
            {
                this.playerItem = playerItem;
            }
        }
    }
}