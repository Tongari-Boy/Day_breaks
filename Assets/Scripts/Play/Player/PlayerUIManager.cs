using Player.Item;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace Player
{
    /// <summary>
    /// プレイヤーのUI
    /// </summary>
    [System.Serializable]
    public class PlayerUIManager
    {
        [Header("アイテムスロット")]
        [SerializeField] private Sprite slotSprite;

        [Header("アイテムスロットの色")]
        [SerializeField] private Color slotColor;

        [Header("アイテムスロットの数")]
        [SerializeField] private int slotAmount = 9;

        [Header("アイテムスロットのオフセット")]
        [SerializeField] private Vector2 slotOffsets = new(0.0F, 0.0F);

        [Header("体力バーの表示")]
        [SerializeField] private bool hasHealthBar = true;

        [Header("体力バーのオフセット")]
        [SerializeField] private Vector2 healthBarOffsets = new(0.0F, -48.0F);

        private PlayerBehaviour playerBehaviour;
        private Canvas canvas;
        private RectTransform canvasTransform;

        private HealthBarHolder healthBarHolder;
        private UIHolder slotUIHolder;
        private SlotHolder[] slotHolders;

        public PlayerUIManager() { }

        public void Start(PlayerBehaviour playerBehaviour)
        {
            this.GeneratePlayerContexts(playerBehaviour);
            this.GeneratePlayerHealthBar();
            this.GeneratePlayerItemSlots();
        }

        private void GeneratePlayerContexts(PlayerBehaviour playerBehaviour)
        {
            this.playerBehaviour = playerBehaviour;
            this.canvas = this.playerBehaviour.GetComponentInChildren<Canvas>();

            if (canvas != null && canvas.enabled)
            {
                this.canvasTransform = canvas.GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 体力バーを生成
        /// </summary>
        private void GeneratePlayerHealthBar()
        {
            if (this.canvas == null)
                return;

            // 体力バーのクリア
            this.DestroyPlayerHealthBar();

            if (this.healthBarHolder == null)
            {
                GameObject backgorundObject = new("Health Bar Background");
                RectTransform backgroundTransform = backgorundObject.AddComponent<RectTransform>();
                Image backgroundImage = backgorundObject.AddComponent<Image>();

                GameObject contentObject = new("Health Bar Contents");
                RectTransform contentTransform = contentObject.AddComponent<RectTransform>();
                Image contentImage = contentObject.AddComponent<Image>();

                backgroundTransform.SetParent(this.canvas.transform);
                backgroundTransform.anchoredPosition = Vector2.zero;
                backgroundTransform.sizeDelta = new(80.0F, 3.0F);
                backgroundImage.color = new(0.0F, 0.0F, 0.0F, 0.5F);

                contentTransform.SetParent(backgroundTransform);
                contentTransform.anchorMin = contentTransform.anchorMax = new(0.0F, 0.5F);
                contentTransform.pivot = new(0.0F, 0.5F);
                contentTransform.anchoredPosition = Vector2.zero;
                contentTransform.sizeDelta = new(backgroundTransform.sizeDelta.x, backgroundTransform.sizeDelta.y);
                contentImage.color = new(160.0F / 255.0F, 240.0F / 255.0F, 64.0F / 255.0F, 1.0F);

                this.healthBarHolder = new
                (
                    backgorundObject,
                    backgroundTransform,
                    backgroundImage,

                    contentObject,
                    contentTransform,
                    contentImage
                );
            }
        }

        /// <summary>
        /// アイテムスロットを生成
        /// </summary>
        private void GeneratePlayerItemSlots()
        {
            if (this.canvas == null)
                return;

            // アイテムスロットのクリア
            this.DestroyPlayerItemSlots();

            // 親を生成
            if (this.slotUIHolder == null)
            {
                GameObject gameObject = new("Slots");
                RectTransform rectTransform = gameObject.AddComponent<RectTransform>();

                rectTransform.SetParent(this.canvas.transform);
                rectTransform.anchoredPosition = Vector2.zero;

                this.slotUIHolder = new UIHolder
                (
                    gameObject,
                    rectTransform
                );
            }

            // 新しいサイズのアイテムスロットを生成
            this.slotHolders = new SlotHolder[Mathf.Max(0, this.slotAmount)];

            // ループ最適化
            int length = slotHolders.Length;

            GameObject slotObject;
            RectTransform slotTransform;
            Image slotImage;

            GameObject nameObject;
            RectTransform nameTransform;
            TextMeshProUGUI nameTextMeshPro;

            GameObject countObject;
            RectTransform countTransform;
            TextMeshProUGUI countTextMeshPro;

            GameObject displayObject;
            RectTransform displayTransform;
            Image displayImage;

            for (int i = 0; i < length; ++i)
            {
                // スロットの生成
                slotObject = new($"Slot {i}");
                slotTransform = slotObject.AddComponent<RectTransform>();
                slotImage = slotObject.AddComponent<Image>();

                displayObject = new("Display");
                displayTransform = displayObject.AddComponent<RectTransform>();
                displayImage = displayObject.AddComponent<Image>();

                countObject = new("Count");
                countTransform = countObject.AddComponent<RectTransform>();
                countTextMeshPro = countObject.AddComponent<TextMeshProUGUI>();

                nameObject = new("Name");
                nameTransform = nameObject.AddComponent<RectTransform>();
                nameTextMeshPro = nameObject.AddComponent<TextMeshProUGUI>();

                // スロットの初期化
                slotTransform.SetParent(this.slotUIHolder.rectTransform);
                slotTransform.anchoredPosition = Vector2.zero;
                slotImage.sprite = this.slotSprite != null ? this.slotSprite : null;
                slotImage.color = this.slotColor;

                displayTransform.SetParent(slotTransform);
                displayTransform.anchoredPosition = Vector2.zero;

                countTransform.SetParent(slotTransform);
                countTransform.anchorMin = countTransform.anchorMax = new(1.0F, 0.5F);
                countTransform.pivot = new(0.0F, 0.5F);
                countTransform.sizeDelta = new();
                countTransform.anchoredPosition = Vector2.zero;
                countTextMeshPro.text = "× ∞";
                countTextMeshPro.fontSize = 12.0F;
                countTextMeshPro.alignment = TextAlignmentOptions.Left;
                countTextMeshPro.textWrappingMode = TextWrappingModes.NoWrap;

                nameTransform.SetParent(slotTransform);
                nameTransform.anchorMin = nameTransform.anchorMax = new(0.5F, 0.0F);
                nameTransform.sizeDelta = new();
                nameTransform.anchoredPosition = Vector2.zero;
                nameTextMeshPro.text = PlayerItemRegistry.PlayerItemHolder.EMPTY.Name;
                nameTextMeshPro.fontSize = 12.0F;
                nameTextMeshPro.alignment = TextAlignmentOptions.Center;
                nameTextMeshPro.textWrappingMode = TextWrappingModes.NoWrap;

                // スロットホルダーの生成
                this.slotHolders[i] = new SlotHolder
                (
                    slotObject,
                    slotTransform,
                    slotImage,

                    displayObject,
                    displayTransform,
                    displayImage,

                    countObject,
                    countTransform,
                    countTextMeshPro,

                    nameObject,
                    nameTransform,
                    nameTextMeshPro
                );
            }
        }

        public void Update()
        {
            this.UpdatePlayerHealthBar();
            this.UpdatePlayerItemSlots();
        }

        /// <summary>
        /// 体力バーの更新
        /// </summary>
        private void UpdatePlayerHealthBar()
        {
            this.healthBarHolder.gameObject?.SetActive(false);

            if (this.playerBehaviour == null || !this.playerBehaviour.enabled)
                return;

            if (this.canvas == null || !this.canvas.enabled || this.canvasTransform == null)
                return;

            if (this.healthBarHolder == null || this.healthBarHolder.gameObject == null || this.healthBarHolder.rectTransform == null || this.healthBarHolder.contentHolder.rectTransform == null)
                return;

            if (!this.hasHealthBar || Camera.main == null || !Camera.main.enabled)
                return;

            this.healthBarHolder.gameObject?.SetActive(true);

            // 体力バーを移動
            RectTransform backgroundTransform = this.healthBarHolder.rectTransform;
            RectTransform contentTransform = this.healthBarHolder.contentHolder.rectTransform;
            Vector2 healthBarPos = Camera.main.WorldToScreenPoint(this.playerBehaviour.transform.position);

            backgroundTransform.position = healthBarPos + (this.healthBarOffsets != null ? this.healthBarOffsets : Vector2.zero);

            // プレイヤーの体力を適用
            float contentLength = 0.0F;

            if (this.playerBehaviour != null && this.playerBehaviour.enabled && this.playerBehaviour.Health > 0.0F)
            {
                contentLength = Mathf.Clamp(this.playerBehaviour.RemainingHealth / this.playerBehaviour.Health, 0.0F, 1.0F);
            }

            contentTransform.sizeDelta = new(backgroundTransform.sizeDelta.x * contentLength, backgroundTransform.sizeDelta.y);
        }

        /// <summary>
        /// アイテムスロットの更新
        /// </summary>
        private void UpdatePlayerItemSlots(float width = 64.0F)
        {
            this.slotUIHolder.gameObject?.SetActive(false);

            if (this.playerBehaviour == null || !this.playerBehaviour.enabled)
                return;

            if (this.canvas == null || !this.canvas.enabled || this.canvasTransform == null)
                return;

            if (this.slotUIHolder == null || this.slotUIHolder.gameObject == null || this.slotHolders == null || this.slotHolders.Length <= 0)
                return;

            this.slotUIHolder.gameObject?.SetActive(true);

            // ループ最適化
            int length = this.slotHolders.Length;
            int offset = length / 2;

            float degs = length > 1 ? Mathf.PI / (length - 1) : Mathf.PI / 2.0F;
            float cos;
            float sin;

            int index = this.playerBehaviour.SelectingSlotIndex;
            float movement = this.playerBehaviour.SlotMovement;

            SlotHolder slotHolder;

            PlayerItemState playerItemState;

            for (int i = 0; i < length; ++i)
            {
                slotHolder = this.slotHolders[i];

                if (slotHolder == null)
                    continue;

                // 位置、サイズ、カラーの係数に使用
                cos = Mathf.Cos(degs * (i + movement));
                sin = 1.0F - cos * cos;

                // スロットに表示するPlayerItemState
                playerItemState = playerBehaviour.GetItem(index + offset - (int) (i + movement));

                // スロットの背景の更新
                if (slotHolder.rectTransform != null)
                {
                    slotHolder.rectTransform.anchoredPosition = new
                    (
                        this.canvasTransform.sizeDelta.x / 2.0F - width * 2.0F,
                        (this.canvasTransform.sizeDelta.y / 2.0F - width * 2.0F) * cos
                    );

                    slotHolder.rectTransform.anchoredPosition += this.slotOffsets != null ? this.slotOffsets : Vector2.zero;

                    slotHolder.rectTransform.sizeDelta = new(width * sin, width * sin);
                }

                if (slotHolder.image != null)
                {
                    slotHolder.image.sprite = this.slotSprite;
                    slotHolder.image.color = new(this.slotColor.r, this.slotColor.g, this.slotColor.b, this.slotColor.a * sin);
                }

                // スロットの表示の更新
                if (slotHolder.displayHolder != null)
                {
                    if (slotHolder.displayHolder.rectTransform != null)
                    {
                        slotHolder.displayHolder.rectTransform.sizeDelta = slotHolder.rectTransform.sizeDelta * 0.8F;
                    }

                    if (slotHolder.displayHolder.image != null)
                    {
                        PlayerItemRegistry.PlayerItemSpriteHolder holder = PlayerItemRegistry.INSTANCE.GetSprite(playerItemState.Id);

                        slotHolder.displayHolder.image.sprite = holder.sprite;
                        slotHolder.displayHolder.image.color = holder.sprite != null ? new(holder.color.r, holder.color.g, holder.color.b, holder.color.a * sin) : new();
                    }
                }

                // スロットの数の更新
                if (slotHolder.countHolder != null && slotHolder.countHolder.textMeshProUGUI != null)
                {
                    slotHolder.countHolder.textMeshProUGUI.text = $"× {playerItemState.Count}";
                    slotHolder.countHolder.textMeshProUGUI.color = i == offset && playerBehaviour.IsUseDown() ? new(1.0F, 0.75F, 0.0F, sin) : new(1.0F, 1.0F, 1.0F, sin);
                    slotHolder.countHolder.textMeshProUGUI.fontSize = 12.0F * sin;
                }

                // スロットの名前更新
                if (slotHolder.nameHolder != null && slotHolder.nameHolder.textMeshProUGUI != null)
                {
                    slotHolder.nameHolder.textMeshProUGUI.text = PlayerItemRegistry.INSTANCE.Get(playerItemState.Id).Name;
                    slotHolder.nameHolder.textMeshProUGUI.color = i == offset && playerBehaviour.IsUseDown() ? new(1.0F, 0.75F, 0.0F, sin) : new(1.0F, 1.0F, 1.0F, sin);
                    slotHolder.nameHolder.textMeshProUGUI.fontSize = 12.0F * sin;
                }
            }
        }

        public void Destroy()
        {
            this.DestroyPlayerHealthBar();
            this.DestroyPlayerItemSlots();
        }

        /// <summary>
        /// 体力バーの破棄
        /// </summary>
        private void DestroyPlayerHealthBar()
        {
            if (this.healthBarHolder != null)
            {
                this.healthBarHolder.Destroy();
            }
        }

        /// <summary>
        /// アイテムスロットを破棄
        /// </summary>
        private void DestroyPlayerItemSlots()
        {
            // 親スロットの破棄
            if (this.slotUIHolder != null)
            {
                this.slotUIHolder.Destroy();
            }

            // 子スロットの破棄
            if (this.slotHolders == null || this.slotHolders.Length <= 0)
                return;

            // ループ最適化
            int length = this.slotHolders.Length;
            SlotHolder slotHolder;

            for (int i = 0; i < length; ++i)
            {
                slotHolder = this.slotHolders[i];

                if (slotHolder != null)
                {
                    slotHolder.Destroy();
                }

                this.slotHolders[i] = null;
            }
        }

        private class HealthBarHolder : UIHolder
        {
            public readonly UIHolder contentHolder;

            public HealthBarHolder
            (
                GameObject backgroundObject,
                RectTransform backgroundTransform,
                Image backgroundImage,

                GameObject contentObject,
                RectTransform contentTransform,
                Image contentImage
            ) : base(backgroundObject, backgroundTransform, backgroundImage)
            {
                this.contentHolder = new UIHolder
                (
                    contentObject,
                    contentTransform,
                    contentImage
                );
            }

            public override void Destroy()
            {
                base.Destroy();
                this.contentHolder.Destroy();
            }
        }

        /// <summary>
        /// アイテムスロットUIのホルダー
        /// </summary>
        private class SlotHolder : UIHolder
        {
            public readonly UIHolder displayHolder;
            public readonly UIHolder countHolder;
            public readonly UIHolder nameHolder;

            public SlotHolder
            (
                GameObject slotObject,
                RectTransform slotTransform,
                Image slotImage,

                GameObject displayObject,
                RectTransform displayTransform,
                Image displayImage,

                GameObject countObject,
                RectTransform countTransform,
                TextMeshProUGUI countTextMeshPro,

                GameObject nameObject,
                RectTransform nameTransform,
                TextMeshProUGUI nameTextMeshPro
            ) : base(slotObject, slotTransform, slotImage)
            {
                this.displayHolder = new UIHolder
                (
                    displayObject,
                    displayTransform,
                    displayImage
                );

                this.countHolder = new UIHolder
                (
                    countObject,
                    countTransform,
                    null,
                    countTextMeshPro
                );

                this.nameHolder = new UIHolder
                (
                    nameObject,
                    nameTransform,
                    null,
                    nameTextMeshPro
                );
            }

            public override void Destroy()
            {
                base.Destroy();
                this.displayHolder.Destroy();
                this.countHolder.Destroy();
                this.nameHolder.Destroy();
            }
        }

        /// <summary>
        /// UIのホルダー
        /// </summary>
        private class UIHolder
        {
            public readonly GameObject gameObject;
            public readonly RectTransform rectTransform;
            public readonly Image image;
            public readonly TextMeshProUGUI textMeshProUGUI;

            public UIHolder(GameObject gameObject, RectTransform rectTransform, Image image = null, TextMeshProUGUI textMeshProUGUI = null)
            {
                this.gameObject = gameObject;
                this.rectTransform = rectTransform;
                this.image = image;
                this.textMeshProUGUI = textMeshProUGUI;
            }

            public virtual void Destroy()
            {
                if (this.gameObject != null)
                {
                    UnityEngine.Object.Destroy(this.gameObject);
                }
            }
        }
    }
}
