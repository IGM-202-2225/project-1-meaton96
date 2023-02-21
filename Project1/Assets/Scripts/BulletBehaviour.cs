using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    private const float HIT_BOX_OFFSET_Y = -.05f, HIT_BOX_RADIUS = .05f;
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    //private bool firedByPlayer;
    float maxY;
    public int damage;
    // Start is called before the first frame update
    void Start() {
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + 5;
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
    public void Init(int damage) {
        this.damage = damage;
    }


    void CheckCollisions() {
        List<GameObject> enemyList = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemies;

        foreach (GameObject enemy in enemyList) {
            EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
            Vector3 bulletPos = new(transform.position.x, transform.position.y + HIT_BOX_OFFSET_Y, 0);
            if (enemyScript.CheckCollision(HIT_BOX_RADIUS, bulletPos)) {
                Destroy(gameObject);
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
