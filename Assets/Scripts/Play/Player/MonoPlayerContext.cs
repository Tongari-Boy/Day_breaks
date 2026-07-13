using Player.Item;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// IPlayerContextインターフェースを実装したMonoBehaviourクラス
    /// </summary>
    public class MonoPlayerContext : MonoBehaviour, IPlayerContext
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

        private float shootingCooldown = 0F;

        [Header("プレイヤーの射撃後のクールダウンの秒数")]
        [SerializeField] private float maxShootingCooldown = 0F;

        [Header("プレイヤーの所持するアイテムIDのリスト")]
        [SerializeField] private List<PlayerItemState> playerItemSlots = new();

        [Header("選択中のアイテムスロットの番号")]
        [SerializeField] private int bindingPlayerItemSlot = 0;

        public float Health
        {
            get { return health; }
            set { health = Mathf.Clamp(value, 0F, MaxHealth); }
        }

        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = Mathf.Max(0F, value); }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = Mathf.Max(0F, value); }
        }

        public float SprintSpeed
        {
            get { return sprintSpeed; }
            set { sprintSpeed = Mathf.Max(0F, value); }
        }

        public GameObject PlayerBullet
        {
            get { return playerBullet; }
            set { playerBullet = value; }
        }

        public GameObject PlayerBulletShooter
        {
            get { return playerBulletShooter; }
            set { playerBulletShooter = value; }
        }

        public float BulletSpawnDistance
        {
            get { return bulletSpawnDistance; }
            set { bulletSpawnDistance = Mathf.Max(0F, value); }
        }

        public float BulletSpeed
        {
            get { return bulletSpeed; }
            set { bulletSpeed = Mathf.Max(0F, value); }
        }

        public float ShootingCooldown
        {
            get { return shootingCooldown; }
            set { shootingCooldown = Mathf.Clamp(0F, value, MaxShootingCooldown); }
        }

        public float MaxShootingCooldown
        {
            get { return maxShootingCooldown; }
            set { maxShootingCooldown = Mathf.Max(value); }
        }

        public List<PlayerItemState> PlayerItemSlots
        {
            get { return playerItemSlots; }
        }

        public int BindingPlayerItemSlot
        {
            get { return bindingPlayerItemSlot; }
            set { bindingPlayerItemSlot = Mathf.Clamp(value, 0, PlayerItemSlots.Count - 1); }
        }
    }
}
