using Castle;
using Enemy;
using Player;
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
        [Header("EnemySpawnをアタッチ")]
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("DecoyFortressManagerをアタッチ")]
        [SerializeField] private DecoyFortress.DecoyFortressManager decoyFortressManager;
        [Header("Timer")]
        [SerializeField] private GameTimer gameTimer;

        void Awake()
        {
            PlayerManager.INSTANCE.Initialize();
            // HPが初期化される前の状態を拾ってしまう可能性があるため、
            // enemyManagerよりcastleManager、decoyFortressManagerの初期化を先に行う
            castleManager.Initialize(this);
            decoyFortressManager.Initialize();

            enemyManager.Initialize();
            enemySpawner.Initialize();

            gameTimer.Initialize();
        }

        void Update()
        {
            PlayerManager.INSTANCE.OnUpdate();
            enemyManager.OnUpdate();
            enemySpawner.OnUpdate();
            gameTimer.OnUpdate();
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