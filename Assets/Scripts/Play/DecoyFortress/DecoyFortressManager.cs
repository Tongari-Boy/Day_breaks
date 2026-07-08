using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    ///  罠砦を管理するクラス
    /// </summary>
    public class DecoyFortressManager : MonoBehaviour, IDamageable, IEnable
    {

        /// <summary>
        /// 罠砦のHP
        /// </summary>
        private int fortressHP = 50;

        /// <summary>
        /// 罠砦の最大HP
        /// </summary>
        private int FortressMaxHP = 50;

        /// <summary>
        /// 罠砦が有効化どうか
        /// 
        /// 無効の場合は敵の攻撃対象にならず、
        /// プレイヤー一定時間が近づくと有効化される。
        /// </summary>
        [SerializeField] private bool FortressEnabled = false;

        /// <summary>
        /// 罠砦の初期化処理
        /// 
        /// HPを最大値に上書きする
        /// </summary>
        public void Initialize()
        {
            fortressHP = FortressMaxHP;
        }

        /// <summary>
        /// 罠砦がダメージを受けたときの処理
        /// (Enemyクラスなどから呼ばれる)
        /// 
        /// </summary>
        /// <param name="damageAmount">受けたダメージ量</param>
        public void OnDamaged(int damageAmount)
        {
            if(fortressHP - damageAmount <= 0)
            {
                fortressHP = 0;
                Debug.Log("罠砦が壊れた");
            }
            else
            {
                fortressHP -= damageAmount;
                Debug.Log("現在の罠砦のHP:" + fortressHP);
            }
        }
        
        /// <summary>
        /// 罠砦の有効化状隊を取得する
        /// 
        /// 敵の探索の際に用いる
        /// </summary>
        /// <returns>罠砦の有効化状隊</returns>
        public bool GetEnable()
        {
            return FortressEnabled;
        }
    }
}