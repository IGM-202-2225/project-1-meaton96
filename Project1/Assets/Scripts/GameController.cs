
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class GameController : MonoBehaviour {
    public List<GameObject> enemies = new();                        //list to store all alive enemies in
    [SerializeField] private GameObject enemyPreFab;                //enemy prefab
    [SerializeField] private float defaultEnemyXOffset;             //how far apart to draw the enemies when creating a standard line pattern
    [SerializeField] private GameObject shopCanvas;                 //pointer to shop game object
    [SerializeField] private GameObject pauseCanvas;
    private Vector2 playerSpawn;                                    //location to respawn the player
    public GameObject player;                                       //pointer to player
    private PlayerBehaviour playerScript;                           //pointer to player script
                                                                    // private int enemyType = 0;                                      
    public int levelNumber;                                        //stores which number level player is on
    private bool paused = false;                                     //bool if game is paused or not
    private const float ROW_SEP_Y = 2;
    private const float ENEMY_SCALE = .8f;
    private const float ENEMY_BASE_SPEED = 4;
    private const int NUM_ENEMIES_PER_ROW = 12;
    private const int ENEMY_Y_OFFSET = 5;
    private readonly List<float> ENEMY_SPAWN_WEIGHTS = new(new float[] { 1f, 0f, 0f, 0f, 0f });
    private readonly List<float> SPAWN_CHANGE_PER_LEVEL = new(new float[] { -.1f, .05f, .03f, .015f, .05f });
    private float BOTTOM_OF_SCREEN;
    // Start is called before the first frame update
    void Start() {
        BOTTOM_OF_SCREEN = -Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        //initialize shop variables
        shopCanvas.GetComponent<ShopBehaviour>().Init();
        levelNumber = 0;
        playerScript = player.GetComponent<PlayerBehaviour>();
        playerSpawn = new Vector2(0f, -3f);
        StartNewLevel();
    }
    //spawns a simple enemy wave with
    //@param enemyType type of enemy
    //@numEnemies number of total enemies to spawn
    //@numRowsAtOnce the vertical number of rows to spawn
    //uses numEnemies and numRowsAtOnce to calculate number of enemies per row
    void SpawnDefaultEnemyWave(int enemyType, int numEnemies, int numRowsAtOnce) {

        int numEnemiesPerRow = numEnemies / numRowsAtOnce;

        //x spawn location set to negative one half of amount of enemies to spawn 
        float spawnX = numEnemiesPerRow * defaultEnemyXOffset / -2;

        Vector3 spawnPoint = new(
            spawnX,
            Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + numRowsAtOnce - ROW_SEP_Y,
            0f);

        //spawns all enemy rows alternating rows passing in true or false to say which direction the enemy is moving
        for (int x = 0; x < numEnemiesPerRow; x++) {
            for (int y = 0; y < numRowsAtOnce; y++) {

                Vector3 spawn = new(spawnPoint.x + x * defaultEnemyXOffset, spawnPoint.y - y * 2, 0f);
                if (y % 2 == 0)
                    SpawnEnemy(enemyType, spawn, false, ENEMY_SCALE);
                else
                    SpawnEnemy(enemyType, spawn, true, ENEMY_SCALE);
            }
        }
    }

    /// <summary>
    /// spawns a single enemy
    /// </summary>
    /// <param name="enemyType"> int enemy type</param>
    /// <param name="pos">vector position to instantitate the enemy prefab</param>
    /// <param name="right">bool if the enemy is moving right or left</param>
    /// <param name="scale">float to scale the enemy model to</param>
    private void SpawnEnemy(int enemyType, Vector3 pos, bool right, float scale) {
        GameObject enemy = Instantiate(enemyPreFab, pos, Quaternion.identity);
        enemy.GetComponent<EnemyBehaviour>().Init(enemyType, ENEMY_BASE_SPEED, right, scale);
        enemies.Add(enemy);
    }

    // Update is called once per frame
    void Update() {
        //checks for player pause by pressing escape key
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (shopCanvas.activeInHierarchy) {
                shopCanvas.GetComponent<ShopBehaviour>().Exit();
            }
            else {
                if (paused) {
                    Resume();
                }
                else {
                    Pause();
                }
            }
        }
        //checks for player alive, player spawning, and out of enemies conditions
        if (!playerScript.IsAlive() && !playerScript.isSpawning) {
            if (playerScript.OutOfLives()) {
                Time.timeScale = 0f;
                //end game screen
            }
            //reset player position and start player spawn routine
            player.transform.position = playerSpawn;
            StartCoroutine(playerScript.Respawn());
        }
        if (!enemies.Any()) {
            StartNewLevel();
        }
        
    }
    void CheckForEnemyOffScreen() {
        Debug.Log(BOTTOM_OF_SCREEN);
        enemies.ForEach(enemy => {
            if (enemy.transform.position.y < BOTTOM_OF_SCREEN) {
                enemies.Remove(enemy);
                return;
            }
        });
    }

    public void Pause() {
        pauseCanvas.SetActive(true);
        paused = true;
        Time.timeScale = 0f;
    }
    public void Resume() {

        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        paused = false;

    }


    //starts a new level
    //exits the shop, resets timescale, increases level number
    public void StartNewLevel() {
        //paused = false;
        //shopCanvas.SetActive(false);
        //Time.timeScale = 1f;
        playerScript.coins += levelNumber;
        levelNumber++;
        //SpawnDefaultEnemyWave(1, 1, 1);
        SpawnEnemyWave(5 + levelNumber * 5);
        for (int x = 0; x < ENEMY_SPAWN_WEIGHTS.Count; x++) {
            ENEMY_SPAWN_WEIGHTS[x] = ENEMY_SPAWN_WEIGHTS[x] + SPAWN_CHANGE_PER_LEVEL[x];
        }
    }
    public void SpawnEnemyWave(int numEnemies) {

        float spawnX = NUM_ENEMIES_PER_ROW * defaultEnemyXOffset / -2;

        int row = 0;
        int count = 0;
        bool right = true;
        while (count < numEnemies) {


            for (int y = 0; y < NUM_ENEMIES_PER_ROW && count < numEnemies; y++) {
                float spawn = Random.Range(0, 1f);
                //Debug.Log(spawn);
                int index = 0;
                float spawnSum = ENEMY_SPAWN_WEIGHTS[0];

                while (spawn > spawnSum) {
                    if (ENEMY_SPAWN_WEIGHTS[index] >= 1) {
                        SPAWN_CHANGE_PER_LEVEL[index] = -.1f;
                    }
                    spawnSum += ENEMY_SPAWN_WEIGHTS[index];
                    index++;

                }

                //index out of bounds at wave 10
                SpawnEnemy(index, new Vector3(
                    spawnX + y * defaultEnemyXOffset,
                    Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + row * ROW_SEP_Y + 1, 0),
                    right,
                    ENEMY_SCALE);
                count++;
            }
            row++;
            right = !right;
        }

    }

}
