using Castle;
using DecoyFortress;
using Enemy;
using Player.Bullet;
using Player.Item;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    public class PlayerBehaviour : MonoBehaviour, IDamageable
    {
        [Header("カメラ追従モード")]
        [SerializeField] private bool cameraFollowingMode = false;

        [Header("プレイヤーの体力")]
        [SerializeField] private int health = 100;

        [Header("プレイヤーの攻撃力")]
        [SerializeField] private int attackDamage = 50;

        [Header("プレイヤーの速度（通常）")]
        [SerializeField] private float movementSpeed = 1.0F;

        [Header("プレイヤーの速度（ダッシュ時）")]
        [SerializeField] private float sprintSpeed = 3.0F;

        [Header("シューター")]
        [SerializeField] private GameObject shooterObject;

        [Header("弾")]
        [SerializeField] private GameObject bulletObject;

        [Header("弾の速度")]
        [SerializeField] private float bulletSpeed = 5.0F;

        [Header("弾の寿命（秒）")]
        [SerializeField] private float bulletDuration = 8.0F;

        [Header("弾のスポーン位置までの距離")]
        [SerializeField] private float bulletSpawnDistance = 0.25F;

        [Header("射撃後のクールダウン（秒）")]
        [SerializeField] private float shootingCooldown = 1.0F;

        [Header("連射モード")]
        [SerializeField] private bool holdingShootingMode = true;

        [Header("アイテムスロット")]
        [SerializeField] private GameObject slotObject;

        [Header("アイテムスロットのアニメーション時間（秒）")]
        [SerializeField] private float slotAnimation = 0.5F;

        [Header("プレイヤーの所持するアイテム")]
        [SerializeField] private PlayerItemState[] playerItemStates = new PlayerItemState[0];

        [Header("アイテム使用後のクールダウン（秒）")]
        [SerializeField] private float usingCooldown = 1.0F;

        private int remainingHealth;
        private float remainingShootingCooldown;
        private float remainingUsingCooldown;
        private float remainingSlotAnimation;

        private float oldSlotPosition;
        private float newSlotPosition;

        private InputAction move;
        private InputAction sprint;
        private InputAction use;
        private InputAction shoot;
        private InputAction cursor;
        private InputAction scroll;

        private new Rigidbody2D rigidbody2D;
        private Canvas canvas;
        private GameObject[] slotObjects;

        private readonly Dictionary<string, EnemyMovement> interactingEnemies = new();
        private readonly Dictionary<string, PlayerItemBehaviour> interactingPlayerItems = new();
        private readonly Dictionary<string, DecoyFortressSetting> interactingDecoyFortresses = new();
        private readonly Dictionary<string, CastleManager> interactingCastles = new();

        /// <summary>
        /// プレイヤーの体力（残り）
        /// </summary>
        public int RemainingHealth
        {
            get { return this.remainingHealth; }
        }

        /// <summary>
        /// プレイヤーの体力（最大）
        /// </summary>
        public int Health
        {
            get { return this.health; }
        }

        /// <summary>
        /// 射撃後のクールダウン（残り）
        /// </summary>
        public float RemainingShootingCooldown
        {
            get { return Mathf.Max(0.0F, this.remainingShootingCooldown); }
        }

        /// <summary>
        /// 射撃後のクールダウン（最大）
        /// </summary>
        public float ShootingCooldown
        {
            get { return Mathf.Max(0.0F, this.shootingCooldown); }
        }

        /// <summary>
        /// アイテム使用後のクールダウン（残り）
        /// </summary>
        public float RemainingUsingCooldown
        {
            get { return Mathf.Max(0.0F, this.remainingUsingCooldown); }
        }

        /// <summary>
        /// アイテム使用後のクールダウン（最大）
        /// </summary>
        public float UsingCooldown
        {
            get { return Mathf.Max(0.0F, this.usingCooldown); }
        }

        /// <summary>
        /// プレイヤーが接触しているEnemyMovement
        /// </summary>
        public Dictionary<string, EnemyMovement>.ValueCollection InteractingEnemies
        {
            get { return this.interactingEnemies.Values; }
        }

        /// <summary>
        /// プレイヤーが接触しているPlayerItemBehaviour
        /// </summary>
        public Dictionary<string, PlayerItemBehaviour>.ValueCollection InteractingPlayerItems
        {
            get { return this.interactingPlayerItems.Values; }
        }

        /// <summary>
        /// プレイヤーが接触しているDecoyFortressSetting
        /// </summary>
        public Dictionary<string, DecoyFortressSetting>.ValueCollection InteractingDecoyFortresses
        {
            get { return this.interactingDecoyFortresses.Values; }
        }

        /// <summary>
        /// プレイヤーが接触しているCastleManager
        /// </summary>
        public Dictionary<string, CastleManager>.ValueCollection InteractingCastles
        {
            get { return this.interactingCastles.Values; }
        }

        /// <summary>
        /// プレイヤーのアイテムスロット（シャローコピー）
        /// </summary>
        public PlayerItemState[] PlayerItemSlots
        {
            get { return (PlayerItemState[])this.playerItemStates.Clone(); }
        }

        /// <summary>
        /// 選択中のスロット番号
        /// </summary>
        public int SelectingSlot
        {
            get
            {
                if (this.playerItemStates.Length > 0)
                {
                    int slot = Mathf.CeilToInt(this.SlotPosition) % this.playerItemStates.Length;

                    if (slot < 0)
                    {
                        slot += this.playerItemStates.Length;
                    }

                    return slot;
                }

                return 0;
            }
        }

        /// <summary>
        /// スロットの位置
        /// </summary>
        private float SlotPosition
        {
            get
            {
                return this.slotAnimation <= 0.0F ? 0.0F : this.newSlotPosition + (this.oldSlotPosition - this.newSlotPosition) * (this.remainingSlotAnimation / this.slotAnimation);
            }
        }

        /// <summary>
        /// スロットのアニメーション補間（0.0F～1.0F）
        /// </summary>
        private float SlotStep
        {
            get
            {
                float step = this.slotAnimation <= 0.0F ? 0.0F : (this.newSlotPosition - this.oldSlotPosition) * (this.remainingSlotAnimation / this.slotAnimation);

                step %= 1.0F;

                if (step < 0.0F)
                {
                    step += 1.0F;
                }

                return step;
            }
        }

        /// <summary>
        /// 選択中のアイテムを使用する
        /// </summary>
        public bool UseItem()
        {
            PlayerItemState playerItemState = this.GetItem(this.SelectingSlot);

            if (PlayerItemState.IsEmpty(playerItemState))
            {
                Debug.Log("空のアイテムスロットを選択しているため、アイテムを使用できませんでした…");

                return false;
            }

            if (playerItemState.Use(this))
            {
                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}の使用に成功しました！）");

                return true;
            }
            else
            {
                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}の使用に失敗しました…");

                return false;
            }
        }

        /// <summary>
        /// <para>PlayerItemState.Countが1以上であれば回収</para>
        /// <para>回収できた場合はPlayerItemState.Countは0になり、そのスロット番号を返す</para>
        /// <para>できない場合は何もせず-1を返す</para>
        /// </summary>
        public int AddItem(PlayerItemState playerItemState)
        {
            int slot = playerItemState != null ? this.AddItem(playerItemState.Id, playerItemState.Count) : -1;

            // 回収できた場合はPlayerItemState.Countを0にする
            if (slot != -1 && playerItemState.Count > 0)
            {
                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}）を{playerItemState.Count}個、回収しました！");

                playerItemState.Count = 0;
            }
            else
            {
                Debug.Log($"プレイヤーはアイテム（ID: {playerItemState.Id}）を回収できませんでした…");
            }

            return slot;
        }

        /// <summary>
        /// <para>同じアイテムを持っていれば、countだけ個数を増やす</para>
        /// <para>アイテムスロットに空きがあれば、アイテムを追加する</para>
        /// <para>追加できた場合はそのスロット番号を返す</para>
        /// <para>できなかった場合は-1を返す</para>
        /// </summary>
        public int AddItem(string id, int count = 1)
        {
            PlayerItemState playerItemState;
            int empty = -1;

            for (int i = this.playerItemStates.Length - 1; i >= 0; --i)
            {
                playerItemState = this.playerItemStates[i];

                if (PlayerItemState.IsEmpty(playerItemState))
                {
                    // 空のスロット番号を記憶
                    empty = i;
                }
                else if (playerItemState.Id == id)
                {
                    // アイテムをcount個増やす
                    playerItemState.Count += count;

                    return i;
                }
            }

            // 空のスロットにアイテムを追加
            if (empty >= 0 && id != PlayerItemState.EMPTY.Id)
            {
                this.playerItemStates[empty] = new PlayerItemState(id, count);

                return empty;
            }

            return -1;
        }

        /// <summary>
        /// <para>指定したIDからスロット内のアイテムを取得する</para>
        /// <para>存在しない場合はPlayerItemState.EMPTYを返す</para>
        /// </summary>
        public PlayerItemState GetItem(string id)
        {
            int length = this.playerItemStates.Length;

            PlayerItemState playerItemState;

            for (int i = 0; i < length; ++i)
            {
                playerItemState = this.PlayerItemSlots[i];

                if (playerItemState != null && playerItemState.Id == id)
                {
                    return playerItemState;
                }
            }

            return PlayerItemState.EMPTY;
        }


        /// <summary>
        /// <para>指定したスロット番号にあるアイテムを取得する</para>
        /// <para>存在しない場合はPlayerItemState.EMPTYを返す</para>
        /// </summary>
        public PlayerItemState GetItem(int slot)
        {
            if (this.playerItemStates.Length > 0)
            {
                int index = slot % this.playerItemStates.Length;

                if (index < 0)
                {
                    index += this.playerItemStates.Length;
                }

                return PlayerItemState.IsEmpty(this.PlayerItemSlots[index]) ? PlayerItemState.EMPTY : this.PlayerItemSlots[index];
            }

            return PlayerItemState.EMPTY;
        }

        /// <summary>
        /// IDamageableより実装
        /// </summary>
        public void OnDamaged(int damageAmount)
        {
            this.remainingHealth = Mathf.Clamp(this.remainingHealth - damageAmount, 0, this.health);
        }

        public void Awake()
        {
            // 入力の取得
            InputActionMap playerActions = this.GetComponent<PlayerInput>().currentActionMap;

            this.move = playerActions.FindAction("Move");
            this.sprint = playerActions.FindAction("Sprint");
            this.use = playerActions.FindAction("Use");
            this.shoot = playerActions.FindAction("Shoot");
            this.cursor = playerActions.FindAction("Cursor");
            this.scroll = playerActions.FindAction("Scroll");

            // Rigidbody2Dの取得
            this.rigidbody2D = this.GetComponent<Rigidbody2D>();

            // Canvasの取得
            this.canvas = this.GetComponentInChildren<Canvas>();

            // ステータスの初期化
            this.remainingHealth = this.health;
            this.remainingShootingCooldown = this.shootingCooldown;

            // UIの初期化
            this.RepaintUI();

        }

        public void Update()
        {
            this.Select();
            this.Use();
            this.Shoot();
            this.RepaintUI();
        }

        public void FixedUpdate()
        {
            this.Move();
            this.Follow();
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void Move()
        {
            // 入力を確認
            if (this.move == null || !this.move.IsPressed())
                return;

            // Rigidbody2Dを確認
            if (this.rigidbody2D == null)
                return;

            // 速度の計算
            Vector2 direction = this.move.ReadValue<Vector2>();
            float finalSpeed = this.sprint != null && this.sprint.IsPressed() ? this.sprintSpeed : this.movementSpeed;

            // 速度の適用
            this.rigidbody2D.linearVelocity = direction * finalSpeed;
        }

        /// <summary>
        /// カメラの更新
        /// </summary>
        private void Follow()
        {
            if (this.cameraFollowingMode && Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
            }
        }

        /// <summary>
        /// アイテムの選択
        /// </summary>
        private void Select()
        {
            // アイテムスロットのアニメーションを更新
            if (this.remainingSlotAnimation > 0.0F)
            {
                this.remainingSlotAnimation = Mathf.Max(0.0F, this.remainingSlotAnimation - Time.deltaTime);
            }

            // 入力を確認
            if (this.scroll == null)
                return;

            Vector2 scrollVelocity = this.scroll.ReadValue<Vector2>();

            // スクロール量を確認
            if (scrollVelocity.y == 0.0F)
            {
                return;
            }

            // アイテムスロットのアニメーションを設定
            this.oldSlotPosition = this.remainingSlotAnimation > 0.0F ? this.SlotPosition : this.newSlotPosition;
            this.newSlotPosition += scrollVelocity.y < 0.0F ? -1 : 1;
            this.remainingSlotAnimation = this.slotAnimation;
        }

        /// <summary>
        /// アイテムの使用
        /// </summary>
        private void Use()
        {
            // クールダウンを減らす
            if (this.remainingUsingCooldown > 0.0F)
            {
                this.remainingUsingCooldown -= Time.deltaTime;
                return;
            }

            // 入力を確認
            if (this.use == null || !this.use.WasPressedThisFrame())
                return;

            PlayerItemState playerItemState = this.GetItem(this.SelectingSlot);

            if (PlayerItemState.IsEmpty(playerItemState))
            {
                Debug.Log("空のアイテムスロットを選択しているため、アイテムを使用できませんでした…");

                return;
            }

            if (playerItemState.Use(this))
            {
                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}の使用に成功しました！）");
            }
            else
            {
                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}の使用に失敗しました…");
            }

            // クールダウンを設定
            this.remainingUsingCooldown = this.usingCooldown;
        }

        /// <summary>
        /// プレイヤーの射撃
        /// </summary>
        private void Shoot()
        {
            // クールダウンを減らす
            if (this.remainingShootingCooldown > 0.0F)
            {
                this.remainingShootingCooldown -= Time.deltaTime;
                return;
            }

            // 入力を確認
            if (this.shoot == null)
                return;

            if (this.holdingShootingMode)
            {
                if (!this.shoot.IsPressed())
                    return;
            }
            else
            {
                if (!this.shoot.WasPressedThisFrame())
                    return;
            }

            // 弾を確認
            if (this.bulletObject == null)
            {
                Debug.LogWarning("プレイヤーの弾が設定されていません！");
                return;
            }

            // シューターを設定
            GameObject shooterObject = this.shooterObject == null ? this.gameObject : this.shooterObject;

            if (!shooterObject.activeSelf || !shooterObject.activeInHierarchy)
            {
                Debug.LogWarning("シューターが非アクティブです！");
                return;
            }

            // 弾の位置と速度の計算
            Vector2 cursorPos = this.cursor.ReadValue<Vector2>();
            Vector2 shooterPos = shooterObject.transform.position;
            Vector2 aimPos = Camera.main ? Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, 0F)) : shooterPos;
            Vector2 aimDir = Vector2.Normalize(new Vector2(aimPos.x, aimPos.y) - new Vector2(shooterPos.x, shooterPos.y));

            // 弾を射撃
            GameObject bulletObject = UnityEngine.Object.Instantiate(this.bulletObject, shooterPos + aimDir * this.bulletSpawnDistance, Quaternion.identity);
            Rigidbody2D rigidbody2D = bulletObject.GetOrAddComponent<Rigidbody2D>();
            BulletBehaviour bulletBehaviour = bulletObject.GetOrAddComponent<BulletBehaviour>();

            rigidbody2D.mass = 1.0F;
            rigidbody2D.linearVelocity = aimDir * this.bulletSpeed;
            rigidbody2D.linearDamping = 0.0F;
            rigidbody2D.angularDamping = 0.0F;
            rigidbody2D.gravityScale = 0.0F;
            bulletBehaviour.AttackDamage = this.attackDamage;
            bulletBehaviour.Duration = this.bulletDuration;

            // クールダウンを設定
            this.remainingShootingCooldown = this.shootingCooldown;
        }

        public void OnCollisionEnter2D(Collision2D collision) { this.InteractGameObject(collision.gameObject, true); }

        public void OnCollisionExit2D(Collision2D collision) { this.InteractGameObject(collision.gameObject, false); }

        public void OnTriggerEnter2D(Collider2D collider2d) { this.InteractGameObject(collider2d.gameObject, true); }

        public void OnTriggerExit2D(Collider2D collider2d) { this.InteractGameObject(collider2d.gameObject, false); }

        /// <summary>
        /// GameObjectとの接触
        /// </summary>
        private void InteractGameObject(GameObject gameObject, bool triggerState)
        {
            if (this.InteractEnemy(gameObject, triggerState))
                return;

            if (this.InteractPlayerItem(gameObject, triggerState))
                return;

            if (this.InteractDecoyFortress(gameObject, triggerState))
                return;

            if (this.InteractCastle(gameObject, triggerState))
                return;
        }

        /// <summary>
        /// Enemyとの接触
        /// </summary>
        private bool InteractEnemy(GameObject gameObject, bool triggerState)
        {
            EnemyMovement enemyMovement = PlayerBehaviour.GetEnemy(gameObject);

            if (enemyMovement != null)
            {
                if (triggerState)
                {
                    this.interactingEnemies[enemyMovement.GetInstanceID().ToString()] = enemyMovement;

                    Debug.Log($"EnemyがPlayerBehaviourに接触しました！（ID: {enemyMovement.GetInstanceID()}）");
                }
                else
                {
                    this.interactingEnemies.Remove(enemyMovement.GetInstanceID().ToString());

                    Debug.Log($"EnemyがPlayerBehaviourを通過しました！（ID: {enemyMovement.GetInstanceID()}）");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// PlayerItemとの接触
        /// </summary>
        private bool InteractPlayerItem(GameObject gameObject, bool triggerState)
        {
            PlayerItemBehaviour playerItemBehaviour = PlayerBehaviour.GetPlayerItem(gameObject);

            if (playerItemBehaviour != null)
            {
                if (triggerState)
                {
                    this.interactingPlayerItems[playerItemBehaviour.GetInstanceID().ToString()] = playerItemBehaviour;

                    Debug.Log($"PlayerItemがPlayerBehaviourに接触しました！（ID: {playerItemBehaviour.GetInstanceID()}）");
                }
                else
                {
                    this.interactingPlayerItems.Remove(playerItemBehaviour.GetInstanceID().ToString());

                    Debug.Log($"PlayerItemがPlayerBehaviourを通過しました！（ID: {playerItemBehaviour.GetInstanceID()}）");
                }

                // アイテムの回収
                this.PickUpItem(playerItemBehaviour);

                return true;
            }

            return false;
        }

        /// <summary>
        /// DecoyFortressとの接触
        /// </summary>
        private bool InteractDecoyFortress(GameObject gameObject, bool triggerState)
        {
            DecoyFortressSetting decoyFortressSetting = PlayerBehaviour.GetDecoyFortress(gameObject);

            if (decoyFortressSetting != null)
            {
                if (triggerState)
                {
                    this.interactingDecoyFortresses[decoyFortressSetting.GetInstanceID().ToString()] = decoyFortressSetting;

                    Debug.Log($"DecoyFortressがPlayerBehaviourに接触しました！（ID: {decoyFortressSetting.GetInstanceID()}）");
                }
                else
                {
                    this.interactingDecoyFortresses.Remove(decoyFortressSetting.GetInstanceID().ToString());

                    Debug.Log($"DecoyFortresがPlayerBehaviourを通過しました！（ID: {decoyFortressSetting.GetInstanceID()}）");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Castleとの接触
        /// </summary>
        private bool InteractCastle(GameObject gameObject, bool triggerState)
        {
            CastleManager castleManager = PlayerBehaviour.GetCastle(gameObject);

            if (castleManager != null)
            {
                if (triggerState)
                {
                    this.interactingCastles[castleManager.GetInstanceID().ToString()] = castleManager;

                    Debug.Log($"CastleがPlayerBehaviourに接触しました！（ID: {castleManager.GetInstanceID()}）");
                }
                else
                {
                    this.interactingCastles.Remove(castleManager.GetInstanceID().ToString());

                    Debug.Log($"CastleがPlayerBehaviourを通過しました！（ID: {castleManager.GetInstanceID()}）");
                }
            }

            return false;
        }

        /// <summary>
        /// アイテムの回収
        /// </summary>
        private void PickUpItem(PlayerItemBehaviour playerItemBehaviour)
        {
            if (playerItemBehaviour != null && playerItemBehaviour.enabled)
            {
                if (this.AddItem(playerItemBehaviour.PlayerItemState) != -1)
                {
                    // PlayerItemBehaviourを消す
                    UnityEngine.Object.Destroy(playerItemBehaviour.gameObject);
                }
            }
        }

        /// <summary>
        /// UIの更新
        /// </summary>
        private void RepaintUI()
        {
            if (this.slotObject == null || this.canvas == null)
                return;

            // アイテムスロットの表示数
            int slotCounts = 7;

            // アイテムスロットのサイズ
            float slotSize = 128.0F;

            // 配列の初期化
            if (this.slotObjects == null)
            {
                this.slotObjects = new GameObject[slotCounts];
            }

            // アイテムスロットの位置を計算
            RectTransform canvasTransform = this.canvas.GetComponent<RectTransform>();
            float width = canvasTransform.sizeDelta.x;
            float height = canvasTransform.sizeDelta.y;
            float movement = this.SlotStep;
            float slotScale;
            float alphaScale;
            int offset = slotCounts >> 1;
            int order;

            GameObject slotObject;
            GameObject uiObject;
            Image uiImage;
            TextMeshProUGUI uiTextMeshPro;

            PlayerItemState playerItemState;
            PlayerItemRegistry.PlayerItemHolder itemHolder;
            PlayerItemRegistry.PlayerItemSpriteHolder spriteHolder;

            for (int i = 0; i < slotCounts; ++i)
            {
                slotObject = this.slotObjects[i];
                order = Mathf.Abs(offset - i);

                // アイテムスロットを生成
                if (slotObject == null)
                {
                    this.slotObjects[i] = slotObject = UnityEngine.Object.Instantiate(this.slotObject, Vector3.zero, Quaternion.identity, this.canvas.transform);
                }

                // アイテムスロットを移動
                slotScale = 1.0F - Mathf.Abs((i + movement - offset) / slotCounts);
                alphaScale = slotScale * slotScale;

                slotObject.transform.position = new(width - slotSize, height * 0.5F + (i + movement - offset) * slotSize * slotScale, 0.0F);
                slotObject.transform.localScale = new(slotScale * 0.5F, slotScale * 0.5F, 1.0F);
                slotObject.transform.SetSiblingIndex(order);

                // アイテムスロットのサイズ変更
                if (slotObject.transform is RectTransform slotTransform)
                {
                    slotTransform.sizeDelta = new(slotSize, slotSize);
                }

                // アイテムスロットの表示
                if (slotObject.transform.childCount >= 3)
                {
                    playerItemState = this.GetItem(this.SelectingSlot - offset + i);

                    // 背景を設定
                    uiObject = slotObject.transform.GetChild(0).gameObject;
                    uiImage = uiObject?.GetComponent<Image>();

                    if (uiObject.transform is RectTransform backgroundTransform)
                    {
                        backgroundTransform.sizeDelta = new(slotSize, slotSize);
                    }

                    if (uiImage != null)
                    {
                        uiImage.color = new(0.0F, 0.0F, 0.0F, 0.5F * alphaScale);
                    }

                    // アイテムの表示を設定
                    uiObject = slotObject.transform.GetChild(1).gameObject;
                    uiImage = uiObject?.GetComponent<Image>();

                    if (uiObject.transform is RectTransform displayTransform)
                    {
                        displayTransform.sizeDelta = new(slotSize, slotSize);
                    }

                    if (uiImage != null)
                    {
                        // 色の設定
                        uiImage.color = new(1.0F, 1.0F, 1.0F, 1.0F * alphaScale);

                        // スプライトの設定
                        spriteHolder = PlayerItemRegistry.INSTANCE.GetSprite(playerItemState.Id);

                        if (spriteHolder != null && spriteHolder.sprite != null)
                        {
                            uiImage.sprite = spriteHolder.sprite;
                            uiImage.enabled = true;
                        }
                        else
                        {
                            uiImage.sprite = null;
                            uiImage.enabled = false;
                        }
                    }

                    // アイテムの個数を設定
                    uiObject = slotObject.transform.GetChild(2).gameObject;
                    uiTextMeshPro = uiObject?.GetComponent<TextMeshProUGUI>();

                    if (uiObject.transform is RectTransform countTransform)
                    {
                        countTransform.sizeDelta = new(slotSize, slotSize);
                    }

                    if (uiTextMeshPro != null)
                    {
                        uiTextMeshPro.text = playerItemState.Count > 0 ? $"× {playerItemState.Count}" : "";
                        uiTextMeshPro.color = new(1.0F, 1.0F, 1.0F, 1.0F * alphaScale);
                    }

                    // アイテムの名前を設定
                    uiObject = slotObject.transform.GetChild(3).gameObject;
                    uiTextMeshPro = uiObject?.GetComponent<TextMeshProUGUI>();

                    if (uiObject.transform is RectTransform nameTransform)
                    {
                        nameTransform.sizeDelta = new(slotSize, slotSize);
                    }

                    if (uiTextMeshPro != null)
                    {
                        itemHolder = PlayerItemRegistry.INSTANCE.Get(playerItemState.Id);

                        if (itemHolder != null && itemHolder.playerItem != null)
                        {
                            uiTextMeshPro.text = itemHolder.playerItem.Name;
                        }
                        else
                        {
                            uiTextMeshPro.text = "Unknown";
                        }

                        uiTextMeshPro.color = new(1.0F, 1.0F, 1.0F, 1.0F * alphaScale);
                    }
                }
            }
        }

        /// <summary>
        /// <para>GameObjectからEnemyMovementを取得する</para>
        /// <para>できなかった場合はnull</para>
        /// </summary>
        public static EnemyMovement GetEnemy(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            HitboxMarker hitBoxMarker = gameObject.GetComponent<HitboxMarker>();

            if (hitBoxMarker != null && hitBoxMarker.enabled)
            {
                EnemyMovement enemyMovement = hitBoxMarker.GetComponentInParent<EnemyMovement>();

                if (enemyMovement != null && enemyMovement.enabled)
                {
                    return enemyMovement;
                }
            }

            return null;
        }

        /// <summary>
        /// <para>GameObjectからDecoyFortressSettingを取得する</para>
        /// <para>できなかった場合はnull</para>
        /// </summary>
        public static DecoyFortressSetting GetDecoyFortress(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            HitboxMarker hitBoxMarker = gameObject.GetComponent<HitboxMarker>();

            if (hitBoxMarker != null && hitBoxMarker.enabled)
            {
                DecoyFortressSetting decoyFortressSetting = hitBoxMarker.GetComponentInParent<DecoyFortressSetting>();

                if (decoyFortressSetting != null && decoyFortressSetting.enabled)
                {
                    return decoyFortressSetting;
                }
            }

            return null;
        }

        /// <summary>
        /// <para>GameObjectからPlayerItemBehaviourを取得する</para>
        /// <para>できなかった場合はnull</para>
        /// </summary>
        public static PlayerItemBehaviour GetPlayerItem(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            PlayerItemBehaviour playerItemBehaviour = gameObject.GetComponent<PlayerItemBehaviour>();

            return playerItemBehaviour != null && playerItemBehaviour.enabled ? playerItemBehaviour : null;
        }

        /// <summary>
        /// <para>GameObjectからCastleManagerを取得する</para>
        /// <para>できなかった場合はnull</para>
        /// </summary>
        public static CastleManager GetCastle(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            HitboxMarker hitBoxMarker = gameObject.GetComponent<HitboxMarker>();

            if (hitBoxMarker != null && hitBoxMarker.enabled)
            {
                CastleManager castleManager = hitBoxMarker.GetComponentInParent<CastleManager>();

                if (castleManager != null && castleManager.enabled)
                {
                    return castleManager;
                }
            }

            return null;
        }
    }
}