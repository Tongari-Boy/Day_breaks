using System.Linq;
using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    /// 罠砦の種類に応じた動きの処理
    /// </summary>
    public class DecoyFortressMovement : MonoBehaviour
    {
        [SerializeField] private DecoyFortressRange range;
        
        /// <summary>
        /// 同オブジェクトのDecoyFortressSetting
        /// </summary>
        private DecoyFortressSetting setting;

        private void Awake()
        {
            setting = GetComponent<DecoyFortressSetting>();
        }

        /// <summary>
        /// 更新処理
        /// 砦の種類に応じた動きを行う
        /// </summary>
        public void OnUpdate()
        {
            // 有効化フラグがfalseなら無視
            if (!setting.GetEnable()) return;

            switch (setting.GetID())
            {

                case DecoyFortressSetting.DecoyFortressIDs.Stop:
                    SpeedDown();
                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Candle:
                    Attack(2);
                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Sword:
                    Attack(1);
                    break;
            }
        }

        /// <summary>
        /// candle,swordの砦の動き
        /// 
        /// 範囲内の敵にダメージを与える
        /// </summary>
        /// <param name="amount">攻撃力</param>
        private void Attack(int amount)
        {
            // ToListで複製してから回すことで、
            // 元のリストの変更による影響を受けない
            var targets = range.GetEnemiesInRange().ToList();
            foreach (var enemy in targets)
            {
                Debug.Log($"攻撃対象: {enemy.name} / 位置: {enemy.transform.position} / 罠砦位置: {transform.position}");
                enemy.OnDamagedByPlayer(amount);
            }
        }

        /// <summary>
        /// stopの砦の動き
        /// 
        /// 範囲内の敵の素早さを遅くする
        /// </summary>
        private void SpeedDown()
        {
            foreach (var enemy in range.GetEnemiesInRange().ToList())
            {
                enemy.ApplySlow();
            }
        }
    }
}
