using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    private const float RADIUS_1 = .7f;
    private const float VERTICAL_ROW_SEPERATION = 3;
    private const string ANIMATOR_EXPLODE_TRIGGER = "explode";
    private const int ANIMATION_FRAMES = 9;
    private bool isAlive;
    private Animator animator;
    private float maxX, maxY;
    private bool right;
    [SerializeField] private float speed;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start() {
        

    }
    public void Init() {
        
    }
    // Update is called once per frame
    void Update() {
        DefaultLateralMovement();
    }

    private void DefaultLateralMovement() {
        //needs adjusting if enemy goes too far off screen
        transform.position += (right ? Vector3.right : Vector3.left) * Time.deltaTime * speed;
        if (sr.bounds.max.x >= maxX || sr.bounds.min.x <= -maxX) {
            right = !right;
            transform.position = new Vector3(transform.position.x,
                transform.position.y - VERTICAL_ROW_SEPERATION, 0);
        }
    }

    public void Init(int type) {
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        right = true;
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y);
        animator.SetTrigger("ship" + type);
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
            Mathf.Pow(RADIUS_1 + radius, 2)) {
            return true;
        }
        return false;
    }

    public void Explode() {
        isAlive = false;
        animator.SetTrigger(ANIMATOR_EXPLODE_TRIGGER);
    }
    public void FinishExplode() {
        Destroy(gameObject);
    }
}
