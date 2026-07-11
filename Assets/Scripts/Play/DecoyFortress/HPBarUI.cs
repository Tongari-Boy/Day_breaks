using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 体力バーの表示を管理する汎用ベースクラス
    /// canvasに対応しています。
    /// </summary>
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;

        public void SetUp(int maxHP, int currentHP)
        {
            if (hpSlider == null) hpSlider = GetComponentInChildren<Slider>();

            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHP;
                hpSlider.value = currentHP;
            }
        }

        public void UpdateHP(int currentHP)
        {
            if (hpSlider != null)
            {
                hpSlider.value = currentHP;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }

}