
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public List<GameObject> enemies = new();                        //list to store all alive enemies in
    [SerializeField] private GameObject enemyPreFab;                //enemy prefab
    private float defaultEnemyXOffset;             //how far apart to draw the enemies when creating a standard line pattern
    [SerializeField] private GameObject shopCanvas;                 //pointer to shop game object
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private UIBehaviour uiScript;
    [SerializeField] private GameObject bossPreFab;
    private Vector2 playerSpawn;                                    //location to respawn the player
    public GameObject player;                                       //pointer to player
    private PlayerBehaviour playerScript;                           //pointer to player script
                                                                    // private int enemyType = 0;                                      
    public int levelNumber;                                        //stores which number level player is on
    public bool isPaused = false;                                     //bool if game is paused or not
    private const float ROW_SEP_Y = 2f;
    private const float ENEMY_SCALE = .8f;
    private const float ENEMY_BASE_SPEED = 4f;
    private const int NUM_ENEMIES_PER_ROW = 12;
    private float enemySpawnY;
    private float enemySpawnX;
    private readonly List<float> ENEMY_SPAWN_WEIGHTS = new(new float[] { .8f, .2f, 0f, 0f, 0f });                             //used to seed enemy spawns, every wave these are added together and used to 
    private readonly List<float> SPAWN_CHANGE_PER_LEVEL = new(new float[] { -.2f, .1f, .06f, .03f, .01f });               //spawn different enemy tpes, so the longer game goes on, the harder enemies spawn more often
    private float BOTTOM_OF_SCREEN;
    // Start is called before the first frame update
    void Start() {
        uiScript = GameObject.FindWithTag("ui").GetComponent<UIBehaviour>();
        BOTTOM_OF_SCREEN = -Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        //initialize shop variables
        shopCanvas.GetComponent<ShopBehaviour>().Init();
        levelNumber = 0;
        playerScript = player.GetComponent<PlayerBehaviour>();
        playerSpawn = new Vector2(0f, -3f);
        enemySpawnY = BOTTOM_OF_SCREEN - 1;
        enemySpawnX = -Camera.main.ScreenToWorldPoint(Vector3.zero).x + 2;
        Debug.Log(enemySpawnX);
        defaultEnemyXOffset = 1f;
        StartNewLevel();
        
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
        foreach (GameObject enemy in enemies) {
            if (!enemy.GetComponent<EnemyBehaviour>().isAlive) {
                enemies.Remove(enemy);
                break;
            }
        }
        //checks for player pause by pressing escape key
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (shopCanvas.activeInHierarchy) {
                shopCanvas.GetComponent<ShopBehaviour>().Exit();
            }
            else if (infoCanvas.activeInHierarchy) {
                infoCanvas.GetComponent<InfoMenuBehaviour>().Back();
            }
            else {
                if (isPaused) {
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
                //game over
                Time.timeScale = 0f;
                player.SetActive(false);

                SceneManager.LoadScene(2, LoadSceneMode.Single);
                //SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByBuildIndex(2));

            }
            //reset player position and start player spawn routine
            player.transform.position = playerSpawn;

        }
        if (!enemies.Any()) {
            StartNewLevel();

        }

    }
    //spawn the boss and toggle boss health bar on UI
    public void SpawnBoss() {
        enemies.Add(Instantiate(bossPreFab, new Vector3(0f, 8.5f, 0f), Quaternion.identity));
        uiScript.ToggleBossHealthBar();

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
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void Resume() {

        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        isPaused = false;

    }


    //starts a new level
    //exits the shop, resets timescale, increases level number
    public void StartNewLevel() {
        if (levelNumber == 10) {
            GameObject.FindWithTag("data").GetComponent<DataTransferBehaviour>().wonGame = true;
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
        playerScript.coins += levelNumber;
        levelNumber++;

        if (levelNumber == 10) {
            SpawnBoss();
        }
        else {
            SpawnEnemyWave(5 + levelNumber * 5);
            //increase the spawn weights on harder enemies, decrease on easier ones
            for (int x = 0; x < ENEMY_SPAWN_WEIGHTS.Count; x++) {
                ENEMY_SPAWN_WEIGHTS[x] = ENEMY_SPAWN_WEIGHTS[x] + SPAWN_CHANGE_PER_LEVEL[x];
            }
        }
    }
    public void SpawnEnemyWave(int numEnemies) {

        

       /* int row = 0;
        int count = 0;
        bool right = true;
        while (count < numEnemies) {


            for (int y = 0; y < NUM_ENEMIES_PER_ROW && count < numEnemies; y++) {
                float spawn = Random.Range(0, 1f);
                
                int index = 0;
                float spawnSum = ENEMY_SPAWN_WEIGHTS[0];

                for (int x = 1; x < ENEMY_SPAWN_WEIGHTS.Count; x++) {
                    if (ENEMY_SPAWN_WEIGHTS[x] >= 1) {
                        SPAWN_CHANGE_PER_LEVEL[x] = -.1f;
                    }
                    if (spawn < spawnSum) {
                        index = x - 1;
                        break;
                    }
                    else {
                        spawnSum += ENEMY_SPAWN_WEIGHTS[x];
                    }
                }

                SpawnEnemy(index, new Vector3(
                    spawnX + y * defaultEnemyXOffset,
                    Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + row * ROW_SEP_Y + 1,
                    0f),
                    right,
                    ENEMY_SCALE);
                count++;
            }
            row++;
            right = !right;
        }*/
        for (int x = 0; x < numEnemies; x++) {
            float spawn = Random.Range(0, 1f);

            int index = 0;
            float spawnSum = ENEMY_SPAWN_WEIGHTS[0];

            for (int y = 1; y < ENEMY_SPAWN_WEIGHTS.Count; y++) {
                if (ENEMY_SPAWN_WEIGHTS[y] >= 1) {
                    SPAWN_CHANGE_PER_LEVEL[y] = -.1f;
                }
                if (spawn < spawnSum) {
                    index = y - 1;
                    break;
                }
                else {
                    spawnSum += ENEMY_SPAWN_WEIGHTS[y];
                }
            }
            Vector3 spawnPos;
            if (x % 2 == 0) {
                spawnPos = new Vector3(enemySpawnX + defaultEnemyXOffset * x, enemySpawnY, 0f);
            }
            else
                spawnPos = new Vector3(-enemySpawnX - defaultEnemyXOffset * x - 1, enemySpawnY - ROW_SEP_Y, 0f);

            Debug.Log(spawnPos.ToSafeString());
            SpawnEnemy(index, spawnPos,
                    x % 2 != 0,
                    ENEMY_SCALE);
        }
    }
    

}
