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
            PlayerItemState state = this.playerItemBehaviour.PlayerItemState;

            bool added = playerBehaviour.AddItem(state.Id,state.Count) != -1;

            if(added)
            {
                Debug.Log($"{state.Id}を{state.Count}個取得");
            }
            else
            {
                Debug.Log("アイテム取得:失敗");
            }

            return added;
        }
    }
}