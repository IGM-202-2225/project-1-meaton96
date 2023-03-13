using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : EnemyBehaviour {

    private readonly Vector3 vertexOne = new(-7.62f, 2.14f, 0f);                            //top left vertex
    private readonly Vector3 vertexTwo = new(0, -4.54f, 0f);                                //front middle vertex
    private readonly Vector3 vertexThree = new(7.62f, 2.14f, 0f);                           //top right vertex
    public float maxHealth;                                                                 //max health of boss
    private const int NUMBER_BULLET_WAVES = 10;                                             //number of times to shoot bullets during bullet hell
    private float attackDelay;                                                              //how long between boss abilities, set using different constants
    private const float BOMB_ATTACK_DELAY = 15f;                                            //how long to wait to attack after shooting bombs out
    private const float BULLET_HELL_ATTACK_DELAY = 2f;                                      //how long to wait to attack again after shooting bullets
    private float timer = 0f;                                                               //timer to blink the size indicator
    private bool inAttackPattern = false;                                                   //flag if the boss is currently doing an attack
    private const int BULLET_DAMAGE = 0;                                                    //how much damage each bullet does
    private const int MAX_HEALTH = 4000;                                                     //max health of the boss
    private const float SPEED = 4f;                                                         //how fast the boss moves right to left
    private const float SHOOT_DELAY = 0.75f;                                                //how long between shooting waves of bullets
    private const int NUM_BULLETS_FIRED = 20;                                               //number of bullets fired in a wave
    private const int NUM_BOMBS_X = 6;                                                      //grid size of bombs placed
    private const int NUM_BOMBS_Y = 3;
    private const float BOMB_THROW_DELAY = .25f;                                            //how long between waiting to fire bomb columns
    private const float START_X = -18;                                                      //bomb starting locations and spread distance
    private const float START_Y = -6;
    private const float BOMB_SPREAD_X = 7f;
    private const float BOMB_SPREAD_Y = 8f;
    private List<BombBehaviour> activeBombs;                                                //list of all bombs on the screen
    [SerializeField] private GameObject bombPrefab;                                         //bomb pre fab pointer
    protected override void Update() {
        DefaultLateralMovement();                           //move
        if (health <= 0) {                                  //check for death
            isAlive = false;
            animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);  //start explode animation
        }
        if (isAlive) {
            if (playerScript.CheckCollision(gameObject, HITBOX_RADIUS, 0f, 0f)) {
                isAlive = false;
                animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);
                playerScript.TakeDamage(DAMAGE_ON_COLLISION, false);
            }
        }
        //pick one of the two attack patterns at random and execute it, then wait a time before doing it again
        if (timer >= attackDelay && !inAttackPattern) {
            
            if (Random.Range(0f, 1f) >= 0.5f) {
                StartCoroutine(LobBombs());
            } 
            else {
                StartCoroutine(LobBombs());
            }
            timer = 0f;
        }
        else if (!inAttackPattern) {
            timer += Time.deltaTime;
        }

    }
    //shoots waves of bullets
    private IEnumerator BulletHell() {
        inAttackPattern = true;
        for (int x = 0; x < NUMBER_BULLET_WAVES; x++) {
            Shoot();
            yield return new WaitForSeconds(shootDelay);
        }
        inAttackPattern = false;
        attackDelay = BULLET_HELL_ATTACK_DELAY;
        yield return null;
    }
    //lombs the screen full of bombs
    private IEnumerator LobBombs() {
        inAttackPattern = true;
        int emptyX = Random.Range(0, NUM_BOMBS_X);
        int emptyY = Random.Range(0, NUM_BOMBS_Y);
        for (int x = 0; x < NUM_BOMBS_X; x++) {
            for (int y = 0; y < NUM_BOMBS_Y; y++) {
                if (x == emptyX && y == emptyY) 
                    continue;
                GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);

                bomb.GetComponent<BombBehaviour>().MoveToLocation(new Vector3(
                    x * BOMB_SPREAD_X + START_X,
                    y * BOMB_SPREAD_Y + START_Y,
                    0f));
                activeBombs.Add(bomb.GetComponent<BombBehaviour>());
            }
            yield return new WaitForSeconds(BOMB_THROW_DELAY);
        }
        activeBombs.ForEach(bomb => bomb.StartExplosionCountdown());
        inAttackPattern = false;
        attackDelay = BOMB_ATTACK_DELAY;
        yield return null;
    }
    //NYI
    private void PutUpShield() {

    }

    //init required values
    private void Awake() {
        activeBombs = new();
        vertRowSep = 0;
        right = true;                                                         //set left or right direction
        health = MAX_HEALTH;                                                              //init health, temporary
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        speed = SPEED;
        shootDelay = SHOOT_DELAY;
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        numBulletsFired = NUM_BULLETS_FIRED;
        damageDone = BULLET_DAMAGE;
        maxHealth = health;
    }
    //triangluar collision detection
    //returns true if the center vector is located inside the triangle formed by ABC
    public override bool CheckCollision(float radius, Vector3 center) {
        float area1, area2, area3, area4;
        Vector3 A = vertexOne + transform.position;
        Vector3 B = vertexTwo + transform.position;
        Vector3 C = vertexThree + transform.position;
        area1 = FindArea(A, B, C);
        area2 = FindArea(center, B, C);
        area3 = FindArea(A, center, C);
        area4 = FindArea(A, B, center);
        Debug.Log(area1);
        Debug.Log(area2 + area3 + area4);
        return area1 == area2 + area3 + area4;
    }
    //returns the area of a triangle made up by the 3 points
    private float FindArea(Vector3 v1, Vector3 v2, Vector3 v3) {
        return Mathf.Abs((v1.x * (v2.y - v3.y) +
                          v2.x * (v3.y - v1.y) +
                          v3.x * (v1.y - v2.y)) / 2.0f);
    }




}
