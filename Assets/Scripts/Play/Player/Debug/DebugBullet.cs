using UnityEngine;

namespace DebugBullet
{
    public class DebugBulletBehaviour : MonoBehaviour
    {
        public void Initialize()
        {
        }

        public void OnUpdate()
        {
        }

        void Start()
        {
            // デバッグ用
            Initialize();
        }

        void Update()
        {
            // デバッグ用
            OnUpdate();
        }
    }
}
