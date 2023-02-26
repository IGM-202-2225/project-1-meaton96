
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    private const float RADIUS = .7f;                     //radius of hitbox
    private const float DEFAULT_SHOOT_DELAY = 10f;        //default delay between shooting
    public float scale = .7f;                             //how big to scale the model and hitbox  
    private const float VERTICAL_ROW_SEPERATION = 3;      //how far apart the rows are, how many units to move down when doing default movement
    private const string ANIMATOR_EXPLODE_TRIGGER = "explode";  //trigger tag for starting explode animation
    public bool isAlive;                //keeps track if the enemy is alive or not
    private Animator animator;          //pointer to animator component
    private float maxX;                 //how far left or right the enemy can move doing default movement
    private bool right;                 //keeps track if the enemy is moving left or right
    private float speed;                //how fast the enemy is moving
    private SpriteRenderer sr;          //pointer to sprite render component
    public float damageDone;                  //how much damage the enemy does with its shot
    public int health;                  //how much health the enemy has
    private float shootTimer = 0f, shootDelay;          //floats to track and delay the time between enemy shots
    [SerializeField] private GameObject bulletPrefab;   //prefab for the enemy bullet
    int type;
    public int numBulletsFired;
    public float bulletSpreadAngle = 10f;
    private const int BASE_SCORE = 100;
    private const float BASE_COIN_DROP_CHANCE = .5f;
    private const float BULLET_LENGTH = .45f;
    private readonly float[] DAMAGE_DONE_EACH_ENEMY = {
        10f,
        10f,
        20f,
        25f,
        35f
    };

    private Color[] BULLET_COLORS = {
        Color.white,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.red,
    };

    // Update is called once per frame
    void Update() {
        
        DefaultLateralMovement();                           //move
        if (health <= 0) {                                  //check for death
            isAlive = false;
            animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);  //start explode animation
        }
        if (shootTimer >= shootDelay) {
            Shoot();
            shootTimer = 0;
        }
        else {
            shootTimer += Time.deltaTime;
        }

    }
    //spawns a bullet at enemy position
    void Shoot() {
        //Vector3 pos = new(transform.position.x, transform.position.y - 1, 0);
        //GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        //bullet.GetComponent<EnemyBulletBehaviour>().Init(damage);
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
            Vector3 pos = new(transform.position.x,
                transform.position.y - BULLET_LENGTH,
                0f);
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.Euler(new Vector3(0f, 0f, angle)));

            int bulletColor = type;
            if (bulletColor >= BULLET_COLORS.Length)
                bulletColor = BULLET_COLORS.Length;

            bullet.GetComponent<EnemyBulletBehaviour>().Init(damageDone, ToRadians(angle), BULLET_COLORS[bulletColor]);
        }
    }
    private float ToRadians(float angle) {
        return angle / 180 * Mathf.PI;
    }

    //default left or right movement for simple enemy waves
    //when enemy hits the edge of the screen it moves down towards the bottom of the screen
    private void DefaultLateralMovement() {
        transform.position += Time.deltaTime * speed * (right ? Vector3.right : Vector3.left);
        if (sr.bounds.max.x >= maxX || sr.bounds.min.x <= -maxX) {
            float xOffsetOnRowChange = .1f;
            if (right)
                xOffsetOnRowChange = -xOffsetOnRowChange;
            right = !right;
            transform.position = new Vector3(transform.position.x + xOffsetOnRowChange,
                transform.position.y - VERTICAL_ROW_SEPERATION, 0);
        }
        //add in check for hitting bottom of the screen
    }


    public void Init(int type, float speed, bool right) {
        
        this.type = type;
        transform.localScale = new Vector3(scale, scale, scale);                    //shrink or grow enemy
        this.right = right;                                                         //set left or right direction
        health = type + 1;                                                              //init health, temporary
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        animator.SetTrigger("ship" + type);     //change the ship model by setting animation trigger
        this.speed = speed;
        shootDelay = Random.Range(DEFAULT_SHOOT_DELAY/ 2.0f, DEFAULT_SHOOT_DELAY) / (type + 1); 

        switch(type) {
            case 0: 
                sr.color = Color.white;
                numBulletsFired = 1;
                damageDone = 10;
                break;
            case 1: sr.color = Color.green;
                numBulletsFired = 3;
                damageDone = 10;
                break;
            case 2: 
                sr.color = Color.grey;
                numBulletsFired = 1;
                damageDone = 20;
                break;
            case 3: 
                sr.color = Color.red;
                numBulletsFired = 4;
                damageDone = 20;
                break;
            case 4:
                sr.color = Color.blue;
                numBulletsFired = 3;
                damageDone = 30;
                break;
            case 5: sr.color = Color.magenta;
                numBulletsFired = 6;
                damageDone = 35;
                break;
        }
        damageDone = DAMAGE_DONE_EACH_ENEMY[type];

    }
    public void SetBulletsFired(int num) {
        numBulletsFired = num;
    }
    public void Init(int type, float speed, bool right, float scale) {
        this.scale = scale;
        Init(type, speed, right);
    }
    public void SetDamage(int damage) {
        damageDone = damage;
    }

    //check for collision with the something located at 'center' vector with hit box circle of radius 'radius'
    public bool CheckCollision(float radius, Vector3 center) {
        if (!isAlive)
            return false;
        if (gameObject == null)
            return false;
        if (center == null)
            return false;
        if (radius <= 0)
            return false;

        if (Mathf.Pow(transform.position.x - center.x, 2) +
            Mathf.Pow(transform.position.y - center.y, 2) <=
            Mathf.Pow(RADIUS * scale + radius, 2)) {
            return true;
        }
        return false;
    }

    //called by keyframe event in the animation
    public void FinishExplode() {
        PlayerBehaviour playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        playerScript.AddToScore(BASE_SCORE * (type + 1));
        float coinsDropped = BASE_COIN_DROP_CHANCE * type;
        int numCoinsDropped = 0;
        if (coinsDropped != (int)coinsDropped && Random.Range(0, 1) <= BASE_COIN_DROP_CHANCE)
            numCoinsDropped++;

        numCoinsDropped += (int)coinsDropped;
        playerScript.AddToCoins(numCoinsDropped);
        Destroy(gameObject);
    }

}

