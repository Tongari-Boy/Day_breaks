using Enemy;
using System.Collections.Generic;
using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    /// 罠砦の効果範囲(攻撃/鈍足化)の検知を行う
    /// 
    /// 検知用にコリダー(円)、スプライトを
    /// 子オブジェクトとして用意し、アタッチする
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class DecoyFortressRange : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rangeSprite;

        private readonly List<EnemyMovement> enemiesInRange = new List<EnemyMovement>();

        private CircleCollider2D rangeCollider;

        private void Awake()
        {
            rangeCollider = GetComponent<CircleCollider2D>();
            rangeCollider.isTrigger = true;
        }

        /// <summary>
        /// 半径の設定
        /// 見た目のスプライトに反映させる
        /// </summary>
        /// <param name="radius"></param>
        public void SetRadius(float radius)
        {
            rangeCollider.radius = radius;

            if(rangeSprite != null)
            {
                // スプライトが直径1のcircle前提の処理
                float diameter = radius * 2f;
                rangeSprite.transform.localScale = new Vector3(diameter, diameter, 1f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // HitBoxMarkerのついたコリダーなら無視
            if (other.GetComponent<HitboxMarker>() == null) return;

            Debug.Log("Trigger Enter: " + other.name);
            var enemy = other.GetComponentInParent<EnemyMovement>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("Trigger Exit: " + other.name);
            var enemy = other.GetComponentInParent<EnemyMovement>();
            if (enemy != null)
            {
                enemiesInRange.Remove(enemy);
            }
        }

        public IReadOnlyList<EnemyMovement> GetEnemiesInRange()
        {
            enemiesInRange.RemoveAll(e =>
                e == null ||
                !e.gameObject.activeInHierarchy ||
                Vector2.Distance(e.transform.position, transform.position) > rangeCollider.radius
            );
            return enemiesInRange;
        }
    }
}