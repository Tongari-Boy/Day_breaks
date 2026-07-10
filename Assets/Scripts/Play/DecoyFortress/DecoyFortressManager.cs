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

        public void Initialize()
        {
            foreach (var fortress in fortresses)
            {
                fortress.Initialize();
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