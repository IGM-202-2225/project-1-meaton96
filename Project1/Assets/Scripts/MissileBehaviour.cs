using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : BulletBehaviour {
    private GameObject targetObject;
    private Vector3 targetVec;
    private const float HITBOX_RAD = .5f, HITBOX_OFFSET = -.35f;
    private const float EXPLOSION_RADIUS = 2f;
    private const float MOUSE_HITBOX_RAD = .25f;
    [SerializeField] private SpriteRenderer firePlume;
    [SerializeField] private GameObject firePlumeO;
    private Color color;
    private float elapsedTime = 0;
    public float maxSpeed;
    private const float ACCELERATION_TIME = 1f;
    public const int ACCELERATION_FRAMES = 90;
    private Vector3 previousPos;
    public bool friendly;
    private const float UPDATE_TIMER = .1f;
    private float timer = 0f;
    private Vector3 travelVector;
    private const int EXPLOSION_DAMAGE = 2;
    private bool backwards;
    private void Awake() {
        animator = GetComponent<Animator>();
        damage = 50;
        speed = 0;
        hitboxRadius = HITBOX_RAD;
    }
    public void Explode() {
        Destroy(gameObject);
    }
    public void DamageNearbyEnemies() {
        foreach (GameObject enemy in enemyList) {
            EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
            if (enemyScript.CheckCollision(EXPLOSION_RADIUS, transform.position)) {
                enemyScript.health -= EXPLOSION_DAMAGE;
            }
        }
    }
    // Update is called once per frame
    void Update() {
        Debug.Log(firePlumeO.transform.position.ToString());
        if (destroy) {
            animator.SetTrigger("boom");
        }
        else {
            if (targetObject != null) {
                targetVec = targetObject.transform.position;
            }
            if (elapsedTime < ACCELERATION_TIME) {
                speed += maxSpeed * ACCELERATION_TIME * Time.deltaTime;
                elapsedTime += Time.deltaTime;
            }

            travelVector = speed * Time.deltaTime * (targetVec - transform.position).normalized;
            transform.position += travelVector;
            targetVec += travelVector;

            if (timer >= UPDATE_TIMER) {

                angle = Mathf.Atan((transform.position.x - previousPos.x) /
                    (transform.position.y - previousPos.y)) * -Mathf.Rad2Deg + (backwards ? 180 : 0);

                hitBoxCenter = new Vector3(transform.position.x + Mathf.Sin(angle * Mathf.Deg2Rad) * -HITBOX_OFFSET,
                    transform.position.y + Mathf.Cos(angle * Mathf.Deg2Rad) * HITBOX_OFFSET,
                    0f);

                transform.rotation = Quaternion.Euler(0, 0, angle);
                timer = 0f;
            }
            else {
                timer += Time.deltaTime;
            }

            color = firePlume.color;
            color.a = color.a == 0 ? 255 : 0;
            firePlume.color = color;
           
            CheckCollisions();

            previousPos = transform.position;
        }
    }


    public void Fire(Vector3 target, bool friendly, bool backwards) {
        enemyList = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemies;
        this.friendly = friendly;
        hitBoxCenter = transform.position - new Vector3(0f, HITBOX_OFFSET, 0f);
        foreach (GameObject enemy in enemyList) {
            if (enemy.GetComponent<EnemyBehaviour>().CheckCollision(MOUSE_HITBOX_RAD, target)) {
                targetObject = enemy;
                return;
            }
        }
        previousPos = transform.position;
        targetVec = new Vector3(target.x, target.y, 0f);
        this.backwards = backwards;
        transform.GetChild(0).transform.position = transform.position + new Vector3(0, -1.255f, 0); // for some reason my fire plume was spawning at z = -22 for some reason so work around

    }
}
