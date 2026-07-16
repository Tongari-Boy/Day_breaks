using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("ゲーム中に使うタイマー")]
    [SerializeField] private float timer = 180.0f;

    [Header("ゲームの最大時間(3分)")]
    [SerializeField] private float MaxTime = 180.0f;

    [Header("タイマーが動いているか")]
    [SerializeField] private bool isTimeRunning = false;

    [Header("UI用のテキスト")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("GameManagerをアタッチ")]
    [SerializeField] private Play.GameManager gameManager;


    public void Initialize()
    {
        // タイマーに最大時間を入れる
        timer = MaxTime;

        // デモ:初期化と同時にタイムスタート
        OnTimeStart();
    }

    /// <summary>
    /// 時間を計り始めるようにする処理
    /// 以降にOnUpdate()を書いていない限り時間が減ることはありません
    /// </summary>
    public void OnTimeStart()
    {
        isTimeRunning = true;
    }

    public void OnUpdate()
    {
        if(isTimeRunning)
        {
            if(timer > 0)
            {
                // 毎フレームの経過時間を引く
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                // 3分経った後
                timer = 0;
                isTimeRunning = false;
                OnTimerUp();
            }
        }
    }

    /// <summary>
    /// 時間を表示する処理
    /// フォーマットを整理するもの
    /// </summary>
    /// <param name="timer">現在の時刻</param>
    private void DisplayTime(float timer)
    {
        // マイナス表示を防ぐ
        if (timer < 0) timer = 0;

        // 分の秒を計算
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        // 00:00の形式でテキストを更新
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// タイムアップ時に呼び出す処理
    /// 明示的にテキストを00:00にする
    /// </summary>
    private void OnTimerUp()
    {
        timerText.text = "00:00";
        gameManager.GameClear();
    }
}