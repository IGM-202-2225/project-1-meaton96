using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameController : MonoBehaviour {
    public List<GameObject> enemies = new();
    [SerializeField] private GameObject enemyPreFab;
    [SerializeField] private float defaultEnemyXOffset;
    private int numEnemiesSpawned = 0;
    // Start is called before the first frame update
    void Start() {
        SpawnDefaultEnemyWave(0, 50, 5);
    }
    void SpawnDefaultEnemyWave(int enemyType, int numEnemies, int numRowsAtOnce) {

        int numEnemiesPerRow = numEnemies / numRowsAtOnce;

        float spawnX = (numEnemiesPerRow * defaultEnemyXOffset) / -2;
        Vector3 spawnPoint = new(
            spawnX,
            Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) - 1,
            0f);
        

        for (int x = 0; x < numEnemiesPerRow; x++) {
            for (int y = 0; y < numRowsAtOnce; y++) {
                Vector3 spawn = new(spawnPoint.x + x * defaultEnemyXOffset, spawnPoint.y - y * 2, 0f);
                if (y % 2 == 0)
                    SpawnEnemy(enemyType, spawn, false, 0.8f);
                else
                    SpawnEnemy(enemyType, spawn, true, 0.8f);
            }
        }
    }

    private void SpawnEnemy(int enemyType, Vector3 pos, bool right, float scale) {
        GameObject enemy = Instantiate(enemyPreFab, pos, Quaternion.identity);
        enemy.GetComponent<EnemyBehaviour>().Init(enemyType, 4, right, scale);
        enemies.Add(enemy);
        ++numEnemiesSpawned;
    }

    // Update is called once per frame
    void Update() {

    }
}
