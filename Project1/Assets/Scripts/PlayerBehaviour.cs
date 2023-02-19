using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject bgControl;
    [SerializeField] private float lateralSpeed, verticalSpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite[] shipSprites = new Sprite[3];
    private int shipType;
    private SpriteRenderer sr;
    private float maxX, maxY;
    [SerializeField] private float bulletSpawnOffset;
    [SerializeField] private float shootDelay = .2f;
    private float shootCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        shipType = 0;

        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y);

        switch (shipType) {
            case 0:sr.sprite = shipSprites[0]; 
                break;
            case 1: sr.sprite = shipSprites[1];
                break;
            case 2: sr.sprite = shipSprites[2];
                break;
        }
    }

    // Update is called once per frame
    void Update()   
    {
        HandlePlayerMovement();
        if (Input.GetKey(KeyCode.Space) && shootCount > shootDelay) {
            Shoot();
            shootCount = 0;
        }
        else
            shootCount += Time.deltaTime;
    }

    void Shoot() {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletBehaviour>().Init(true);

    }

    void HandlePlayerMovement() {

        if (Input.GetKey(KeyCode.A) && sr.bounds.min.x >= -maxX) {
            transform.position += lateralSpeed * Time.deltaTime * Vector3.left;
        }
        if (Input.GetKey(KeyCode.S) && sr.bounds.min.y >= -maxY) {
            transform.position += verticalSpeed * Time.deltaTime * Vector3.down;
        }
        if (Input.GetKey(KeyCode.D) && sr.bounds.max.x <= maxX) {
            transform.position += lateralSpeed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && sr.bounds.max.y <= maxY) {
            transform.position += verticalSpeed * Time.deltaTime * Vector3.up;
        }
        
    }
    
}
