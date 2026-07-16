using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    /// <summary>
    /// 次のシーンへ遷移する
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}