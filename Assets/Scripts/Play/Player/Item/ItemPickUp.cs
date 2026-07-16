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
            Debug.Log($"衝突: {collision.gameObject.name}, タグ: {collision.gameObject.tag}");
            if (!collision.gameObject.CompareTag("Player")) return;

            var playerBehaviour = collision.gameObject.GetComponent<Player.PlayerBehaviour>();
            Debug.Log($"PlayerBehaviour取得結果: {(playerBehaviour != null ? "成功" : "失敗")}");

            if (playerBehaviour == null)
            {
                Debug.Log("PlayerBehaviorが見つかりません");
                return;
            }

            if(AddToInventory(playerBehaviour))
            {
                Destroy(gameObject);
            }
            
        }

        /// <summary>
        /// プレイヤのインベリに触れたアイテムを入れる
        /// </summary>
        private bool AddToInventory(Player.PlayerBehaviour playerBehaviour)
        {
            bool added = playerBehaviour.AddItem(itemId,amount);

            if(added)
            {
                Debug.Log($"{itemId}を{amount}個取得");
            }
            else
            {
                Debug.Log("アイテム取得:失敗");
            }

            return added;
        }
    }
}