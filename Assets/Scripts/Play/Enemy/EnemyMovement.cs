using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    /// <summary>
    ///  敵を管理するクラス
    /// </summary>
    public class EnemyMovement : MonoBehaviour
    {
        /// <summary>
        /// 敵の状態を表す列挙型
        /// </summary>
        public enum EnemyState
        {
            Chasing,  // 追跡中
            Attacking // 攻撃中
        }

        /// <summary>
        /// 敵のHP
        /// </summary>
        [Header("敵のHP")]
        [SerializeField] private int enemyHP = 100;

        /// <summary>
        /// 敵の移動速度
        /// </summary>
        [Header("敵の移動速度")]
        [SerializeField] private float enemySpeed = 5f;

        /// <summary>
        /// 敵が与えるダメージ量
        /// </summary>
        [Header("敵が与えるダメージ量")]
        [SerializeField] private int enemyDamage = 10;

        /// <summary>
        /// 敵の攻撃範囲
        /// </summary>
        [Header("敵の攻撃範囲")]
        [SerializeField] private float enemyAttackRange = 1.5f;

        /// <summary>
        /// 敵の攻撃頻度(クールダウン)
        /// </summary>
        [Header("敵の攻撃頻度(クールダウン)")]
        [SerializeField] private float enemyCoolDown = 2f;

        /// <summary>
        /// 敵の現在の状隊
        /// </summary>
        private EnemyState currentState = EnemyState.Chasing;

        /// <summary>
        /// 追跡/攻撃候補のリスト
        /// </summary>
        private List<IDamageable> targetCandidates = new List<IDamageable>();

        private IDamageable currentTarget;

        private float attackTimer;

        public void OnUpdate()
        {
            switch (currentState)
            {
                case EnemyState.Chasing:
                    HandleChasing();
                    break;

                case EnemyState.Attacking:
                    HandleAttacking();
                    break;
            }
        }


        /// <summary>
        /// 外部から追跡/攻撃対象の候補を登録
        /// </summary>
        /// <param name="canduadates"></param>
        public void SetTargetCandidates(List<IDamageable> canduadates)
        {
            targetCandidates = canduadates;
        }


        /// <summary>
        /// 追跡処理
        /// </summary>
        private void HandleChasing()
        {
            // ターゲットが存在しない場合、再探索
            if (currentTarget == null || currentTarget.transform == null)
            {
                FindNearestTarget();
                if (currentTarget == null) return;  // ターゲットが全くない場合
            }

            // ターゲットの方向へ移動
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            transform.position += direction * enemySpeed * Time.deltaTime;

            // 距離判定
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance < enemyAttackRange)
            {
                currentState = EnemyState.Attacking;
                attackTimer = enemyCoolDown; // 攻撃のクールダウンタイマーをリセット
            }
        }

        /// <summary>
        /// 最も近いターゲットを探し、ターゲットに設定する
        /// </summary>
        private void FindNearestTarget()
        {
            if (targetCandidates == null || targetCandidates.Count == 0)
            {
                currentTarget = null;
                return;
            }

            IDamageable nearestTarget = null;
            float shortestDistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (var candidate in targetCandidates)
            {
                // すでに破壊されているものはスキップ
                if (candidate == null || candidate.transform == null) continue;

                float distance = Vector3.Distance(currentPosition, candidate.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = candidate;
                }
            }

            currentTarget = nearestTarget;
        }


        /// <summary>
        /// 攻撃処理
        /// </summary>
        private void HandleAttacking()
        {
            // 攻撃中にターゲットが消滅した場合、追跡状隊に
            if (currentTarget == null || currentTarget.transform == null)
            {
                currentState = EnemyState.Chasing;
                return;
            }

            // 攻撃のクールダウンタイマーの進行
            attackTimer += Time.deltaTime;

            if (attackTimer >= enemyCoolDown)
            {
                Attack();
                attackTimer = 0f; // クールダウンタイマーをリセット

                // 攻撃直後に距離判定を行い、離れていた場合、追跡状隊に
                float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (distance > enemyAttackRange)
                {
                    currentState = EnemyState.Chasing;
                }
            }
        }


        /// <summary>
        /// 攻撃の実行
        /// </summary>
        private void Attack()
        {
            // 攻撃の瞬間にもう一度距離と存在を確認(空振りを防止するため)
            if (currentTarget != null && currentTarget.transform != null)
            {
                // 敵から攻撃対象までの距離
                float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

                if (distance < enemyAttackRange)
                {
                    // 目標のHPを減らす
                    currentTarget.OnDamaged(enemyDamage);
                    Debug.Log($"{currentTarget.transform.name}に{enemyDamage}のダメージを与えた");
                }
            }
        }
    }
}