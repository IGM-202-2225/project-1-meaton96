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
    private Vector2 playerSpawn;                                    //location to respawn the player
    public GameObject player;                                       //pointer to player
    private PlayerBehaviour playerScript;                           //pointer to player script
    private int enemyType = 0;                                      //stores what type of enemy to spawn
    private int levelNumber;                                        //stores which number level player is on
    private bool paused = true;                                     //bool if game is paused or not
    private const float ROW_SEP_Y = 2;
    private const float ENEMY_SCALE = .8f;
    private const float ENEMY_BASE_SPEED = 4;
    // Start is called before the first frame update
    void Start() {
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
            if (!paused) {
                StartShop(false);
                paused = true;
            }
            else {
                Resume();
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
            StartShop(true);
        }
    }
    //resumes the game from the puase screen
    public void Resume() {
        if (enemies.Count == 0) {
            //if enemies are 0 start a new level
            StartNewLevel();
        }
        else {
            //otherwise just resume the game
            Time.timeScale = 1f;
            shopCanvas.SetActive(false);
            paused = false;
        }
    }

    //Starts the shop
    //bool param to tell the shop if upgrades are purchasable or not
    public void StartShop(bool betweenLevels) {
        shopCanvas.GetComponent<ShopBehaviour>().canPurchase = betweenLevels;
        Time.timeScale = 0f;
        shopCanvas.SetActive(true);
        shopCanvas.GetComponent<ShopBehaviour>().OnShopShow();
    }
    //starts a new level
    //exits the shop, resets timescale, increases level number
    public void StartNewLevel() {
        paused = false;
        shopCanvas.SetActive(false);
        Time.timeScale = 1f;
        levelNumber++;
        SpawnDefaultEnemyWave(enemyType++, 1, 1);

        

        //todo
    }
    public void SpawnEnemyWave() {

    }
}
