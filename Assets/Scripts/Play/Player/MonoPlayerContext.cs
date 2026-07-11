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
        [SerializeField] private GameObject playerShooter;

        [Header("プレイヤーの弾がスポーンする発射点からの距離（シューターの位置を基準とする）")]
        [SerializeField] private float bulletSpawnDistance = 1F;

        [Header("プレイヤーの弾の速度係数")]
        [SerializeField] private float bulletSpeed = 1F;

        private float shootingCooldown = 0F;

        [Header("プレイヤーの射撃後のクールダウンの秒数")]
        [SerializeField] private float maxShootingCooldown = 0F;

        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; }
        }

        public float SprintSpeed
        {
            get { return sprintSpeed; }
            set { sprintSpeed = value; }
        }

        public GameObject PlayerBullet
        {
            get { return playerBullet; }
            set { playerBullet = value; }
        }

        public GameObject PlayerShooter
        {
            get { return playerShooter; }
            set { playerShooter = value; }
        }

        public float BulletSpawnDistance
        {
            get { return bulletSpawnDistance; }
            set { bulletSpawnDistance = value; }
        }

        public float BulletSpeed
        {
            get { return bulletSpeed; }
            set { bulletSpeed = value; }
        }

        public float ShootingCooldown
        {
            get { return shootingCooldown; }
            set { shootingCooldown = value; }
        }

        public float MaxShootingCooldown
        {
            get { return maxShootingCooldown; }
            set { maxShootingCooldown = value; }
        }
    }
}
