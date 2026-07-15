using System;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムをインラインで作成できるビルダー
    /// </summary>
    public class PlayerItemBuilder
    {
        private readonly PlayerItem playerItem;

        public PlayerItemBuilder(string id)
        {
            this.playerItem = new PlayerItem(id);
        }

        /// <summary>
        /// アイテムの名前を設定
        /// </summary>
        public PlayerItemBuilder Name(string name)
        {
            this.playerItem.Name = name;

            return this;
        }

        /// <summary>
        /// アイテムを使用したときのアクションを設定
        /// </summary>
        public PlayerItemBuilder Action(Action<IPlayerContext> action)
        {
            this.playerItem.Action = action;

            return this;
        }

        /// <summary>
        /// アイテムを作成する
        /// </summary>
        public IPlayerItem Build()
        {
            return this.playerItem;
        }

        /// <summary>
        /// アイテムを作成し、レジストリに登録する
        /// </summary>
        public void BuildAndRegister()
        {
            PlayerItemRegistry.INSTANCE.Register(this.playerItem);
        }

        private class PlayerItem : IPlayerItem
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
                get { return this.id; }
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public Action<IPlayerContext> Action
            {
                get { return this.action; }
                set { this.action = value; }
            }

            public void Use(IPlayerContext playerContext)
            {
                this.Action(playerContext);
            }
        }
    }
}
