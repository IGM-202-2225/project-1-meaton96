
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    private const float RADIUS_1 = .7f;
    private const float DEFAULT_SHOOT_DELAY = 3f;
    public float scale = .7f;
    private const float VERTICAL_ROW_SEPERATION = 3;
    private const string ANIMATOR_EXPLODE_TRIGGER = "explode";
    private const int ANIMATION_FRAMES = 9;
    public bool isAlive;
    private Animator animator;
    private float maxX, maxY;
    private bool right;
    private float speed;
    private SpriteRenderer sr;
    public int damage;
    public int health;
    private float shootTimer = 0f, shootDelay;
    [SerializeField] private GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start() {


    }

    // Update is called once per frame
    void Update() {
        //DefaultLateralMovement();
        if (health <= 0) {
            isAlive = false;
            animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);
        }
        if (shootTimer >= shootDelay) {
            Shoot();
            shootTimer = 0;
        }
        else {
            shootTimer += Time.deltaTime;
        }

    }
    void Shoot() {
        Vector3 pos = new(transform.position.x, transform.position.y - 1, 0);
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.GetComponent<EnemyBulletBehaviour>().Init(damage);

    }

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
    }


    public void Init(int type, int speed, bool right, int damage) {

        transform.localScale = new Vector3(scale, scale, scale);
        this.right = right;
        health = 1;
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y);
        animator.SetTrigger("ship" + type);
        this.speed = speed;
        this.damage = damage;
        shootDelay = Random.Range(0, DEFAULT_SHOOT_DELAY / (type + 1));

    }
    public void Init(int type, int speed, bool right, float scale, int damage) {
        this.scale = scale;
        Init(type, speed, right, damage);
    }

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
            Mathf.Pow(RADIUS_1 * scale + radius, 2)) {
            return true;
        }
        return false;
    }

    public void FinishExplode() {
        Destroy(gameObject);
    }
}
