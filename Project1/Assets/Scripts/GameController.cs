using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class GameController : MonoBehaviour {
    public List<GameObject> enemies = new();
    [SerializeField] private GameObject enemyPreFab;
    [SerializeField] private float defaultEnemyXOffset;
    [SerializeField] private GameObject shopCanvas;
    private Vector2 playerSpawn;
    public GameObject player;
    private PlayerBehaviour playerScript;
    private int enemyType = 0;
    private int levelNumber;
    private bool paused = true;
    // Start is called before the first frame update
    void Start() {
        
        shopCanvas.GetComponent<ShopBehaviour>().Init();
        levelNumber = 0;
        playerScript = player.GetComponent<PlayerBehaviour>();
        playerSpawn = new Vector2(0f, -3f);
        StartNewLevel();
    }
    void SpawnDefaultEnemyWave(int enemyType, int numEnemies, int numRowsAtOnce) {

        int numEnemiesPerRow = numEnemies / numRowsAtOnce;

        float spawnX = numEnemiesPerRow * defaultEnemyXOffset / -2;
        Vector3 spawnPoint = new(
            spawnX,
            Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + numRowsAtOnce - 2,
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
        enemy.GetComponent<EnemyBehaviour>().Init(enemyType, 4, right, scale, 1);
        enemies.Add(enemy);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!paused) {
                StartShop(false);
                paused = true;
            }
            else {
                Resume();
            }


        }
        if (!playerScript.IsAlive() && !playerScript.isSpawning) {
            if (playerScript.OutOfLives()) {
                Time.timeScale = 0f;
                //end game screen
            }


            player.transform.position = playerSpawn;
            StartCoroutine(playerScript.Respawn());

        }
        if (!enemies.Any()) {
            StartShop(true);
        }
    }
    public void Resume() {
        if (enemies.Count == 0) {

            StartNewLevel();
        }
        else {
            Time.timeScale = 1f;
            shopCanvas.SetActive(false);
            paused = false;
        }
    }


    public void StartShop(bool betweenLevels) {
        shopCanvas.GetComponent<ShopBehaviour>().canPurchase = betweenLevels;
        Time.timeScale = 0f;
        shopCanvas.SetActive(true);
        shopCanvas.GetComponent<ShopBehaviour>().OnShopShow();
    }
    public void StartNewLevel() {
        paused = false;
        shopCanvas.SetActive(false);
        Time.timeScale = 1f;
        levelNumber++;
        SpawnDefaultEnemyWave(enemyType++, 10, 2);
        //todo
    }
}
