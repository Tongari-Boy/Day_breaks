using Unity.VisualScripting;
using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムのドロップ(ワールドへの出現)を担当するクラス
    /// </summary>
    public class ItemDropSpawner : MonoBehaviour
    {
        public static ItemDropSpawner INSTANCE { get; private set; }

        [Header("ドロップ時に生成するアイテム")]
        [SerializeField] private GameObject playerItemObject;

        private void Awake()
        {
            INSTANCE = this;
        }

        /// <summary>
        /// 指定した位置にアイテムをドロップする
        /// </summary>
        /// <param name="itemId">PlayerItemRegistryに登録済みのアイテムID</param>
        /// <param name="position">ドロップ位置</param>
        /// <param name="itemCount">アイテムの個数</param>
        public void Drop(string itemId, Vector3 position, int itemCount = 1)
        {
            // GameObjectの生成
            GameObject playerItemObject = Instantiate(this.playerItemObject, position, Quaternion.identity);

            // PlayerItemBehaviourの取得
            PlayerItemBehaviour playerItemBehaviour = playerItemObject.GetOrAddComponent<PlayerItemBehaviour>();

            // PlayerItemStateを初期化
            if (playerItemBehaviour != null && playerItemBehaviour.enabled)
            {
                playerItemBehaviour.PlayerItemState.Id = itemId;
                playerItemBehaviour.PlayerItemState.Count = itemCount;
            }

            playerItemObject.GetOrAddComponent<ItemPickUp>();
        }
    }
}