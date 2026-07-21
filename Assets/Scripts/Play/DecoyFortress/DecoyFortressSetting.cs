using UnityEngine;

namespace DecoyFortress
{
    /// <summary>
    /// 罠砦一体ごとの状態・ロジックを管理するクラス
    /// 
    /// </summary>
    public class DecoyFortressSetting : MonoBehaviour, IDamageable, IEnable
    {
        /// <summary>
        /// 罠砦の種類の定義
        /// </summary>
        public enum DecoyFortressIDs
        {
            Normal,
            Stop,       // 鈍足化効果のタイマー
            Candle,     // ろうそく(攻撃力小)
            Sword,      // 剣(攻撃力大)
            Bomb        // デモ:爆発
        }

        [Header("UI設定")]
        [SerializeField] private GameObject uiPrefab; // DecoyFortessUIがアタッチされたPrefab
        [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // 砦の上のオフセット値

        /// <summary>
        /// 罠砦のHP
        /// </summary>
        private float fortressHP = 50;

        /// <summary>
        /// 罠砦の最大HP
        /// </summary>
        private float FortressMaxHP = 50;

        /// <summary>
        /// 罠砦が有効化どうか
        /// 
        /// 無効の場合は敵の攻撃対象にならず、
        /// プレイヤー一定時間が近づくと有効化される。
        /// </summary>
        [SerializeField] private bool fortressEnabled = false;

        /// <summary>
        /// 生成したUIの参照を保持する変数
        /// </summary>
        private DecoyFortressUI fortressUI;

        /// <summary>
        /// 罠砦の種類
        /// 
        /// 初期値はNormal
        /// </summary>
        [SerializeField] private DecoyFortressIDs fotressID = DecoyFortressIDs.Normal;
        /// <summary>
        /// 罠砦の初期化処理
        /// 
        /// HPを最大値に上書きする
        /// </summary>
        public void Initialize()
        {
            fortressHP = FortressMaxHP;

            // 有効フラグの状態に合わせてUIを動的生成・初期化
            HandleUIInitialization();
        }

        /// <summary>
        /// UIの生成と初期状態の制御
        /// </summary>
        private void HandleUIInitialization()
        {
            // すでにUIが生成されている場合は一旦破棄(リトライなどの考慮)
            if (fortressUI != null) Destroy(fortressUI.gameObject);

            if(uiPrefab != null)
            {
                // 砦の子要素としてUIを生成
                // 第3引数にfalseを指定することで、罠砦のローカル座標を基準に配置
                GameObject uiInstance = Instantiate(uiPrefab, transform,false);

                // 砦の大きさに影響されないようにUIのスケールをリセット
                uiInstance.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
                uiInstance.transform.localPosition = uiOffset;

                // UIコンポーネントを取得して初期化
                fortressUI = uiInstance.GetComponent<DecoyFortressUI>();
                if(fortressUI != null)
                {
                    fortressUI.SetUp(FortressMaxHP, fortressHP);

                    // 現在の有効フラグに合わせて表示・非表示を切り替える
                    fortressUI.SetActive(fortressEnabled);
                }
            }
        }

        /// <summary>
        /// 罠砦がダメージを受けたときの処理
        /// (Enemyクラスなどから呼ばれる)
        /// 
        /// </summary>
        /// <param name="damageAmount">受けたダメージ量</param>
        public void OnDamaged(float damageAmount)
        {
            if (fortressHP - damageAmount <= 0)
            {
                fortressHP = 0;
                fortressEnabled = false;
                Debug.Log("罠砦が壊れた");

                // UIの更新と非表示化
                if (fortressUI != null)
                {
                    fortressUI.UpdateHP(fortressHP);
                    fortressUI.SetActive(false); // 壊れたら非表示
                }
            }
            else
            {
                fortressHP -= damageAmount;
                Debug.Log("現在の罠砦のHP:" + fortressHP);

                // UIの数値を更新
                if (fortressUI != null)
                {
                    fortressUI.UpdateHP(fortressHP);
                }
            }
        }
        
        /// <summary>
        /// 罠砦の建築/復活処理
        /// 
        ///     アイテムがあるか、罠砦に触れているかの判定は
        ///     プレイヤ側で行う
        /// </summary>
        public void Build()
        {
            fortressHP = FortressMaxHP;
            fortressUI.UpdateHP(fortressHP);
            SetEnable(true);
        }


        public void SetEnable(bool enable)
        {
            fortressEnabled = enable;

            // フラグが変わったタイミングでUIの表示・非表示を連動
            if(fortressUI != null)
            {
                fortressUI.SetActive(fortressEnabled);
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
            return fortressEnabled;
        }

        /// <summary>
        /// 罠砦の種類を返す
        /// </summary>
        /// <returns>罠砦ID</returns>
        public DecoyFortressIDs GetID()
        {
            return this.fotressID;
        }
    }
}