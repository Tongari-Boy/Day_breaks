using Play;
using Player;
using UnityEngine;

namespace Castle
{
    /// <summary>
    ///  城を管理するクラス
    /// </summary>
    public class CastleManager : MonoBehaviour,IDamageable
    {
        private GameManager gameManager;

        [Header("城のUI")]
        [SerializeField] private CastleUI castleUI;

        /// <summary>
        /// 城のHP
        /// 
        /// 初期値は城の最大値とします
        /// </summary>
        [Header("城のHP(ゲームスタート時に最大値で上書きされるため、変更しても意味はありません)")]
        [SerializeField] private int castleHP = 100;

        /// <summary>
        /// 城の最大HP
        /// </summary>
        [Header("城の最大HP")]
        [SerializeField] private int CastleMaxHP = 100;

        public void Initialize(GameManager gameManager)
        {
            this.gameManager = gameManager;

            castleHP = CastleMaxHP; 

            // UIの初期化
            if(castleUI != null)
            {
                castleUI.SetUp(CastleMaxHP, castleHP);
            }
        }

        /// <summary>
        /// 城がダメージを受けたときの処理
        /// (Enemyクラスなどから呼ばれる)
        /// 
        /// <param name = "damageAmount">受けたダメージ量</param>
        /// </summary>
        public void OnDamaged(int damageAmount)
        {
            if (castleHP - damageAmount <= 0)
            {
                castleHP = 0;
                if (castleUI != null) castleUI.UpdateHP(castleHP);
                Debug.Log("城が壊れた");
                gameManager.GameOver();
            }
            else
            {
                castleHP -= damageAmount;
                if (castleUI != null) castleUI.UpdateHP(castleHP);
                Debug.Log("現在の城のHP:" + castleHP);
            }
        }

    }
}