using Unity.VisualScripting;
using UnityEngine;

namespace Physics
{
    /// <summary>
    /// このクラスをGameObjectにアタッチするとPhysicsManagerに紐づけられる
    /// </summary>
    public class EntityBehaviour : MonoBehaviour
    {
        private readonly Entity entity = new(Shape.SQUARE);

        /// <summary>
        /// GameObjectの位置や速度を保持
        /// </summary>
        public Entity Entity
        {
            get { return this.entity; }
        }

        public void Start()
        {
            // このEntityBehaviourとEntityを紐づける
            PhysicsManager.INSTANCE.Register(this, this.entity);
        }

        public void Update()
        {
            // GameObjectの座標を更新する
            this.transform.position = this.entity.Position;
        }

        public void OnDestroy()
        {
            // このEntityBehaviourとEntityの紐づけを解除
            PhysicsManager.INSTANCE.Remove(this);
        }
    }
}
