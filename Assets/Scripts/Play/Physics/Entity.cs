using UnityEngine;

namespace Physics
{
    /// <summary>
    /// 物理空間（PhysicsManager）に存在できるオブジェクトを定義
    /// </summary>
    public class Entity
    {
        private Shape shape;

        private Vector2 position;

        private Vector2 velocity;

        private Vector2 scale;

        private float friction = 1F;

        public Entity(Shape shape) : this(shape, new(0F, 0F), new(0F, 0F), new(1F, 1F)) { }

        public Entity(Shape shape, Vector2 position, Vector2 velocity, Vector2 scale)
        {
            this.Shape = shape;
            this.Position = position;
            this.Velocity = velocity;
            this.Scale = scale;
        }

        /// <summary>
        /// <para>エンティティ同士が衝突しているかどうか</para>
        /// <para>衝突しない場合は正の無限大を返す</para>
        /// </summary>
        public static float Intersects(Entity a, Entity b, float duration)
        {
            int precision = Mathf.Clamp
            (
                Mathf.CeilToInt
                (
                    Mathf.Max
                    (
                        Mathf.Abs(a.Velocity.x * duration),
                        Mathf.Abs(a.Velocity.y * duration),
                        Mathf.Abs(b.Velocity.x * duration),
                        Mathf.Abs(b.Velocity.y * duration)
                    )
                ),
                1,
                100
            );

            float step = duration / precision;
            float time;

            for (int i = 0; i < precision; ++i)
            {
                time = i * step;

                if
                (
                    Shape.Intersects
                    (
                        a.Shape,
                        a.Position + a.Velocity * time,
                        a.Scale,
                        b.Shape,
                        b.Position + b.Velocity * time,
                        b.Scale
                    )
                )
                {
                    return i * step;
                }
            }

            return float.PositiveInfinity;
        }

        public Shape Shape
        {
            get { return Shape.SQUARE; }

            set { this.shape = value; }
        }

        public Vector2 Position
        {
            get { return this.position; }

            set { this.position = value; }
        }

        public Vector2 Velocity
        {
            get { return this.velocity; }

            set { this.velocity = value; }
        }

        public Vector2 Scale
        {
            get { return this.scale; }

            set { this.scale = value; }
        }

        public float Friction
        {
            get { return this.friction; }

            set { this.friction = value; }
        }
    }
}
