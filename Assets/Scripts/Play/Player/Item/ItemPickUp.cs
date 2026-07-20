using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// ステージに落ちているアイテム
    /// プレイヤが触れると所持数に加算される
    /// </summary>
    [RequireComponent(typeof(PlayerItemBehaviour))]
    public class ItemPickUp : MonoBehaviour
    {
        private PlayerItemBehaviour playerItemBehaviour;

        private void Awake()
        {
            this.playerItemBehaviour = this.GetComponent<PlayerItemBehaviour>();
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

            int slot = playerBehaviour.AddItem(this.playerItemBehaviour.PlayerItemState);

            // 回収できたときはオブジェクトを破棄
            if(slot != -1)
            {
                Destroy(gameObject);
            }
        }
    }
}