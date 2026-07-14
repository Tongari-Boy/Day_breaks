using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Physics
{
    /// <summary>
    /// <para>GameObject同士の衝突を検知するための物理エンジン</para>
    /// <para>インスタンスを取得するにはPhysicsManager.INSTANCEを参照する</para>
    /// </summary>
    public class PhysicsManager
    {
        public static readonly PhysicsManager INSTANCE = new();

        private readonly Dictionary<GameObject, Entity> entities = new();

        private PhysicsManager() { }

        /// <summary>
        /// GameObjectを物理空間に追加
        /// </summary>
        public void Register(GameObject gameObject, Entity entity)
        {
            this.entities.Add(gameObject, entity);
        }

        /// <summary>
        /// GameObjectを物理空間から削除
        /// </summary>
        public void Remove(GameObject gameObject)
        {
            this.entities.Remove(gameObject);
        }

        /// <summary>
        /// PhysicsManagerの初期化
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// PhysicsManagerの更新
        /// </summary>
        public void OnUpdate()
        {
            this.Collide();
            this.Update();
        }

        /// <summary>
        /// 衝突を検知
        /// </summary>
        private void Collide()
        {
        }

        /// <summary>
        /// Entityの情報を更新
        /// </summary>
        private void Update()
        {
            foreach (Entity entity in this.entities.Values)
            {
                entity.Position += Time.deltaTime * entity.Velocity;
                entity.Velocity -= Time.deltaTime * entity.Friction * entity.Velocity;
            }
        }
    }
}
