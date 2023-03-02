using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {
    private float movementSpeed;                                                        //how fast the player can fly around
    [SerializeField] private GameObject bulletPrefab;                                   //pointer to bullet pre fab 
    [SerializeField] private Sprite[] shipSprites = new Sprite[3];                      //holds sprites for each ship type NYI
    private Animator animator;
    public int shipType;                                                                //NYI
    private SpriteRenderer sr;                                                          //pointer to sprite renderer component
    private float maxX, maxY, minY;                                                           //floats to hold screen dimensions to keep player on screen
    [SerializeField] private float bulletSpawnOffset;                                   //how far above the player to spawn the bullet
    [SerializeField] private float attackDelay;                                         //in seconds how long between player can fire bullets
    private float shootCount = 0;                                                       //counter for time passage for shooting
    public float firstRadius = 1f, secondRadius = 0.4f;                                 //first and second sizes for hitbox circles
    public float firstRadiusYOffset = .5f;                                              //offsets for drawing hitboxes
    public float secondRadiusXOffset = .1f;
    public float thirdRadiusYOffset = -0.4f, thirdRadiusXOffset = 0.25f;
    public float currentHealth;                                                           //current health points
    public float maxHealth;                                                               //maximum health points
    public int score;                                                                   //player score
    private int lives;                                                                  //player lives
    public int coins;                                                                   //player money
    public bool isSpawning = false;                                                     //if the player is spawning (player is immune)
    public int numBulletsFired;                                                         //number of bullets fired per shot
    public float bulletSpreadAngle = 10f;                                               //angle between bullets when firing more than 1
    public int damageDone = 1;                                                          //the damage of each bullet
    public int armor;                                                                   //player armor
    public int numTargetsPierced = 0;                                                   //number of enemies bullets pierce through
    private const int HEALTH_PER_UPGRADE = 25;                                          //how much health the player gets per upgrade level
    private const int BASE_HEALTH = 100;                                                //how much health player starts with
    private const float BASE_SPEED = 8;                                                 //how fast player moves before any upgrades
    private const float SPEED_PER_UPGRADE = .5f;                                        //how much faster player gets per upgrade level
    private const float ATTACK_SPEED_PER_UPGRADE = -0.05f;                               //how much less time needs to pass before player can shoot again
    private const float BASE_ATTACK_DELAY = .5f;                                        //base time in seconds between player attacks
    private const float ARMOR_STRENGTH = 1.5f;                                           //how much damage each point of armor reduces
    public int[] upgradeLevels;                                                         //stores how many of each upgrade type player has purchased
    public float AttackSpeed { get { return ((int)(10 / attackDelay)) / 10f; } }       //float for displaying player attack speed on interface

    private enum State { Normal, Roll, Respawning }
    private State state;
    private float rollTimer = .2f;
    private float rollCounter;

    

    // Start is called before the first frame update
    void Start() {
        state = State.Normal;
        //set all variables to their base level
        upgradeLevels = new int[6];
        attackDelay = BASE_ATTACK_DELAY;

        armor = 0;
        movementSpeed = BASE_SPEED;
        numBulletsFired = 1;
        shipType = 0;   //only working ship type, others will have bad hitboxes
        lives = 3;
        coins = 100;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) - 1;
        minY = -maxY + 2;

        currentHealth = maxHealth = BASE_HEALTH;

        switch (shipType) {
            case 0:
                sr.sprite = shipSprites[0];
                break;
            case 1:
                sr.sprite = shipSprites[1];
                break;
            case 2:
                sr.sprite = shipSprites[2];
                break;
        }
    }

    //increases player score by n
    public void AddToScore(int n) {
        score += n;
    }
    // Update is called once per frame
    void Update() {

        //call player movement method and checks for player shooting

        HandlePlayerMovement();
        if (Input.GetKey(KeyCode.Space) && shootCount > attackDelay) {
            Shoot();
            shootCount = 0;
        }
        else
            shootCount += Time.deltaTime;

        if (state == State.Roll)
            HandleRoll();

    }
    //take damage by passed in value (reduced by armor)
    public void TakeDamage(float damage, bool isBullet) {
        currentHealth -= damage - (isBullet ? armor * ARMOR_STRENGTH : 0);
        if (currentHealth < 0) {
            currentHealth = 0;
        }
    }
    //check for collisions with enemy bullet, check outer radius circle before calling inner circle collision
    public bool CheckCollision(GameObject bullet, float radius, float offsetX, float offSetY) {
        if (state == State.Respawning || state == State.Roll)
            return false;

        //magnitute^2 < x^2 + y^2
        Vector3 bulletPos = bullet.transform.position;
        if (Mathf.Pow(transform.position.x - bulletPos.x + offsetX, 2) +
            Mathf.Pow(transform.position.y - bulletPos.y + offSetY, 2) <=
            Mathf.Pow(firstRadius + radius, 2)) {
            return CheckSecondCollision(bullet, radius, offsetX, offSetY);
        }
        return false;
    }
    //checks each of the 5 inner hitbox circles against bullet hitbox to check for actual
    //ship collisions
    private bool CheckSecondCollision(GameObject bullet, float radius, float offsetX, float offSetY) {
        Vector3 bulletPos = new(bullet.transform.position.x + offsetX,
            bullet.transform.position.y + offSetY,
            0);

        Vector3[] hitBoxCenters = new Vector3[5];
        hitBoxCenters[0] = new Vector3(transform.position.x,
            transform.position.y + firstRadiusYOffset, 0);
        hitBoxCenters[1] = new Vector3(transform.position.x + secondRadiusXOffset, transform.position.y, 0);
        hitBoxCenters[2] = new Vector3(transform.position.x - secondRadiusXOffset, transform.position.y, 0);
        hitBoxCenters[3] = new Vector3(transform.position.x + thirdRadiusXOffset, transform.position.y + thirdRadiusYOffset, 0);
        hitBoxCenters[4] = new Vector3(transform.position.x - thirdRadiusXOffset, transform.position.y + thirdRadiusYOffset, 0);

        for (int x = 0; x < hitBoxCenters.Length; x++) {
            if (Mathf.Pow(hitBoxCenters[x].x - bulletPos.x, 2) +
                Mathf.Pow(hitBoxCenters[x].y - bulletPos.y, 2) <=
                Mathf.Pow(radius + secondRadius, 2))
                return true;
        }

        return false;
    }
    //creates bullet objects for each bullet shot
    //sets the angle and inits the bullet with the angle and damage done
    void Shoot() {

        float angle;

        //changes how bullets are spawned based on how many are being fired
        for (int x = 0; x < numBulletsFired; x++) {
            if (numBulletsFired % 2 != 0) {
                if (x == 0)
                    angle = 0;
                else if (x % 2 != 0) {
                    angle = bulletSpreadAngle * (x + 1) / 2;
                }
                else
                    angle = -bulletSpreadAngle * x / 2;
            }
            else {
                if (x % 2 == 0) {
                    angle = -bulletSpreadAngle * (x + 1) / 2;
                }
                else
                    angle = bulletSpreadAngle * x / 2;
            }
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, -angle)));
            bullet.GetComponent<BulletBehaviour>().Init(damageDone, ToRadians(angle), numTargetsPierced);
        }
    }
    //helper function since Quaternion Euler method takes degrees and movement (Mathf trig functions) takes radians??
    private float ToRadians(float angle) {
        return angle / 180 * Mathf.PI;
    }

    //respawns the player
    public IEnumerator Respawn() {
        state = State.Respawning;
        //isSpawning = true;                              //set spawning flag
        for (int x = 0; x < 12; x++) {
            Color color = sr.color;
            color.a = color.a == 0 ? 255f : 0;
            sr.color = color;
            yield return new WaitForSeconds(0.25f);     //alternates every .25seconds between 0 and 255 alpha for 3 seconds
        }
        currentHealth = maxHealth;  //reset health
                                    //isSpawning = false;         //clear flag
        state = State.Normal;
        DecrementLives();           //lose a life
        yield return null;          //exit coroutine
    }

    //check for each player movement, using velocity vector * time to increase position vector
    void HandlePlayerMovement() {

        
        if (Input.GetKey(KeyCode.A) && sr.bounds.min.x >= -maxX) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.left;
        }
        if (Input.GetKey(KeyCode.S) && sr.bounds.min.y >= minY) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.down;
        }
        if (Input.GetKey(KeyCode.D) && sr.bounds.max.x <= maxX) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && sr.bounds.max.y <= maxY) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            state = State.Roll;
            movementSpeed *= 2f;
            animator.SetTrigger("Roll");
        }

    }
    void HandleRoll() {
        //Vector3 dir = (previousPos - transform.position).normalized;
        //transform.position += movementSpeed * Time.deltaTime * dir;
        rollCounter += Time.deltaTime;
        if (rollCounter >= rollTimer) {
            state = State.Normal;
            movementSpeed /= 2f;
            rollCounter = 0;
        }
    }
    //returns a string of the players stats for displaying on the UI
    public string GetPlayerStat() {
        return "Health " + currentHealth + "/" + maxHealth + "\t" +
            "Armor " + armor + "\n" +
            "Bullets " + numBulletsFired + "\t\t" +
            "Atl Speed " + AttackSpeed + "\n" +
            "Pierce " + numTargetsPierced + "\t\t" +
            "Damage " + damageDone;
    }
    //upgrades one of the player's stats based on the upgrade ID
    //sets that state to be upgraded to level
    public void SetUpgradeLevel(int upgradeId, int level) {
        switch (upgradeId) {
            case 0:
                currentHealth = maxHealth = level * HEALTH_PER_UPGRADE + BASE_HEALTH;
                break;
            case 1:
                armor = level;
                break;
            case 2:
                movementSpeed = BASE_SPEED + level * SPEED_PER_UPGRADE;
                break;
            case 3:
                damageDone = level + 1;
                break;
            case 4:
                attackDelay = BASE_ATTACK_DELAY + level * ATTACK_SPEED_PER_UPGRADE;
                break;
            case 5:
                numBulletsFired = level + 1;
                break;
        }
        upgradeLevels[upgradeId] = level;
    }
    //getters and setters if needed

    public bool IsAlive() { return currentHealth > 0; }
    public void DecrementLives() { lives--; }
    public void IncrementLives() { lives++; }
    public void ResetLives() { lives = 3; }
    public bool OutOfLives() { return lives <= 0; }
    public void AddToCoins(int numCoins) {
        coins += numCoins;
    }
    public int Lives { get { return lives; } }

    //checks if the player can afford the item or not
    public bool CanPurchaseItem(int itemCost, int shoppingCartTotal) {
        return coins >= itemCost + shoppingCartTotal;
    }
}
