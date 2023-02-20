using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    public const float HIT_BOX_OFFSET_Y = .2f, HIT_BOX_RADIUS = .2f;
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    //private bool firedByPlayer;
    float maxY;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + 5;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions();
        transform.position -= new Vector3(
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

        PlayerBehaviour playerScript = GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameController>().
            player.GetComponent<PlayerBehaviour>();

        if (playerScript.CheckCollision(gameObject)) {
            playerScript.HitByBullet(damage);
            Destroy(gameObject);
        }

    }
}
