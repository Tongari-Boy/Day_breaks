using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    /// <summary>
    ///  敵を管理するクラス
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
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
        /// 追跡/攻撃候補のリスト
        /// </summary>
        private List<IDamageable> targetCandidates = new List<IDamageable>();

        private IDamageable currentTarget;

        private float attackTimer;


        /// <summary>
        /// 外部から追跡/攻撃対象の候補を登録
        /// </summary>
        /// <param name="canduadates"></param>
        public void SetTargetCandidates(List<IDamageable> canduadates)
        {
            targetCandidates = canduadates;
        }
    }
}