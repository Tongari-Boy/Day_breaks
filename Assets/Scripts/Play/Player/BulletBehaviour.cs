using Enemy;
using UnityEngine;

namespace Player.Bullet
{
    public class BulletBehaviour : MonoBehaviour
    {
        [Header("弾の攻撃力")]
        [SerializeField] private int attackDamage = 50;

        [Header("弾の寿命（秒）")]
        [SerializeField] private float duration = 10.0F;

        public int AttackDamage
        {
            get { return this.attackDamage; }
            set { this.attackDamage = value; }
        }

        public float Duration
        {
            get { return this.duration; }
            set { this.duration = value; }
        }

        /// <summary>
        /// 弾の寿命（残り）
        /// </summary>
        private float remaningDuration;

        public void Start()
        {
            // ステータスの初期化
            this.remaningDuration = this.duration;
        }

        public void Update()
        {
            // 寿命を減らす
            if (this.duration > 0.0F)
            {
                this.duration -= Time.deltaTime;
            }
            else
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            this.Attack(collision2D);
        }

        /// <summary>
        /// 衝突したものにダメージを与える
        /// </summary>
        private void Attack(Collision2D collision2D)
        {
            // HitboxMarkerとの衝突を検知
            HitboxMarker hitboxMarker = collision2D.gameObject.GetComponent<HitboxMarker>();

            if (hitboxMarker != null && hitboxMarker.enabled)
            {
                EnemyMovement enemyMovement = collision2D.gameObject.GetComponentInParent<EnemyMovement>();

                // HitboxMarkerがEnemyMovementの子であるかチェック
                if (enemyMovement != null && enemyMovement.enabled && enemyMovement.CompareTag("Enemy"))
                {
                    // EnemyMovementにダメージを与える
                    enemyMovement.OnDamagedByPlayer(this.attackDamage);
                }

                // 弾を消す
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }
}