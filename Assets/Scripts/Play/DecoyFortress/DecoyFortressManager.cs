using System.Collections.Generic;
using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    ///  罠砦全体を管理するクラス
    /// </summary>
    public class DecoyFortressManager : MonoBehaviour
    {
        [Header("DecoyFortressSettingのついた罠砦をすべてアタッチ")]
        [SerializeField]private List<DecoyFortressSetting> fortresses;

        private List<DecoyFortressMovement> movements;

        public void Initialize()
        {
            movements = new List<DecoyFortressMovement>();

            foreach (var fortress in fortresses)
            {
                fortress.Initialize();

                // 同じGameObjectについているMovementを自動取得
                var movement = fortress.GetComponent<DecoyFortressMovement>();
                if (movement != null)
                {
                    movements.Add(movement);
                }
                else
                {
                    Debug.LogWarning($"{fortress.name} に DecoyFortressMovement がアタッチされていません");
                }
            }
        }

        public void OnUpdate()
        {
            foreach (var movement in movements)
            {
                movement.OnUpdate();
            }

            foreach (var fortress in fortresses)
            {
                fortress.OnUpdate();
            }
        }

        /// <summary>
        /// 敵の攻撃対象候補として渡すためのリストを取得
        /// </summary>
        public List<IDamageable> GetAllAsDamageable()
        {
            return fortresses.ConvertAll(f => (IDamageable)f);
        }
    }
}