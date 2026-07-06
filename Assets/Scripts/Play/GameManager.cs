using Castle;
using Enemy;
using UnityEngine;


namespace Play
{
    /// <summary>
    /// ゲームプレイ中の進行を管理するクラス
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("CastleManagerをアタッチ")]
        [SerializeField] private CastleManager castleManager;

        [Header("EnemyManagerをアタッチ")]
        [SerializeField] private EnemyManager enemyManager;

        void Awake()
        {
            castleManager.Initialize(this);

            enemyManager.Initialize();
        }

        void Update()
        {
            enemyManager.OnUpdate();
        }

        /// <summary>
        /// ゲームオーバー処理
        /// 
        /// 城が破壊された(HPが0になった)ら呼ばれる
        /// </summary>
        public void GameOver()
        {
            Debug.Log("城が破壊されたので、ゲームオーバー");
        }

    }
}