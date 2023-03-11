using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : BulletBehaviour {
    private GameObject targetObject;                                                //pointer to the target object if one exists
    private Vector3 targetVec;                                                      //pointer to target vector
    private const float HITBOX_RAD = .5f, HITBOX_OFFSET = -.35f;                    //hitbox offsets
    private const float EXPLOSION_RADIUS = 2f;                                      //radius how big the secondary explosion is
    [SerializeField] private SpriteRenderer firePlume;                              //pointer to fire plume to create flicker effect
    private Color color;                                                            //color to hold temp value for flickering the plume
    private float elapsedTime = 0;                                                  //timer to count elapsed time when missile is first fired
    public float maxSpeed;                                                          //the missile max speed
    private const float ACCELERATION_TIME = 1f;                                     //how long in seconds before the rocket will hit max speed after being fired
    private Vector3 previousPos;                                                    //keeps track of the previous position every .1 seconds to maintain the proper rotation
    public bool friendly;                                                           //if the missile was fired by the player
    private const float UPDATE_TIMER = .1f;                                         //how often to update the position
    private float timer = 0f;                                                       //timer counter
    private Vector3 travelVector;                                                   //the direction the missile is traveling
    private const int EXPLOSION_DAMAGE = 2;                                         //how much damage secondary explosion does
    private bool backwards;                                                         //if the missile was fired backwards, used to flip the rocket sprite 180 degrees
    private void Awake() {
        animator = GetComponent<Animator>();
        damage = 50;
        speed = 0;
        hitboxRadius = HITBOX_RAD;
    }
    //called at the end of the explosion animation
    public void Explode() {
        Destroy(gameObject);
    }
    //checks the missile explosion hitbox against all the enmies and damages them if they are close enough
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
        //destroy the game object on the next frame after a collision
        if (destroy) {
            animator.SetTrigger("boom");
        }
        else {
            //if there is a target object, update the target vector to where the target object is
            if (targetObject != null) {
                targetVec = targetObject.transform.position;
            }
            //increase speed if still in acceleration phase
            if (elapsedTime < ACCELERATION_TIME) {
                speed += maxSpeed * ACCELERATION_TIME * Time.deltaTime;
                elapsedTime += Time.deltaTime;
            }
            //get the travel vector and move the missile by that much
            //also increase target vector by the same amount so as to have the missile continue off screen if there is not target object
            travelVector = speed * Time.deltaTime * (targetVec - transform.position).normalized;
            transform.position += travelVector;
            targetVec += travelVector;

            //update the angle, and move the hitbox 
            //delayed every UPDATE_TIMER seconds as its not necessary to do both these things every frame
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

            //flicker the fire plume every frame to make it look like its burning kinda
            color = firePlume.color;
            color.a = color.a == 0 ? 255 : 0;
            firePlume.color = color;
           
            //check collision with enemies
            CheckCollisions();

            previousPos = transform.position;
        }
    }

    /// <summary>
    /// needs to be called when object is instantiated, sets important variables and sets a target object 
    /// </summary>
    public void Fire(Vector3 target, bool friendly, bool backwards) {
        enemyList = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemies;
        this.friendly = friendly;
        hitBoxCenter = transform.position - new Vector3(0f, HITBOX_OFFSET, 0f);
        
        previousPos = transform.position;
        targetVec = new Vector3(target.x, target.y, 0f);
        this.backwards = backwards;
        //not good last minute fix
        transform.GetChild(0).transform.position = transform.position + new Vector3(0, -1.255f, 0); // for some reason my fire plume was spawning at z = -22 for some reason so work around

    }
    //overload of Fire method to set target object if player clicked on an enemy
    public void Fire(GameObject target, bool friendly, bool backwards) {
        targetObject = target;
        Fire(target.transform.position, friendly, backwards);

    }
}
