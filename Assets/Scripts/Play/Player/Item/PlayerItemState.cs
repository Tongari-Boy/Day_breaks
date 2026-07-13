using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// <para>アイテムのIdと個数を保持する</para>
    /// <para>IPlayerContextで使用される</para>
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
            get { return id; }

            set { id = value; }
        }

        /// <summary>
        /// アイテムの個数
        /// </summary>
        public int Count
        {
            get { return count; }

            set { count = Mathf.Max(0, value); }
        }

        /// <summary>
        /// <para>レジストリ（PlayerItemRegistry）から該当するアイテムを使用する</para>
        /// <para>使用するとcountがamountだけ減少する</para>
        /// <para>IDがレジストリに存在しない場合、もしくはcount - amountが0未満になるときは何もしない</para>
        /// </summary>
        /// <param name="playerContext">使用するプレイヤーのデータ</param>
        /// <param name="amount">減少するアイテムの個数</param>
        public void Use(IPlayerContext playerContext, int amount = 1)
        {
            if (Count - amount >= 0 && PlayerItemRegistry.INSTANCE.Use(Id, playerContext))
            {
                Count -= amount;
            }
        }
    }
}
