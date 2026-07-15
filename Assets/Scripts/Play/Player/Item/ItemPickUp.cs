using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// ステージに落ちているアイテム
    /// プレイヤが触れると所持数に加算される
    /// </summary>
    public class ItemPickUp : MonoBehaviour
    {
        [SerializeField] private string itemId;

        [Header("拾得時に加算する個数(基本的に1)")]
        [SerializeField] private int amount = 1;

        public void SetUp(string itemId)
        {
            this.itemId = itemId;
        }

        /// <summary>
        /// プレイヤとの衝突判定を行う処理
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var playerContext = collision.GetComponent<IPlayerContext>();
            // 衝突したものがプレイヤでなかったら無視
            if (playerContext == null) return;

            AddToInventory(playerContext);
            Destroy(gameObject);
        }

        /// <summary>
        /// プレイヤのインベリに触れたアイテムを入れる
        /// </summary>
        /// <param name="playerContext"></param>
        private void AddToInventory(IPlayerContext playerContext)
        {
            var existingSlot = playerContext.PlayerItemSlots.Find(slot => slot.Id == itemId);

            if(existingSlot != null)
            {
                existingSlot.Count += amount;
            }
            else
            {
                playerContext.PlayerItemSlots.Add(new PlayerItemState
                {
                    Id = itemId,
                    Count = amount
                });
            }

            Debug.Log($"{itemId}を{amount}個取得");
        }
    }
}