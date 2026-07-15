using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 経過時間に応じて敵をスポーンさせるクラス
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("スポーン情報のリスト")]
        [SerializeField] private List<EnemySpawnData> spawnDataList;

        [SerializeField] private EnemyManager enemyManager;

        private float elapseTime;

        // まだスポーンしていないものだけを管理
        //  スポーン済なら除外する
        private List<EnemySpawnData> pendingSpawns;

        public void Initialize()
        {
            elapseTime = 0f;
            pendingSpawns = new List<EnemySpawnData>(spawnDataList);
            pendingSpawns.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
        }

        public void OnUpdate()
        {
            elapseTime += Time.deltaTime;

            // 経過時間を超えたものを順番にスポーンさせる
            while (pendingSpawns.Count > 0 && pendingSpawns[0].SpawnTime <= elapseTime)
            {
                Spawn(pendingSpawns[0]);
                pendingSpawns.RemoveAt(0);
            }
        }


        private void Spawn(EnemySpawnData data)
        {
            var enemy = Instantiate(data.EnemyPrefab, data.SpawnPosition, Quaternion.identity);
            enemyManager.RegisterEnemy(enemy);
        }
    }
}