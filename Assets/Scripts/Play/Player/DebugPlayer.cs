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
        PlayerItemRegistry.INSTANCE.Initialize();
        PlayerManager.INSTANCE.Initialize();
        PlayerManager.INSTANCE.SetPlayer(gameObject);
    }

    void Update()
    {
        PlayerManager.INSTANCE.OnUpdate();
    }
}
