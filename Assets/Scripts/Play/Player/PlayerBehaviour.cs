using Player.Bullet;
using Player.Item;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
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
        [SerializeField] private float bulletSpawnDistance = 1.0F;

        [Header("射撃後のクールダウン（秒）")]
        [SerializeField] private float shootingCooldown = 1.0F;

        [Header("連射モード")]
        [SerializeField] private bool holdingShootingMode = true;

        [Header("プレイヤーのアイテムスロット")]
        [SerializeField] private PlayerItemState[] playerItemSlots = new PlayerItemState[0];

        [Header("プレイヤーのアイテムスロットのサイズ")]
        [SerializeField] private int playerItemSlotSize = 3;

        [Header("アイテム使用後のクールダウン（秒）")]
        [SerializeField] private float usingCooldown = 1.0F;

        private int remainingHealth;
        private float remainingUsingCooldown;
        private float remainingShootingCooldown;

        private InputAction move;
        private InputAction sprint;
        private InputAction use;
        private InputAction shoot;
        private InputAction cursor;

        private new Rigidbody2D rigidbody2D;

        /// <summary>
        /// <para>アイテムスロットに空きがあれば、アイテムを追加する</para>
        /// <para>同じアイテムを持っていれば、countだけ個数を増やす</para>
        /// <para>アイテムを追加できた場合はtrueを返す</para>
        /// </summary>
        public bool AddItem(string id, int count = 1)
        {
            this.Resize();

            if (id == null)
                return false;

            PlayerItemState playerItemState = new(id, count);
            PlayerItemState slotItemState;

            int length = this.playerItemSlots.Length;

            // 同じアイテムを探す
            for (int i = 0; i < length; ++i)
            {
                slotItemState = this.playerItemSlots[i];

                if (slotItemState.Equals(playerItemState))
                {
                    slotItemState.Count += count;

                    // アイテムの個数が0になった場合は空にする
                    if (slotItemState.Count <= 0)
                    {
                        this.playerItemSlots[i] = PlayerItemState.EMPTY;
                    }

                    return true;
                }
            }

            // 空のアイテムスロットを探す
            for (int i = 0; i < length; ++i)
            {
                slotItemState = this.playerItemSlots[i];

                if (slotItemState.Equals(PlayerItemState.EMPTY))
                {
                    this.playerItemSlots[i] = playerItemState;

                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// アイテムスロットのリサイズ
        /// </summary>
        private void Resize()
        {
            if (this.playerItemSlots.Length != this.playerItemSlotSize)
            {
                // 新しいサイズのアイテムスロットを生成
                PlayerItemState[] newPlayerItemSlots = new PlayerItemState[Math.Max(0, this.playerItemSlotSize)];
                int oldLength = this.playerItemSlots.Length;
                int newLength = newPlayerItemSlots.Length;

                // 要素をコピー
                for (int i = 0; i < newLength; ++i)
                {
                    if (i < oldLength)
                    {
                        newPlayerItemSlots[i] = this.playerItemSlots[i];
                    }
                    else
                    {
                        newPlayerItemSlots[i] = PlayerItemState.EMPTY;
                    }
                }

                this.playerItemSlots = newPlayerItemSlots;
            }
            else
            {
                int length = this.playerItemSlots.Length;

                // nullをPlayerItemState.EMPTYに置き換える
                for (int i = 0; i < length; ++i)
                {
                    if (this.playerItemSlots[i] == null)
                    {
                        this.playerItemSlots[i] = PlayerItemState.EMPTY;
                    }
                }
            }
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

            // Rigidbody2Dの取得
            this.rigidbody2D = this.GetComponent<Rigidbody2D>();

            // ステータスの初期化
            this.remainingHealth = this.health;
            this.remainingShootingCooldown = this.shootingCooldown;
        }

        public void Update()
        {
            this.Use();
            this.Shoot();
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
        /// IDamageableより実装
        /// </summary>
        public void OnDamaged(int damageAmount)
        {
            this.remainingHealth = Mathf.Clamp(this.remainingHealth - damageAmount, 0, this.health);
        }
    }
}