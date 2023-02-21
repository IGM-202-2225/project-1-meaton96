
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
    public int damage;                  //how much damage the enemy does with its shot
    public int health;                  //how much health the enemy has
    private float shootTimer = 0f, shootDelay;          //floats to track and delay the time between enemy shots
    [SerializeField] private GameObject bulletPrefab;   //prefab for the enemy bullet
    // Start is called before the first frame update
    void Start() {


    }

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
        Vector3 pos = new(transform.position.x, transform.position.y - 1, 0);
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.GetComponent<EnemyBulletBehaviour>().Init(damage);

    }

    //default left or right movement for simple enemy waves
    //when enemy hits the edge of the screen it moves down towards the bottom of the screen
    private void DefaultLateralMovement() {
        transform.position += (right ? Vector3.right : Vector3.left) * Time.deltaTime * speed;
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


    public void Init(int type, int speed, bool right, int damage) {

        transform.localScale = new Vector3(scale, scale, scale);                    //shrink or grow enemy
        this.right = right;                                                         //set left or right direction
        health = type;                                                              //init health, temporary
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y);
        animator.SetTrigger("ship" + type);     //change the ship model by setting animation trigger
        this.speed = speed;
        this.damage = damage;
        shootDelay = DEFAULT_SHOOT_DELAY;

    }
    public void Init(int type, int speed, bool right, float scale, int damage) {
        this.scale = scale;
        Init(type, speed, right, damage);
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
        Destroy(gameObject);
    }
}
