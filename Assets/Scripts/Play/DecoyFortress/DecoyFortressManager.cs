using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    ///  罠砦を管理するクラス
    /// </summary>
    public class DecoyFortressManager : MonoBehaviour, IDamageable
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
    }
}