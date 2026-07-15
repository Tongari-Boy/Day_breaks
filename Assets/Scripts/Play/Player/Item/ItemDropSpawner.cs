using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムのドロップ(ワールドへの出現)を担当するクラス
    /// </summary>
    public class ItemDropSpawner : MonoBehaviour
    {
        public static ItemDropSpawner INSTANCE { get; private set; }

        [Header("ドロップ時に生成するピックアッププレハブ")]
        [SerializeField] private ItemPickUp itemPickupPrefab;

        private void Awake()
        {
            INSTANCE = this;
        }

        /// <summary>
        /// 指定した位置にアイテムをドロップする
        /// </summary>
        /// <param name="itemId">PlayerItemRegistryに登録済みのアイテムID</param>
        /// <param name="position">ドロップ位置</param>
        public void Drop(string itemId, Vector3 position)
        {
            var pickup = Instantiate(itemPickupPrefab, position, Quaternion.identity);
            pickup.SetUp(itemId);
        }
    }
}