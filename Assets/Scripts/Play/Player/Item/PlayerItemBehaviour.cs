using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// GameObjectとして存在できるアイテム
    /// </summary>
    public class PlayerItemBehaviour : MonoBehaviour
    {
        [Header("アイテムのステータス")]
        [SerializeField] private PlayerItemState playerItemState = new();

        public PlayerItemState PlayerItemState
        {
            get { return playerItemState; }
        }
    }
}
