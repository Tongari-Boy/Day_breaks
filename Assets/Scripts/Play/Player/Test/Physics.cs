using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    /// <summary>
    /// PhysicsEngineをアタッチする
    /// </summary>
    public class PhysicsBehaviour : MonoBehaviour
    {
        void Start()
        {
            PhysicsEngine.INSTANCE.Initialize();
        }

        void Update()
        {
            PhysicsEngine.INSTANCE.OnUpdate();
        }
    }

    /// <summary>
    /// <para>TestBulletをRigidbodyなしで動かすために作ったしょぼい物理エンジン</para>
    /// <para>Registerメソッドを使ってオブジェクトを物理空間に追加できる</para>
    /// <para>このクラスはシングルトンであるため、PhysicsEngine.INSTANCEを使用する</para>
    /// </summary>
    public class PhysicsEngine
    {
        /// <summary>
        /// PhysicsEngineのインスタンス
        /// </summary>
        public static readonly PhysicsEngine INSTANCE = new();
        
        private readonly Dictionary<object, Entity> entities = new();

        private PhysicsEngine() { }

        /// <summary>
        /// 物理空間にGameObjectを追加する
        /// </summary>
        /// <param name="gameObject">登録するGameObject</param>
        public void Register(GameObject gameObject, Vector2 velocity = new())
        {
            Entity entity = new GameObjectEntity(gameObject);

            entity.Init();
            entity.Position = gameObject.transform.position;
            entity.Velocity = velocity;

            entities.Add(gameObject, entity);
        }

        /// <summary>
        /// 物理空間からGameObjectを削除する
        /// </summary>
        /// <param name="gameObject">削除するGameObject</param>
        public void Remove(GameObject gameObject)
        {
            entities.Remove(gameObject);
        }

        /// <summary>
        /// 登録されたGameObjectの座標を設定する
        /// </summary>
        /// <param name="gameObject">設定するGameObject</param>
        /// <param name="position">設定する座標</param>
        public void SetPosition(GameObject gameObject, Vector3 position)
        {
            if (entities.ContainsKey(gameObject))
                entities[gameObject].Position = position;
        }

        /// <summary>
        /// 登録されたGameObjectの速度を設定する
        /// </summary>
        /// <param name="gameObject">設定するGameObject</param>
        /// <param name="velocity">設定する速度</param>
        public void SetVelocity(GameObject gameObject, Vector2 velocity)
        {
            if (entities.ContainsKey(gameObject))
                entities[gameObject].Velocity = velocity;
        }

        public void Initialize()
        {
            foreach (Entity entity in entities.Values)
            {
                entity.Init();
            }
        }

        /// <summary>
        /// Time.deltaTime秒だけ物理空間を更新する
        /// </summary>
        public void OnUpdate()
        {
            OnUpdate(Time.deltaTime);
        }

        /// <summary>
        /// 指定された秒数分だけ物理空間を更新する
        /// </summary>
        /// <param name="deltaTime">経過させる物理時間（秒）</param>
        public void OnUpdate(float deltaTime)
        {
            foreach (Entity entity in entities.Values)
            {
                entity.Update(deltaTime);
            }
        }

        /// <summary>
        /// これを実装したクラスは物理空間に存在できる
        /// </summary>
        private abstract class Entity
        {
            /// <summary>
            /// エンティティの座標
            /// </summary>
            public abstract Vector2 Position { get; set; }

            /// <summary>
            /// エンティティの速度
            /// </summary>
            public abstract Vector2 Velocity { get; set; }

            /// <summary>
            /// エンティティの初期化
            /// </summary>
            public void Init() { }

            /// <summary>
            /// エンティティの更新
            /// </summary>
            /// <param name="deltaTime">経過させる物理時間</param>
            public void Update(float deltaTime)
            {
                Position += Velocity * deltaTime;
            }
        }

        /// <summary>
        /// <para>任意のクラスをラップできるエンティティ</para>
        /// <para>ハッシュテーブルでは、このエンティティはラップされたインスタンスと同じものとみなされる</para>
        /// <para>※ラップされたインスタンスが存在するときのみに限る</para>
        /// </summary>
        private abstract class WrapperEntity<T> : Entity
        {
            protected readonly T instance;

            public WrapperEntity(T instance)
            {
                this.instance = instance;
            }
        }

        /// <summary>
        /// GameObjectをEntityでラップしたもの
        /// </summary>
        private class GameObjectEntity : WrapperEntity<GameObject>
        {
            /// <summary>
            /// エンティティの速度を保持
            /// </summary>
            private Vector2 velocity = Vector2.zero;

            public override Vector2 Position
            {
                get
                {
                    return instance ? instance.transform.position : default;
                }

                set
                {
                    if (instance)
                        instance.transform.position = value;
                }
            }

            public override Vector2 Velocity
            {
                get
                {
                    return velocity;
                }

                set
                {
                    velocity = value;
                }
            }

            public GameObjectEntity(GameObject gameObject) : base(gameObject) { }
        }
    }
}