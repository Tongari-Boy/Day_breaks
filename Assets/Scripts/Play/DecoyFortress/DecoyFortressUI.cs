using UnityEngine;
using UnityEngine.UI;

namespace DecoyFortress
{
    /// <summary>
    /// 罠砦関係のUIを管理するクラス
    /// </summary>
    public class DecoyFortressUI : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;

        public void SetUp(int maxHP,int currentHP)
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
            if(hpSlider != null)
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