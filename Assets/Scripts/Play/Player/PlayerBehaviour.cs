using Physics;
using Player.Item;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// IPlayerContext、IDamageableを実装
    /// </summary>
    public class PlayerBehaviour : EntityBehaviour, IPlayerContext, IDamageable
    {
        [Header("プレイヤーの体力")]
        [SerializeField] private float health = 0F;

        [Header("プレイヤーの最大体力")]
        [SerializeField] private float maxHealth = 0F;

        [Header("通常移動の速度係数")]
        [SerializeField] private float movementSpeed = 1F;

        [Header("ダッシュ時の速度係数")]
        [SerializeField] private float sprintSpeed = 1F;

        [Header("プレイヤーの弾")]
        [SerializeField] private GameObject playerBullet;

        [Header("プレイヤーの弾を射撃するGameObject（シューター）")]
        [SerializeField] private GameObject playerBulletShooter;

        [Header("プレイヤーの弾がスポーンする位置までの距離（シューターの位置を基準とする）")]
        [SerializeField] private float bulletSpawnDistance = 1F;

        [Header("プレイヤーの弾の速度係数")]
        [SerializeField] private float bulletSpeed = 1F;

        [Header("プレイヤーの射撃後のクールダウンの残り秒数")]
        [SerializeField] private float shootingCooldown = 0F;

        [Header("プレイヤーの射撃後のクールダウンの秒数")]
        [SerializeField] private float maxShootingCooldown = 0F;

        [Header("プレイヤーの所持するアイテムIDのリスト")]
        [SerializeField] private List<PlayerItemState> playerItemSlots = new();

        [Header("選択中のアイテムスロットの番号")]
        [SerializeField] private int bindingPlayerItemSlot = 0;

        public new void Start()
        {
            // 摩擦係数の初期化
            this.Entity.Friction = 0.95F;

            base.Start();
        }

        public float Health
        {
            get { return this.health; }
            set { this.health = Mathf.Clamp(value, 0F, this.MaxHealth); }
        }

        public float MaxHealth
        {
            get { return this.maxHealth; }
            set { this.maxHealth = Mathf.Max(0F, value); }
        }

        public float MovementSpeed
        {
            get { return this.movementSpeed; }
            set { this.movementSpeed = Mathf.Max(0F, value); }
        }

        public float SprintSpeed
        {
            get { return this.sprintSpeed; }
            set { this.sprintSpeed = Mathf.Max(0F, value); }
        }

        public GameObject PlayerBullet
        {
            get { return this.playerBullet; }
            set { this.playerBullet = value; }
        }

        public GameObject PlayerBulletShooter
        {
            get { return this.playerBulletShooter; }
            set { this.playerBulletShooter = value; }
        }

        public float BulletSpawnDistance
        {
            get { return this.bulletSpawnDistance; }
            set { this.bulletSpawnDistance = Mathf.Max(0F, value); }
        }

        public float BulletSpeed
        {
            get { return this.bulletSpeed; }
            set { this.bulletSpeed = Mathf.Max(0F, value); }
        }

        public float ShootingCooldown
        {
            get { return this.shootingCooldown; }
            set { this.shootingCooldown = Mathf.Clamp(0F, value, this.MaxShootingCooldown); }
        }

        public float MaxShootingCooldown
        {
            get { return this.maxShootingCooldown; }
            set { this.maxShootingCooldown = Mathf.Max(value); }
        }

        public List<PlayerItemState> PlayerItemSlots
        {
            get { return this.playerItemSlots; }
        }

        public int BindingPlayerItemSlot
        {
            get { return this.bindingPlayerItemSlot; }
            set { this.bindingPlayerItemSlot = Mathf.Clamp(value, 0, this.PlayerItemSlots.Count - 1); }
        }

        /// <summary>
        /// IDamageableより実装
        /// </summary>
        public void OnDamaged(int damageAmount)
        {
            this.Health -= damageAmount;
        }

        /// <summary>
        /// IDamageableより実装
        /// </summary>
        public new UnityEngine.Transform transform
        {
            get { return base.transform; }
        }
    }
}