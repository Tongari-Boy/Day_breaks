using UnityEngine;
using System.Collections.Generic;
using Castle;

namespace Enemy
{
    /// <summary>
    ///  敵全体を管理するクラス
    ///  
    ///  スポーンが大きな管理内容
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("EnemyMovementの付いた敵をアタッチ")]
        [SerializeField] private List<EnemyMovement> enemies;

        /// <summary>
        /// 攻撃対象のクラス
        /// </summary>
        [Header("CastleManagerをアタッチ")]
        [SerializeField] private CastleManager castleManager;

        public void Initialize()
        {
            foreach(var enemy in enemies)
            {
                enemy.SetTargetCandidates(
                    new List<IDamageable>()
                    {
                        castleManager
                    }
                );
            }
        }


        public void OnUpdate()
        {
            foreach(var enemy in enemies)
            {
                enemy.OnUpdate();
            }
        }
    }
}