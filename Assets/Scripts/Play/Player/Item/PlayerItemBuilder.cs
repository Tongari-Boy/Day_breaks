using System;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムをインラインで作成できるビルダー
    /// </summary>
    public class PlayerItemBuilder
    {
        /// <summary>
        /// アイテムのインスタンス
        /// </summary>
        private readonly PlayerItem playerItem;

        public PlayerItemBuilder(string id)
        {
            playerItem = new PlayerItem(id);
        }

        /// <summary>
        /// アイテムの名前を設定
        /// </summary>
        /// <param name="name">アイテムの名前</param>
        /// <returns>アイテムのビルダー</returns>
        public PlayerItemBuilder Name(string name)
        {
            playerItem.Name = name;

            return this;
        }

        /// <summary>
        /// アイテムを使用したときのアクションを設定
        /// </summary>
        /// <param name="action">アイテムのアクション</param>
        /// <returns>アイテムのビルダー</returns>
        public PlayerItemBuilder Action(Action<IPlayerContext> action)
        {
            playerItem.Action = action;

            return this;
        }

        /// <summary>
        /// アイテムを作成する
        /// </summary>
        /// <returns>作成されたアイテム</returns>
        public IPlayerItem Build()
        {
            return playerItem;
        }

        /// <summary>
        /// アイテムを作成し、レジストリに登録する
        /// </summary>
        public void BuildAndRegister()
        {
            PlayerItemRegistry.INSTANCE.Register(playerItem);
        }

        /// <summary>
        /// ビルダー用に実装
        /// </summary>
        public class PlayerItem : IPlayerItem
        {
            private readonly string id;

            private string name;

            private Action<IPlayerContext> action;

            public PlayerItem(string id)
            {
                this.id = id;
                this.name = "Unknown";
                this.action = playerContext => { };
            }

            public string Id
            {
                get { return id; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public Action<IPlayerContext> Action
            {
                get { return action; }
                set { action = value; }
            }

            public void Use(IPlayerContext playerContext)
            {
                Action(playerContext);
            }
        }
    }
}
