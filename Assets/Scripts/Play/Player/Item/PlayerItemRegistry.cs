using Player.Item;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// <para>アイテム（IPlayerItem）を登録できるレジストリ</para>
    /// <para>このクラスはシングルトン設計であるため、PlayerItemRegistry.INSTANCEからインスタンスを取得</para>
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
        /// <param name="id">アイテムのID</param>
        /// <returns>アイテムのビルダー</returns>
        public PlayerItemBuilder Create(string id)
        {
            return new PlayerItemBuilder(id);
        }

        /// <summary>
        /// <para>新しいアイテムを登録する</para>
        /// <para>アイテムIDが重複する場合は上書きされる</para>
        /// </summary>
        /// <param name="playerItem">登録するアイテム</param>
        /// <returns>上書きされたアイテム（存在しない場合はnull）</returns>
        public IPlayerItem Register(IPlayerItem playerItem)
        {
            IPlayerItem oldPlayerItem = playerItems.ContainsKey(playerItem.Id) ? playerItems[playerItem.Id] : null;
            
            playerItems[playerItem.Id] = playerItem;

            return oldPlayerItem;
        }

        /// <summary>
        /// <para>登録されたアイテムを使用する</para>
        /// <para>IDが存在しない場合は何もしない</para>
        /// </summary>
        /// <param name="id">アイテムのID</param>
        /// <returns>アイテムIDが存在しなかった場合はfalseを返す</returns>
        public bool Use(string id, IPlayerContext playerContext)
        {
            if (playerItems.ContainsKey(id))
            {
                playerItems[id].Use(playerContext);

                return true;
            }

            return false;
        }

        /// <summary>
        /// PlayerItemRegistryの初期化
        /// </summary>
        public void Initialize() {
            // デバッグ用アイテムを登録する
            Create("debug_health_display").Name("[デバッグ] 体力の表示").Action(playerContext => Debug.Log($"体力：{playerContext.Health}、最大：{playerContext.MaxHealth}")).BuildAndRegister();
            Create("debug_speed_display").Name("[デバッグ] 移動速度の表示").Action(playerContext => Debug.Log($"通常：{playerContext.MovementSpeed}、ダッシュ時：{playerContext.SprintSpeed}")).BuildAndRegister();
        }
    }
}