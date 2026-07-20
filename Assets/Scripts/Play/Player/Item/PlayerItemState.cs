using System;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムのIDと個数を保持する
    /// </summary>
    [System.Serializable]
    public class PlayerItemState
    {
        /// <summary>
        /// 空のPlayerItemState
        /// </summary>
        public static readonly PlayerItemState EMPTY = new EmptyItemState();

        [Header("アイテムのID")]
        [SerializeField] private string id = "";

        [Header("アイテムの個数")]
        [SerializeField] private int count = 0;

        /// <summary>
        /// アイテムのID
        /// </summary>
        public string Id
        {
            get { return this.id; }
            set { this.id = value ?? ""; }
        }

        /// <summary>
        /// アイテムの個数
        /// </summary>
        public int Count
        {
            get { return this.count; }
            set { this.count = Mathf.Max(0, value); }
        }

        public PlayerItemState(string id = "", int count = 1)
        {
            this.Id = id;
            this.Count = count;
        }

        public static bool IsEmpty(PlayerItemState playerItemState)
        {
            return playerItemState == null || playerItemState.Id == PlayerItemState.EMPTY.Id || playerItemState.Count <= 0;
        }

        /// <summary>
        /// IDが同じであればtrueを返す
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            else if (obj is PlayerItemState playerItemState)
            {
                return playerItemState.id == this.id;
            }

            return false;
        }

        /// <summary>
        /// IDと同じハッシュ値を返す
        /// </summary>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        /// <summary>
        /// アイテムを使用する
        /// </summary>
        public bool Use(PlayerBehaviour playerBehaviour)
        {
            return PlayerItemRegistry.INSTANCE.Use(this, playerBehaviour);
        }

        private class EmptyItemState : PlayerItemState
        {
            public EmptyItemState() : base("", 0) { }

            public new string Id
            {
                get { return this.id; }
                set { }
            }

            public new int Count
            {
                get { return this.count; }
                set { }
            }

            public new bool Use(PlayerBehaviour playerBehaviour)
            {
                Debug.Log("プレイヤーが空のPlayerItemStateを使用しました…");

                return false;
            }
        }
    }
}