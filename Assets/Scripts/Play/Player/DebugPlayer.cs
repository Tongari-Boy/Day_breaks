using Player;
using UnityEngine;

/// <summary>
/// PlayerManagerのデバッグ用
/// </summary>
public class DebugPlayer : MonoBehaviour
{
    void Start()
    {
        PlayerManager.INSTANCE.Initialize();
        PlayerManager.INSTANCE.SetPlayer(gameObject);
    }

    void Update()
    {
        PlayerManager.INSTANCE.OnUpdate();
    }
}
