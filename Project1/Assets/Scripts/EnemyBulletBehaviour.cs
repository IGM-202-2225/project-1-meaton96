using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    public const float HIT_BOX_OFFSET_Y = .2f, HIT_BOX_RADIUS = .2f;    //how far off center and size of the circle hit box
    [SerializeField] private float speed;                               //how fast the bullet moves
    [SerializeField] private float angle;                               //angle off straight down
    private float maxY;                                                 //edge of the screen
    public int damage;                                                  //how much damage the bullet does
    PlayerBehaviour playerScript;

    // Start is called before the first frame update
    void Start()
    {
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + 5;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions();                              //check for collisions with player
        transform.position += new Vector3(              //adjust position, move bullet
            speed * Time.deltaTime * Mathf.Sin(angle),
            -speed * Time.deltaTime * Mathf.Cos(angle),
            0);
        
        if (transform.position.y > maxY || transform.position.y < -maxY) {
            Destroy(gameObject);                        //destroy if off the screen
        }

    }
    public void Init(int damage, float angle) {
        this.damage = damage;                   //set damage
        playerScript = GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameController>().
            player.GetComponent<PlayerBehaviour>(); //get player script instance
        this.angle = angle;
    }

    //checks if the bullet hit the player by calling player CheckCollision
    void CheckCollisions() { 
        if (playerScript.CheckCollision(gameObject)) {
            playerScript.HitByBullet(damage);
            Destroy(gameObject);
        }

    }
}
