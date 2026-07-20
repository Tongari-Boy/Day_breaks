using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    /// 罠砦の種類に応じた動きの処理
    /// </summary>
    public class DecoyFortressMovement : MonoBehaviour
    {
        /// <summary>
        /// 同オブジェクトのDecoyFortressSetting
        /// </summary>
        private DecoyFortressSetting setting;

        private void Awake()
        {
            setting = GetComponent<DecoyFortressSetting>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdate()
        {
            // 有効化フラグがfalseなら無視
            if (!setting.GetEnable()) return;

            switch (setting.GetID())
            {
                case DecoyFortressSetting.DecoyFortressIDs.Normal:
                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Stop:

                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Candle:

                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Sword:

                    break;
                case DecoyFortressSetting.DecoyFortressIDs.Bomb:

                    break;
            }
        }
    }
}
