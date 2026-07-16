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

        private PlayerItemRegistry() { }

        /// <summary>
        /// 新しいアイテムを登録する
        /// </summary>
        public bool Register(IPlayerItem playerItem, GameObject gameObject = null)
        {
            if (playerItem == null)
                return false;

            try
            {
                this.playerItems.Add(playerItem.Id, new PlayerItemHolder(playerItem, gameObject));

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
        /// アイテムにGameObjectを紐づける
        /// </summary>
        public bool SetGameObject(string id, GameObject gameObject)
        {
            if (this.playerItems.ContainsKey(id))
            {
                PlayerItemHolder holder = this.playerItems[id];

                if (holder != null)
                {
                    holder.gameObject = gameObject;

                    return true;
                }
            }

            Debug.LogWarning($"アイテム（ID: {id}）は登録されていません！");

            return false;
        }

        /// <summary>
        /// <para>アイテムに紐づけられたGameObjectを取得する</para>
        /// <para>存在しない場合はnull</para>
        /// </summary>
        public GameObject GetGameObject(string id)
        {
            return this.playerItems.ContainsKey(id) ? this.playerItems[id].gameObject : null;
        }

        /// <summary>
        /// 登録されたアイテムを使用する
        /// </summary>
        public bool Use(PlayerItemState playerItemState, GameObject playerObject)
        {
            if (this.playerItems.ContainsKey(playerItemState.Id))
            {
                IPlayerItem playerItem = this.playerItems[playerItemState.Id]?.playerItem;

                if (playerItem != null && playerItemState.Count >= playerItem.Cost)
                {
                    playerItemState.Count -= playerItem.Cost;

                    playerItem.Use(playerItemState, playerObject);

                    return true;
                }

                Debug.LogError($"アイテム（ID: {playerItemState.Id}）がnullです！");
            }
            else
            {
                Debug.LogWarning($"アイテム（ID: {playerItemState.Id}）は登録されていません！");
            }

            return false;
        }

        /// <summary>
        /// アイテムとその他オブジェクトを紐づける
        /// </summary>
        private class PlayerItemHolder
        {
            /// <summary>
            /// アイテム
            /// </summary>
            public IPlayerItem playerItem;

            /// <summary>
            /// アイテムのGameObject
            /// </summary>
            public GameObject gameObject;

            public PlayerItemHolder(IPlayerItem playerItem, GameObject gameObject)  
            {
                this.playerItem = playerItem;
                this.gameObject = gameObject;
            }
        }
    }
}