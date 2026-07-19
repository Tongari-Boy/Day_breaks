using Castle;
using DecoyFortress;
using Enemy;
using Player.Bullet;
using Player.Item;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("プレイヤーのアイテムスロット")]
        [SerializeField] private PlayerItemState[] playerItemSlots = new PlayerItemState[0];

        [Header("プレイヤーのマウスホイールの感度")]
        [SerializeField] private float mouseWheelSensitivity = 0.25F;

        [Header("アイテム使用後のクールダウン（秒）")]
        [SerializeField] private float usingCooldown = 1.0F;

        private int remainingHealth;
        private float remainingShootingCooldown;
        private float remainingUsingCooldown;
        private float selectingSlotPos;

        private InputAction move;
        private InputAction sprint;
        private InputAction use;
        private InputAction shoot;
        private InputAction cursor;
        private InputAction scroll;

        private new Rigidbody2D rigidbody2D;

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
            get { return (PlayerItemState[])this.playerItemSlots.Clone(); }
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
                playerItemState.Count = 0;

                Debug.Log($"プレイヤーがアイテム（ID: {playerItemState.Id}）を{playerItemState.Count}個、回収しました！");
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

            for (int i = this.playerItemSlots.Length - 1; i >= 0; --i)
            {
                playerItemState = this.playerItemSlots[i];

                if (playerItemState == null || playerItemState.Id == PlayerItemState.EMPTY.Id)
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
                this.playerItemSlots[empty] = new PlayerItemState(id, count);

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
            int length = this.playerItemSlots.Length;

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
            if (this.playerItemSlots.Length > 0)
            {
                int index = slot % this.playerItemSlots.Length;

                if (index < 0)
                {
                    index += this.playerItemSlots.Length;
                }

                return this.PlayerItemSlots[index] ?? PlayerItemState.EMPTY;
            }

            return PlayerItemState.EMPTY;
        }

        /// <summary>
        /// プレイヤーが選択しているスロット番号を取得する
        /// </summary>
        /// <returns></returns>
        public int GetSelectingSlot()
        {
            return Mathf.FloorToInt(this.selectingSlotPos * this.playerItemSlots.Length);
        }

        /// <summary>
        /// IDamageableより実装
        /// </summary>
        public void OnDamaged(int damageAmount)
        {
            this.remainingHealth = Mathf.Clamp(this.remainingHealth - damageAmount, 0, this.health);
        }

        public void Start()
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

            // ステータスの初期化
            this.remainingHealth = this.health;
            this.remainingShootingCooldown = this.shootingCooldown;
        }

        public void Update()
        {
            this.Select();
            this.Use();
            this.Shoot();
        }

        public void FixedUpdate()
        {
            this.Move();
            this.Follow();
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
                this.PickUp(playerItemBehaviour);

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
            // 入力を確認
            if (this.scroll == null)
                return;

            Vector2 scrollVelocity = this.scroll.ReadValue<Vector2>();

            // スクロール量を確認
            if (scrollVelocity.sqrMagnitude <= 0.0F)
                return;

            // 選択中のアイテムスロットの位置を更新
            this.selectingSlotPos += this.playerItemSlots.Length == 0 ? 0.0F : scrollVelocity.normalized.y / this.playerItemSlots.Length * this.mouseWheelSensitivity;
            this.selectingSlotPos %= 1.0F;

            if (this.selectingSlotPos < 0.0F)
            {
                this.selectingSlotPos += 1.0F;
            }

            // 誤差を補正
            this.selectingSlotPos = Mathf.Clamp(this.selectingSlotPos, 0.0F, 1.0F);

            Debug.Log(this.GetSelectingSlot());
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

            PlayerItemState playerItemState = this.GetItem(this.GetSelectingSlot());

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

        /// <summary>
        /// アイテムの回収
        /// </summary>
        private void PickUp(PlayerItemBehaviour playerItemBehaviour)
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