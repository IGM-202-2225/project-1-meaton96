using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : EnemyBehaviour {

    private readonly Vector3 vertexOne = new(-7.62f, 2.14f, 0f);
    private readonly Vector3 vertexTwo = new(0, -4.54f, 0f);
    private readonly Vector3 vertexThree = new(7.62f, 2.14f, 0f);
    public float maxHealth;

    
    private void Awake() {
        vertRowSep = 0;

        //transform.localScale = new Vector3(scale, scale, scale);                    //shrink or grow enemy
        right = true;                                                         //set left or right direction
        health = 100;                                                              //init health, temporary
        animator = GetComponent<Animator>();
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        speed = 4f;
        shootDelay = .75f;
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        numBulletsFired = 20;
        damageDone = 1;
        maxHealth = health;
    }
    public override bool CheckCollision(float radius, Vector3 center) {
        float area1, area2, area3, area4;
        Vector3 A = vertexOne + transform.position;
        Vector3 B = vertexTwo + transform.position;
        Vector3 C = vertexThree + transform.position;
        area1 = FindArea(A, B, C);
        area2 = FindArea(center, B, C);
        area3 = FindArea(A, center, C);
        area4 = FindArea(A, B, center);
        Debug.Log(area1);
        Debug.Log(area2 + area3 + area4);
        return area1 == area2 + area3 + area4;
    }
    private float FindArea(Vector3 v1, Vector3 v2, Vector3 v3) {
        return Mathf.Abs((v1.x * (v2.y - v3.y) +
                          v2.x * (v3.y - v1.y) +
                          v3.x * (v1.y - v2.y)) / 2.0f);
    }




}
