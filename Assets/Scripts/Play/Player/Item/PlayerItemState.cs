using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムのIdと個数を保持する
    /// </summary>
    [System.Serializable]
    public class PlayerItemState
    {
        [Header("アイテムのID")]
        [SerializeField] private string id;

        [Header("アイテムの個数")]
        [SerializeField] private int count;

        /// <summary>
        /// アイテムのID
        /// </summary>
        public string Id
        {
            get { return this.id; }

            set { this.id = value; }
        }

        /// <summary>
        /// アイテムの個数
        /// </summary>
        public int Count
        {
            get { return this.count; }

            set { this.count = Mathf.Max(0, value); }
        }

        /// <summary>
        /// レジストリ（PlayerItemRegistry）から該当するアイテムを使用する
        /// </summary>
        public void Use(IPlayerContext playerContext, int amount = 1)
        {
            if (this.Count - amount >= 0 && PlayerItemRegistry.INSTANCE.Use(this.Id, playerContext))
            {
                this.Count -= amount;
            }
        }
    }
}
