
using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    protected const float HITBOX_RADIUS = .7f;                     //radius of hitbox
    protected const float DEFAULT_SHOOT_DELAY = 10f;        //default delay between shooting
    public float scale = .7f;                             //how big to scale the model and hitbox  
    protected const float VERTICAL_ROW_SEPERATION = 3;      //how far apart the rows are, how many units to move down when doing default movement
    protected float vertRowSep;
    protected const string ANIMATOR_EXPLODE_TRIGGER = "explode";  //trigger tag for starting explode animation
    public bool isAlive;                                    //keeps track if the enemy is alive or not
    protected Animator animator;                              //pointer to animator component
    protected float maxX;                                     //how far left or right the enemy can move doing default movement
    protected bool right;                                  //keeps track if the enemy is moving left or right
    protected float speed;                                 //how fast the enemy is moving
    protected SpriteRenderer sr;                           //pointer to sprite render component
    public float damageDone;                                //how much damage the enemy does with its shot
    public int health;                                      //how much health the enemy has
    protected float shootTimer = 0f, shootDelay;          //floats to track and delay the time between enemy shots
    [SerializeField] protected GameObject bulletPrefab;   //prefab for the enemy bullet
    int type;
    private bool killedByPlayer = false;
    private float bottomOfScreen;
    protected PlayerBehaviour playerScript;
    public int numBulletsFired;
    public float bulletSpreadAngle = 10f;
    protected const int BASE_SCORE = 100;
    protected const float BASE_COIN_DROP_CHANCE = .5f;
    protected const float BULLET_LENGTH = .45f;
    protected const float DAMAGE_ON_COLLISION = 10f;
    public bool movingOnToScreen;
    public static int ID = 0;
    public int id;
    private float leftSide, rightSide;
    public static readonly float[] DAMAGE_DONE_EACH_ENEMY = {
        10f,
        10f,
        20f,
        25f,
        35f
    };
    public static readonly int[] NUM_BULLETS_EACH_ENEMY = { 1,3,1,4,6 };
    public static readonly Color[] COLOR_EACH_ENEMY = {
        Color.white,
        Color.green,
        Color.grey,
        Color.yellow,   
        Color.red
    };
    public static readonly Color[] BULLET_COLORS = {
        Color.white,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.red,
    };



    // Update is called once per frame
    protected virtual void Update() {
        if (movingOnToScreen) {
            if (transform.position.x > leftSide + sr.bounds.extents.x &&
                transform.position.x < rightSide - sr.bounds.extents.x) {
                movingOnToScreen = false;
            }
        }
        DefaultLateralMovement();                           //move
        if (transform.position.y <= bottomOfScreen) {
            health = 0;
        }
        if (health <= 0) {                                  //check for death
            isAlive = false;
            animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);  //start explode animation
        }
        if (isAlive) {
            if (playerScript.CheckCollision(gameObject, HITBOX_RADIUS, 0f, 0f)) {
                isAlive= false;
                animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);
                playerScript.TakeDamage(DAMAGE_ON_COLLISION, false);
            }
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
    protected void Shoot() {
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
    protected virtual void DefaultLateralMovement() {
        transform.position += Time.deltaTime * speed * (right ? Vector3.right : Vector3.left);
        if (!movingOnToScreen && (sr.bounds.max.x >= maxX || sr.bounds.min.x <= -maxX)) {
            float xOffsetOnRowChange = .1f;
            if (right)
                xOffsetOnRowChange = -xOffsetOnRowChange;
            right = !right;
            transform.position = new Vector3(transform.position.x + xOffsetOnRowChange,
                transform.position.y - vertRowSep, 0);
        }
        //add in check for hitting bottom of the screen
    }
    public void TakeDamage(int dam) {
        health -= dam;
        if (health < 0) health = 0;
        if (health == 0) {
            killedByPlayer = true;
            isAlive = false;
        }
        else {
            ChangeSpriteAlpha(.5f);
            StartCoroutine(FlashOnDamageTake());
        }
    }

    private void ChangeSpriteAlpha(float alpha) {
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = alpha;
        GetComponent<SpriteRenderer>().color = c;
    }

    public void Init(int type, float speed, bool right) {
        movingOnToScreen = true;
        id = ID++;
        vertRowSep = VERTICAL_ROW_SEPERATION;
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
        shootDelay = Random.Range(DEFAULT_SHOOT_DELAY / 2.0f, DEFAULT_SHOOT_DELAY) / (type + 1); //enemies scale harder
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        numBulletsFired = NUM_BULLETS_EACH_ENEMY[type];
        damageDone = DAMAGE_DONE_EACH_ENEMY[type];
        sr.color = COLOR_EACH_ENEMY[type];
        bottomOfScreen = Camera.main.ScreenToWorldPoint(Vector3.zero).y + 4;
        leftSide = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        rightSide = -leftSide;
        movingOnToScreen = true;

    }
    IEnumerator FlashOnDamageTake() {
        yield return new WaitForSeconds(.75f);
        ChangeSpriteAlpha(1f);
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
    public virtual bool CheckCollision(float radius, Vector3 center) {
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
            Mathf.Pow(HITBOX_RADIUS * scale + radius, 2)) {
            return true;
        }
        return false;
    }

    //called by keyframe event in the animation
    public void FinishExplode() {
        if (killedByPlayer) {
            PlayerBehaviour playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            playerScript.AddToScore(BASE_SCORE * (type + 1));
            float coinsDropped = BASE_COIN_DROP_CHANCE * type;
            int numCoinsDropped = 0;
            if (coinsDropped != (int)coinsDropped && Random.Range(0, 1) <= BASE_COIN_DROP_CHANCE)
                numCoinsDropped++;

            numCoinsDropped += (int)coinsDropped;
            playerScript.AddToCoins(numCoinsDropped);
        }
        Destroy(gameObject);
    }
    

}

