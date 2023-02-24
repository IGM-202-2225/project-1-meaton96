using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {
    private float movementSpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite[] shipSprites = new Sprite[3];
    public int shipType;
    private SpriteRenderer sr;
    private float maxX, maxY;
    [SerializeField] private float bulletSpawnOffset;
    [SerializeField] private float attackDelay;
    private float shootCount = 0;
    public float firstRadius = 1f, secondRadius = 0.4f;
    public float firstRadiusYOffset = .5f;
    public float secondRadiusXOffset = .1f;
    public float thirdRadiusYOffset = -0.4f, thirdRadiusXOffset = 0.25f;
    public int currentHealth;   
    public int maxHealth;
    public int score;
    private int lives;
    public int coins;
    public bool isSpawning = false;
    public int numBulletsFired;
    public float bulletSpreadAngle = 10f;
    public int damageDone = 1;
    public int armor;
    public int numTargetsPierced = 0;
    private const int HEALTH_PER_UPGRADE = 10;
    private const int BASE_HEALTH = 10;
    private const float BASE_SPEED = 8;
    private const float SPEED_PER_UPGRADE = .5f;
    private const float ATTACK_SPEED_PER_UPGRADE = -0.5f;
    private const float BASE_ATTACK_DELAY = .5f;
    public int[] upgradeLevels;
    public float AttackSpeed { get { return 1 / attackDelay; } }

    // Start is called before the first frame update
    void Start() {
        upgradeLevels = new int[6];
        attackDelay = BASE_ATTACK_DELAY;
        armor = 0;
        movementSpeed = BASE_SPEED;
        numBulletsFired = 1; 
        shipType = 0;
        lives = 3;
        coins = 10;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) - 4;

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

    public void AddToScore(int n) {
        score += n;
    }
    // Update is called once per frame
    void Update() {
        HandlePlayerMovement();
        if (Input.GetKey(KeyCode.Space) && shootCount > attackDelay) {
            Shoot();
            shootCount = 0;
        }
        else
            shootCount += Time.deltaTime;
    }
    public void HitByBullet(int damage) {
        currentHealth -= damage;
        if (currentHealth< 0) {
            currentHealth = 0;
        }
    }
    public bool CheckCollision(GameObject bullet) {
        if (!bullet.TryGetComponent<EnemyBulletBehaviour>(out _))
            return false;
        Vector3 bulletPos = bullet.transform.position;
        if (Mathf.Pow(transform.position.x - bulletPos.x, 2) +
            Mathf.Pow(transform.position.y - bulletPos.y + EnemyBulletBehaviour.HIT_BOX_OFFSET_Y, 2) <=
            Mathf.Pow(firstRadius + EnemyBulletBehaviour.HIT_BOX_RADIUS, 2)) {
            return CheckSecondCollision(bullet);
        }
        return false;
    }
    private bool CheckSecondCollision(GameObject bullet) {
        Vector3 bulletPos = new(bullet.transform.position.x,
            bullet.transform.position.y + EnemyBulletBehaviour.HIT_BOX_OFFSET_Y,
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
                Mathf.Pow(EnemyBulletBehaviour.HIT_BOX_RADIUS + secondRadius, 2))
                return true;
        }

        return false;
    }
    //creates bullet objects for each bullet shot
    //sets the angle and inits the bullet with the angle and damage done
    void Shoot() {
        float angle;
        
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
            bullet.GetComponent<BulletBehaviour>().Init(damageDone, ToRadians(angle));
        }




    }
    private float ToRadians(float angle) {
        return angle / 180 * Mathf.PI;
    }

    //respawns the player
    public IEnumerator Respawn() {
        isSpawning = true;                              //set spawning flag
        for (int x = 0; x < 12; x++) {
            Color color = sr.color;
            color.a = color.a == 0 ? 255f : 0;
            sr.color = color;
            yield return new WaitForSeconds(0.25f);     //alternates every .25seconds between 0 and 255 alpha for 3 seconds
        }
        currentHealth = maxHealth;  //reset health
        isSpawning = false;         //clear flag
        DecrementLives();           //lose a life
        yield return null;          //exit coroutine
    }

    void HandlePlayerMovement() {

        if (Input.GetKey(KeyCode.A) && sr.bounds.min.x >= -maxX) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.left;
        }
        if (Input.GetKey(KeyCode.S) && sr.bounds.min.y >= -maxY) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.down;
        }
        if (Input.GetKey(KeyCode.D) && sr.bounds.max.x <= maxX) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && sr.bounds.max.y <= maxY) {
            transform.position += movementSpeed * Time.deltaTime * Vector3.up;
        }

    }
    public string getPlayerStats() {
        return "Health " + currentHealth + "/" + maxHealth + "\t" +
            "Armor " + armor + "\n" +
            "Bullets " + numBulletsFired + "\t\t" +
            "Atl Speed " + AttackSpeed + "\n" +
            "Pierce " + numTargetsPierced + "\t\t" + 
            "Damage " + damageDone;
    }
    public void SetUpgradeLevel(int upgradeId, int level) {
        switch(upgradeId) {
            case 0: currentHealth = maxHealth = level * HEALTH_PER_UPGRADE + BASE_HEALTH;
                break;
            case 1: armor = level;
                break;
            case 2: movementSpeed = BASE_SPEED + level * SPEED_PER_UPGRADE;
                break;
            case 3: damageDone = level + 1;
                break;
            case 4: attackDelay = BASE_ATTACK_DELAY + level * ATTACK_SPEED_PER_UPGRADE;
                break;
            case 5: numBulletsFired = level + 1;
                break;
        }
        upgradeLevels[upgradeId] = level;
    }

    public bool IsAlive() { return currentHealth > 0; }
    public void DecrementLives() { lives--; }
    public void IncrementLives() { lives++; }
    public void ResetLives() { lives = 3; }
    public bool OutOfLives() { return lives <= 0; }
    public void AddToCoins(int numCoins) {
        coins += numCoins;
    }
    public int Lives { get { return lives; } }

    public bool CanPurchaseItem(int itemCost, int shoppingCartTotal) {
        return coins >= itemCost + shoppingCartTotal;
    }
}
