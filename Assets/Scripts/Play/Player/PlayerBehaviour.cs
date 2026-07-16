using Player.Bullet;
using Player.Item;
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
        [SerializeField] private float bulletDuration = 10.0F;

        [Header("弾のスポーン位置までの距離")]
        [SerializeField] private float bulletSpawnDistance = 1.0F;

        [Header("射撃後のクールダウン")]
        [SerializeField] private float shootingCooldown = 1.0F;

        [Header("連射モード")]
        [SerializeField] private bool holdingShootingMode = true;

        [Header("プレイヤーの所持するアイテムスロット")]
        [SerializeField] private List<PlayerItemState> playerItemSlots = new();

        [Header("プレイヤーの選択中のアイテムスロット")]
        [SerializeField] private int bindingSlot = 0;

        [Header("アイテム使用後のクールダウン")]
        [SerializeField] private float usingCooldown = 1.0F;

        private int remainingHealth;
        private float remainingUsingCooldown;
        private float remainingShootingCooldown;

        private InputAction move;
        private InputAction sprint;
        private InputAction use;
        private InputAction shoot;
        private InputAction cursor;

        private Rigidbody2D rigidbody2D;

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

            GameObject bulletObject = UnityEngine.Object.Instantiate(this.bulletObject, shooterPos + aimDir * this.bulletSpawnDistance, Quaternion.identity);
            Rigidbody2D rigidbody2D = bulletObject.GetComponent<Rigidbody2D>();
            BulletBehaviour bulletBehaviour = bulletObject.GetComponent<BulletBehaviour>();

            if (rigidbody2D != null)
            {
                rigidbody2D.linearVelocity = aimDir * this.bulletSpeed;
            }

            if (bulletBehaviour != null && bulletBehaviour.enabled)
            {
                bulletBehaviour.AttackDamage = this.attackDamage;
                bulletBehaviour.Duration = this.bulletDuration;
            }

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