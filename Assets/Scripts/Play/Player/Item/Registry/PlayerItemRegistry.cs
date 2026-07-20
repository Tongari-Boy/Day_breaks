using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        /// <summary>
        /// 登録されたスプライトホルダー
        /// </summary>
        private readonly Dictionary<string, PlayerItemSpriteHolder> playerItemSprites = new();

        private PlayerItemRegistry()
        {
            this.Initialize();
        }

        public bool Register(params IPlayerItem[] playerItems)
        {
            bool isRegisteredAll = true;

            foreach (IPlayerItem playerItem in playerItems)
            {
                isRegisteredAll &= this.Register(playerItem);
            }

            return isRegisteredAll;
        }

        /// <summary>
        /// アイテムを登録する
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

                Debug.Log($"アイテム（ID: {playerItem.Id}）が登録されました！");

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
        /// <para>登録されたアイテムを取得する</para>
        /// <para>存在しない場合はnull</para>
        /// </summary>
        public PlayerItemHolder Get(string id)
        {
            if (id != null && id != "" && this.playerItems.ContainsKey(id))
            {
                return this.playerItems[id];
            }

            return null;
        }

        /// <summary>
        /// 登録されたアイテムを使用する
        /// </summary>
        public bool Use(PlayerItemState playerItemState, PlayerBehaviour playerBehaviour)
        {
            if (PlayerItemState.IsEmpty(playerItemState))
                return false;

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
        /// <para>Spriteの登録／削除をする</para>
        /// <para>削除するには引数spriteをnullにする</para>
        /// <para>IDはアイテムのIDと同じにする</para>
        /// </summary>
        public bool SetSprite(string id, Sprite sprite, Color color)
        {
            if (id == null || id == "")
            {
                Debug.LogWarning("アイテムIDが空であるため、Spriteを登録できません！");

                return false;
            }

            if (sprite != null)
            {
                this.playerItemSprites[id] = new PlayerItemSpriteHolder(sprite, color);

                Debug.Log($"アイテムのスプライト（ID: {id}）が登録されました！");

                return true;
            }
            else if (this.playerItemSprites.Remove(id))
            {
                Debug.Log($"アイテムのスプライト（ID: {id}）が削除されました！");

                return true;
            }

            return false;
        }

        /// <summary>
        /// <para>登録されたSpriteを取得する</para>
        /// <para>存在しない場合はnull</para>
        /// </summary>
        public PlayerItemSpriteHolder GetSprite(string id)
        {
            if (id != null && id != "" && this.playerItemSprites.ContainsKey(id))
            {
                return this.playerItemSprites[id];
            }

            return null;
        }

        /// <summary>
        /// レジストリにアイテムを登録する
        /// </summary>
        public void Initialize()
        {
            // アイテムを登録する
            this.Register
            (
                new NormalDecoyFortressRegenerator(),
                new StopDecoyFortressRegenerator(),
                new CandleDecoyFortressRegenerator(),
                new SwordDecoyFortressRegenerator(),
                new BombDecoyFortressRegenerator()
            );
        }

        /// <summary>
        /// アイテムホルダー
        /// </summary>
        public class PlayerItemHolder
        {
            /// <summary>
            /// アイテム
            /// </summary>
            public readonly IPlayerItem playerItem;

            public PlayerItemHolder(IPlayerItem playerItem)  
            {
                this.playerItem = playerItem;
            }
        }

        /// <summary>
        /// スプライトホルダー
        /// </summary>
        public class PlayerItemSpriteHolder
        {
            /// <summary>
            /// スプライト
            /// </summary>
            public readonly Sprite sprite;

            /// <summary>
            /// スプライトの色
            /// </summary>
            public readonly Color color;

            public PlayerItemSpriteHolder(Sprite sprite, Color color)
            {
                this.sprite = sprite;
                this.color = color;
            }
        }
    }
}