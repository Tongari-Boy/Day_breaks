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
        [Header("DecoyFortressManagerをアタッチ")]
        [SerializeField] private DecoyFortress.DecoyFortressManager decoyFortressManager;

        public void Initialize()
        {
            //  城+罠砦をまとめて候補リストに
            var targets = new List<IDamageable> { castleManager };
            targets.AddRange(decoyFortressManager.GetAllAsDamageable());

            foreach(var enemy in enemies)
            {
                enemy.Initialize();
                enemy.SetTargetCandidates(targets);
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