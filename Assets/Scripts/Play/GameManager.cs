using Castle;
using Enemy;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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

        [Header("ゲーム中フラグ")]
        [SerializeField] private bool gaming = false;

        [SerializeField] private SpriteRenderer backgroundPanel;

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

            GameStart();
        }

        void Update()
        {
            if (gaming)
            {
                PlayerManager.INSTANCE.OnUpdate();
                enemyManager.OnUpdate();
                enemySpawner.OnUpdate();
                gameTimer.OnUpdate();
                decoyFortressManager.OnUpdate();

                // 城の透明度適応
                float rate = gameTimer.GetTimeRate();
                // 残り15%から消え始める
                float alpha = Mathf.InverseLerp(0f, 0.15f, rate);
                castleManager.SetAlpha(alpha);

                backgroundPanel.color = Color.Lerp(
                    new Color32(0, 243, 255, 30),
                    new Color32(0,0,0,30),
                    alpha
                );
            }
        }

        /// <summary>
        /// ゲーム中フラグをオンにする
        /// これによりUpdateが動き始まる
        /// </summary>
        public void GameStart()
        {
            gaming = true;
        }

        /// <summary>
        /// ゲームクリア処理
        /// </summary>
        public void GameClear()
        {
            gaming = false;
            SceneManager.LoadScene("003_Result");
        }

        /// <summary>
        /// ゲームオーバー処理
        /// 
        /// 城が破壊された(HPが0になった)ら呼ばれる
        /// </summary>
        public void GameOver()
        {
            Debug.Log("城が破壊されたので、ゲームオーバー");
            gaming = false;

            SceneManager.LoadScene("004_GameOver");
        }
    }
}