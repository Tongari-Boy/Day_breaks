using System;
using UnityEngine;

namespace Physics
{
    /// <summary>
    /// Entityの形状を定義
    /// </summary>
    public class Shape
    {
        /// <summary>
        /// 空
        /// </summary>
        public static readonly Shape EMPTY = Shape.CreatePolygon(0);

        /// <summary>
        /// 正方形
        /// </summary>
        public static readonly Shape SQUARE = Shape.CreatePolygon(4);

        private readonly Vector2[] vertices;
        private readonly int[] indexes;

        public Shape(Vector2[] vertices, int[] indexes)
        {
            int vLen = vertices.Length;
            int iLen = indexes.Length;

            int idx;

            // 頂点の定義が正しいかチェック
            for (int i = 0; i < iLen; ++i)
            {
                idx = indexes[i];

                if (idx < 0 || idx >= vLen)
                {
                    throw new ArgumentException("不正な頂点の定義を検知しました");
                }
            }

            this.vertices = vertices;
            this.indexes = indexes;
        }

        /// <summary>
        /// 正n角形を生成
        /// </summary>
        public static Shape CreatePolygon(int n, float w = 1F, float h = 1F)
        {
            if (n < 1)
                return new(new Vector2[0], new int[0]);

            Vector2[] vertices = new Vector2[n];
            int[] indexes = new int[n * 2];
            float radian = 2F * Mathf.PI / n;

            for (int i = 0; i < n; ++i)
            {
                vertices[i] = new(Mathf.Cos(i * radian) * w, Mathf.Sin(i * radian) * h);
                indexes[i * 2 + 0] = (i + 0) % n;
                indexes[i * 2 + 1] = (i + 1) % n;
            }

            return new Shape(vertices, indexes);
        }

        /// <summary>
        /// ２つのShapeが交わるか
        /// </summary>
        public static bool Intersects(Shape shpA, in Vector2 posA, in Vector2 sclA, Shape shpB, in Vector2 posB, in Vector2 sclB)
        {
            Vector2[] aVtx = shpA.vertices;
            Vector2[] bVtx = shpB.vertices;
            int[] aIdx = shpA.indexes;
            int[] bIdx = shpB.indexes;
            int aLen = shpA.indexes.Length - 1;
            int bLen = shpB.indexes.Length - 1;

            for (int i = 0; i < aLen; i += 2)
            {
                for (int j = 0; j < bLen; j += 2)
                {
                    if (Shape.Intersects(
                        posA + aVtx[aIdx[i + 0]] * sclA,
                        posA + aVtx[aIdx[i + 1]] * sclA,
                        posB + bVtx[bIdx[j + 0]] * sclB,
                        posB + bVtx[bIdx[j + 1]] * sclB)
                    )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 線分AB、CDが交わっているか
        /// </summary>
        private static bool Intersects(in Vector2 a, in Vector2 b, in Vector2 c, in Vector2 d)
        {
            return Shape.Cross(c - a, b - a) * Shape.Cross(b - a, d - a) >= 0F && Shape.Cross(b - c, d - c) * Shape.Cross(d - c, a - c) >= 0F;
        }

        /// <summary>
        /// ベクトルA、Bの外積
        /// </summary>
        private static float Cross(in Vector2 a, in Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }
    }
}