using Player.Item;
using System.Collections.Generic;
using UnityEngine;

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
        /// 敵の種類を定義
        /// 
        /// 種類に応じたアイテムを落とす
        /// </summary>
        public enum EnemyIDs
        {
            Normal,
            Speed,      // Normalの1.5倍のスピード
            Powerful,   // Normalの1.5倍の攻撃力
            Large,      // でかい
            Blinded     // 透明になる
        }

        /// <summary>
        /// 敵の種別
        /// 
        /// デフォルトはNormal
        /// </summary>
        [Header("種別")]
        [SerializeField] EnemyIDs enemyID = EnemyIDs.Normal;

        /// <summary>
        /// 敵のHP
        /// </summary>
        private int enemyHP = 3;
        
        /// <summary>
        /// 敵の移動速度
        /// </summary>
        private float enemySpeed = 1f;

        /// <summary>
        /// 敵が与えるダメージ量
        /// 
        /// 城の最大体力(100)の5%
        /// </summary>
        private float  enemyDamage = 5;

        /// <summary>
        /// 敵の攻撃頻度(クールダウン)
        /// </summary>
        private float enemyCoolDown = 1f;

        /// <summary>
        /// 敵の攻撃範囲
        /// </summary>
        [Header("敵の攻撃範囲")]
        [SerializeField] private float enemyAttackRange = 1.5f;

        [Header("敵の追跡から止まるまでの範囲(攻撃範囲以下にしないと正常に動作しません)")]
        [SerializeField] private float enemyChasingRange = 1f;

        [Header("自身のコリダーの判定")]
        [SerializeField] private float selfRadius = 10f;

        /// <summary>
        /// 敵の現在の状隊
        /// </summary>
        private EnemyState currentState = EnemyState.Chasing;

        /// <summary>
        /// 追跡/攻撃候補のリスト
        /// </summary>
        private List<IDamageable> targetCandidates = new List<IDamageable>();

        private IDamageable currentTarget;

        [Header("再探索の間隔(秒)")]
        [SerializeField] private float retargetInterval = 1f;

        private float retargetTimer;

        private float attackTimer;

        /// <summary>
        /// 透明敵用:子オブジェクトのスプライトを参照
        /// </summary>
        private SpriteRenderer spriteRenderer;
        private float fadeTimer = 0;
        private float currentAlpha = 1.0f;

        public void Initialize()
        {
            // 種別に応じたステータスに変更
            switch(enemyID)
            {
                case EnemyIDs.Normal:
                    break;
                case EnemyIDs.Speed:
                    enemySpeed *= 1.5f;
                    break;
                case EnemyIDs.Powerful:
                    enemyHP = 4;
                    enemyDamage *= 1.5f;
                    enemyAttackRange = 0.8f;
                    break;
                case EnemyIDs.Large:
                    enemyHP = 25;
                    enemyDamage = 7.5f;
                    enemyDamage = 5;
                    break;
                case EnemyIDs.Blinded:
                    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                    enemyHP = 2;
                    enemyDamage *= 0.8f;
                    enemySpeed *= 0.8f;
                    break;
            }

            var hitboxMarker = GetComponentInChildren<HitboxMarker>();

            if (hitboxMarker == null) return;

            var myCollider = hitboxMarker.HitCollider;

            if (myCollider is CircleCollider2D circle)
            {
                selfRadius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);
            }
        }

        public void OnUpdate()
        {
            if(enemyID == EnemyIDs.Blinded)
            {
                FadeInFadeOut();
            }

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
            if (!IsTargetValid(currentTarget))
            {
                FindNearestTarget();
                if (currentTarget == null) return;  // ターゲットが全くない場合
            }
            else
            {
                // 一定時間でより近い有効なターゲットがいないか再探索
                retargetTimer += Time.deltaTime;
                if (retargetTimer >= retargetInterval)
                {
                    retargetTimer = 0f;
                    FindNearestTarget();
                }
            }

            // 移動前に、表面までの距離が攻撃範囲より大きいかを判定
            float distance = GetSurfaceDistance(currentTarget);

            if (distance < enemyAttackRange)
            {
                currentState = EnemyState.Attacking;
                attackTimer = enemyCoolDown;
                return;
            }

            // ターゲットの方向へ移動
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;

            // 移動してもいい最大量(行きすぎない量)
            float moveAmount = Mathf.Min(enemySpeed * Time.deltaTime, distance - enemyChasingRange);
            moveAmount = Mathf.Max(moveAmount, 0f); //　負値を防ぐ

            this.transform.position += moveAmount * direction;
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
                if (!IsTargetValid(candidate)) continue;

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
            if (!IsTargetValid(currentTarget))
            {
                currentTarget = null;
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
                if (!IsInAttackRange(currentTarget))
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
                if (currentTarget != null && currentTarget.transform != null && IsInAttackRange(currentTarget))
                {
                    // 目標のHPを減らす
                    currentTarget.OnDamaged(enemyDamage);
                    Debug.Log($"{currentTarget.transform.name}に{enemyDamage}のダメージを与えた");
                }
            }
        }

        
        /// <summary>
        /// 現在のターゲットが有効かどうかを判定する
        /// (破壊、無効化された場合はfalse)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsTargetValid(IDamageable target)
        {
            // すでに破壊されているものはスキップ
            if (target == null || target.transform == null) return false;

            // 無効化されている対象はスキップ(有効化前の罠砦など)
            if (target is IEnable enable && !enable.GetEnable()) return false;

            return true;
        }

        private float GetSurfaceDistance(IDamageable target)
        {
            var hitBoxMarker = target.transform.GetComponentInChildren<HitboxMarker>();
            if(hitBoxMarker != null)
            {
                // ターゲットのコリダーの表面までの最短距離
                Vector2 closePoint = hitBoxMarker.HitCollider.ClosestPoint(transform.position);
                return Vector2.Distance(transform.position, closePoint) - selfRadius;
           }

            // コリダーがない場合は中心点間の距離にフォールバック
            return Vector2.Distance(transform.position,target.transform.position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">追跡/攻撃対象</param>
        /// <returns>追跡/攻撃対象の表面までの距離</returns>
        private bool IsInAttackRange(IDamageable target)
        {
            return GetSurfaceDistance(target) < enemyAttackRange;
        }

        /// <summary>
        /// プレイヤ、罠砦から攻撃を受けたときの処理
        /// </summary>
        /// <param name="amount"></param>
        public void OnDamagedByPlayer(int amount)
        {
            // 透明敵は透明度が0.8未満なら攻撃を受けない
            if (enemyID == EnemyIDs.Blinded && currentAlpha < 0.8f) return;

            if(enemyHP - amount > 0)
            {
                enemyHP -= amount;
            }
            else
            {
                enemyHP = 0;
                // 種別に応じたアイテムの生成
                switch(enemyID)
                {
                    case EnemyIDs.Normal:
                        ItemDropSpawner.INSTANCE.Drop("candle_decoy_fortress_regenerator", transform.position);
                        break;
                    case EnemyIDs.Speed:
                        ItemDropSpawner.INSTANCE.Drop("stop_decoy_fortress_regenerator", transform.position);
                        break;
                    case EnemyIDs.Powerful:
                        ItemDropSpawner.INSTANCE.Drop("sword_decoy_fortress_regenerator", transform.position);
                        break;
                }
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 罠砦の鈍足化効果による移動速度のダウン
        /// </summary>
        public void ApplySlow()
        {
            enemySpeed *= 0.8f;
        }

        /// <summary>
        /// 透明な敵(Blinded)の動き
        /// 
        /// 1秒ごとにフェードインフェードアウト
        /// </summary>
        private void FadeInFadeOut()
        {
            if (spriteRenderer == null) return;

            fadeTimer += Time.deltaTime;

            float t = Mathf.PingPong(fadeTimer, 1f);

            currentAlpha = Mathf.Lerp(0.0f, 1.0f, t);

            Color color = spriteRenderer.color;
            color.a = currentAlpha;

            spriteRenderer.color = color;
        }
    }
}