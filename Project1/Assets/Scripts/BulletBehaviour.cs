using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    private const float HIT_BOX_OFFSET_Y = .52f, HIT_BOX_RADIUS = .05f;
    [SerializeField] private float speed;
    private List<GameObject> enemyList;
    public float angle;

    public int numEnemiesPierced;
    //private bool firedByPlayer;
    float maxY;
    public int damage;
    // Start is called before the first frame update
    void Start() {
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + 5;
        enemyList = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemies;
    }

    // Update is called once per frame
    void Update() {
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


    void CheckCollisions() {
        

        foreach (GameObject enemy in enemyList) {
            EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
            Vector3 bulletPos = new(transform.position.x, transform.position.y + HIT_BOX_OFFSET_Y, 0);
            if (enemyScript.CheckCollision(HIT_BOX_RADIUS, bulletPos)) {
                if (numEnemiesPierced <= 0) {
                    Destroy(gameObject);
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
