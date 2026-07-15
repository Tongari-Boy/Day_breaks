using Castle;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Enemy
{
    /// <summary>
    ///  敵全体を管理するクラス
    ///  
    ///  スポーンが大きな管理内容
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("あらかじめ配置する敵")]
        [SerializeField] private List<EnemyMovement> enemies = new();

        /// <summary>
        /// 攻撃対象のクラス
        /// </summary>
        [Header("CastleManagerをアタッチ")]
        [SerializeField] private CastleManager castleManager;
        [Header("DecoyFortressManagerをアタッチ")]
        [SerializeField] private DecoyFortress.DecoyFortressManager decoyFortressManager;
        [Header("Playerをアタッチ")]
        [SerializeField] private Player.MonoPlayerContext player ;

        private List<IDamageable> targets;

        public void Initialize()
        {
            //  城+罠砦をまとめて候補リストに
            targets = new List<IDamageable> { castleManager };
            targets.AddRange(decoyFortressManager.GetAllAsDamageable());
            // targets.AddRange {player};

            foreach(var enemy in enemies)
            {
                SetupEnemy(enemy);
            }
        }

        /// <summary>
        /// スポナーなどから、新しく生成された敵を登録する
        /// </summary>
        public void RegisterEnemy(EnemyMovement enemy)
        {
            enemies.Add(enemy);
            SetupEnemy(enemy);
        }

        private void SetupEnemy(EnemyMovement enemy)
        {
            enemy.Initialize();
            enemy.SetTargetCandidates(targets);
        }


        public void OnUpdate()
        {
            // 破棄済みの敵をリストから取り除く(後ろから走査して安全に削除)
            for(int i = enemies.Count -1; i >= 0; i--)
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                enemies[i].OnUpdate();
            }
        }
    }
}