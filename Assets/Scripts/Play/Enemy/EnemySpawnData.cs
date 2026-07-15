using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵1体分のスポーン情報(いつ、どこに、何を出現させるか)
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        [Header("ゲーム開始から何秒後にスポーンするか")]
        [SerializeField] private float spawnTime;

        [Header("スポーンする位置")]
        [SerializeField] private Vector2 spawnPosition;

        [Header("スポーンする敵のプレハブ")]
        [SerializeField] private EnemyMovement enemyPrefab;

        public float SpawnTime => spawnTime;
        public Vector2 SpawnPosition => spawnPosition;
        public EnemyMovement EnemyPrefab => enemyPrefab;
    }
}