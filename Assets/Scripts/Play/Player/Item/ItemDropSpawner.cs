using System;
using Unity.VisualScripting;
using UnityEngine;
using static Player.Item.PlayerItemRegistry;

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

        [Header("アイテムのSprite")]
        [SerializeField] private PlayerItemSprite[] playerItemSprites;

        private void Awake()
        {
            INSTANCE = this;

            this.Register(PlayerItemRegistry.INSTANCE);
        }

        /// <summary>
        /// PlayerItemRegistryにアイテムのSpriteを登録する
        /// </summary>
        /// <param name="playerItemRegistry"></param>
        private void Register(PlayerItemRegistry playerItemRegistry)
        {
            foreach (PlayerItemSprite playerItemSprite in this.playerItemSprites)
            {
                playerItemRegistry.SetSprite(playerItemSprite.id, playerItemSprite.sprite, playerItemSprite.color);
            }
        }

        /// <summary>
        /// 指定した位置にアイテムをドロップする
        /// </summary>
        /// <param name="itemId">PlayerItemRegistryに登録済みのアイテムID</param>
        /// <param name="position">ドロップ位置</param>
        /// <param name="itemCount">アイテムの個数</param>
        public void Drop(string itemId, Vector3 position, int itemCount = 1)
        {
            // GameObjectが設定されているか確認
            if (this.playerItemObject == null)
            {
                Debug.LogError("GameObjectが未設定のため、アイテムをドロップできませんでした…");

                return;
            }

            // GameObjectの生成
            GameObject playerItemObject = Instantiate(this.playerItemObject, position, Quaternion.identity);

            // 必要なコンポーネントの取得／アタッチ
            PlayerItemBehaviour playerItemBehaviour = playerItemObject.GetOrAddComponent<PlayerItemBehaviour>();
            SpriteRenderer spriteRenderer = playerItemObject.GetOrAddComponent<SpriteRenderer>();

            // PlayerItemBehaviourの設定
            if (playerItemBehaviour != null && playerItemBehaviour.enabled)
            {
                playerItemBehaviour.PlayerItemState.Id = itemId;
                playerItemBehaviour.PlayerItemState.Count = itemCount;
            }

            // SpriteRendererの設定
            if (spriteRenderer != null && spriteRenderer.enabled)
            {
                PlayerItemSpriteHolder playerItemSpriteHolder = PlayerItemRegistry.INSTANCE.GetSprite(itemId);

                // Spriteがレジストリに登録されているか確認
                if (playerItemSpriteHolder != null && playerItemSpriteHolder.sprite != null)
                {
                    spriteRenderer.sprite = playerItemSpriteHolder.sprite;
                    spriteRenderer.color = playerItemSpriteHolder.color;

                    Debug.Log(playerItemSpriteHolder.color);
                }
            }
        }

        /// <summary>
        /// アイテムのSpriteを定義する
        /// </summary>
        [Serializable]
        private class PlayerItemSprite
        {
            [Header("アイテムのID")]
            [SerializeField] public string id;

            [Header("アイテムのSprite")]
            [SerializeField] public Sprite sprite;

            [Header("アイテムのSpriteの色")]
            [SerializeField] public Color color;
        }
    }
}