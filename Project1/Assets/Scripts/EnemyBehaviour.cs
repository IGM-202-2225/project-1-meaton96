using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    private const float RADIUS_1 = .7f;
    private const string ANIMATOR_EXPLODE_TRIGGER = "explode";
    private const int ANIMATION_FRAMES = 9;
    private bool isAlive;
    private Animator animator;
    // Start is called before the first frame update
    void Start() {
        
        
    }

    // Update is called once per frame
    void Update() {

    }
    public void SetShipType(int type) {
        animator = GetComponent<Animator>();
        isAlive = true;

        animator.SetTrigger("ship" + type);
    }

    public bool CheckCollision(float radius, Vector3 center) {
        //collision not working for ship 4
        if (!isAlive)
            return false;
        if (gameObject == null)
            return false;
        if (center == null)
            return false;
        if (radius < 0) 
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
