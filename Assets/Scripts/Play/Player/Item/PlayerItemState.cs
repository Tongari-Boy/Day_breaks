using UnityEngine;

namespace Player.Item
{
    /// <summary>
    /// アイテムのIDと個数を保持する
    /// </summary>
    [System.Serializable]
    public class PlayerItemState
    {
        [Header("アイテムのID")]
        [SerializeField] private string id;

        [Header("アイテムの個数")]
        [SerializeField] private int count;

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public PlayerItemState(string id = "unknown", int count = 0)
        {
            this.id = id;
            this.count = count;
        }
    }
}