using Player;
using Player.Item;
using UnityEngine;

/// <summary>
/// PlayerManagerのデバッグ用
/// </summary>
public class DebugPlayer : MonoBehaviour
{
    void Start()
    {
        PlayerManager.INSTANCE.Initialize();
        PlayerManager.INSTANCE.SetPlayer(this.gameObject);
    }

    void Update()
    {
        PlayerManager.INSTANCE.OnUpdate();
    }
}
