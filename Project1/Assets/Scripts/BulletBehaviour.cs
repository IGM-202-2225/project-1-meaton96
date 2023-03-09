using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    private const float HIT_BOX_OFFSET_Y = .52f, HIT_BOX_RADIUS = .05f;
    protected float hitboxRadius;
    public float speed;
    protected List<GameObject> enemyList;
    public float angle;
    protected Vector3 hitBoxCenter;
    protected bool destroy = false;
    protected Animator animator;

    public int numEnemiesPierced;
    //private bool firedByPlayer;
    float maxY;
    public int damage;
    // Start is called before the first frame update
    void Start() {
        hitboxRadius = HIT_BOX_RADIUS;
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + 5;
        enemyList = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemies;
    }

    // Update is called once per frame
    void Update() {
        if (destroy) {
            Destroy(gameObject);
            //maybe spawn explosion?
        }
        CheckCollisions();
        transform.position += new Vector3(
            speed * Time.deltaTime * Mathf.Sin(angle),
            speed * Time.deltaTime * Mathf.Cos(angle),
            0);
        
        if (transform.position.y > maxY || transform.position.y < -maxY) {
            Destroy(gameObject);
        }

    }
    public void Init(int damage, float angle, int pierce) {
        this.damage = damage;
        this.angle = angle;
        numEnemiesPierced= pierce;
    }


    protected virtual void CheckCollisions() {
        

        foreach (GameObject enemy in enemyList) {
            EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
            hitBoxCenter = new(transform.position.x, transform.position.y + HIT_BOX_OFFSET_Y, 0);
            if (enemyScript.CheckCollision(hitboxRadius, hitBoxCenter)) {
                if (numEnemiesPierced <= 0) {
                    destroy = true;
                    
                }
                else
                    numEnemiesPierced--;
                enemyScript.health -= damage;
                if (enemyScript.health <= 0) {
                    enemyScript.isAlive = false;
                }   

                if (!enemyScript.isAlive)
                    enemyList.Remove(enemy);
                break;
            }
        }
    }
}
