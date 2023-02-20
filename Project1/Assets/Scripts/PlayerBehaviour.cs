using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float firstRadius = 1f, secondRadius = 0.4f;
    public float firstRadiusYOffset = .5f;
    public float secondRadiusXOffset = .1f;
    public float thirdRadiusYOffset = -0.4f, thirdRadiusXOffset = 0.25f;
    public int currentHealth = 10;
    public int maxHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        shipType = 0;

        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) - 4;

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
    public void HitByBullet(int damage) {
        currentHealth -= damage;    
    }
    public bool CheckCollision(GameObject bullet) {
        if (!bullet.TryGetComponent<EnemyBulletBehaviour>(out _))
            return false;
        Vector3 bulletPos = bullet.transform.position;
        if (Mathf.Pow(transform.position.x - bulletPos.x, 2) +
            Mathf.Pow(transform.position.y - bulletPos.y + EnemyBulletBehaviour.HIT_BOX_OFFSET_Y, 2) <=
            Mathf.Pow(firstRadius + EnemyBulletBehaviour.HIT_BOX_RADIUS, 2)) {
                return CheckSecondCollision(bullet);
        }
        return false;
    }
    private bool CheckSecondCollision(GameObject bullet) {
        Vector3 bulletPos = new(bullet.transform.position.x,
            bullet.transform.position.y + EnemyBulletBehaviour.HIT_BOX_OFFSET_Y,
            0);

        Vector3[] hitBoxCenters = new Vector3[5];
        hitBoxCenters[0] = new Vector3(transform.position.x,
            transform.position.y + firstRadiusYOffset, 0);
        hitBoxCenters[1] = new Vector3(transform.position.x + secondRadiusXOffset, transform.position.y, 0);
        hitBoxCenters[2] = new Vector3(transform.position.x - secondRadiusXOffset, transform.position.y, 0);
        hitBoxCenters[3] = new Vector3(transform.position.x + thirdRadiusXOffset, transform.position.y + thirdRadiusYOffset, 0);
        hitBoxCenters[4] = new Vector3(transform.position.x - thirdRadiusXOffset, transform.position.y + thirdRadiusYOffset, 0);

        for (int x = 0; x < hitBoxCenters.Length; x++) {
            if (Mathf.Pow(hitBoxCenters[x].x - bulletPos.x, 2) +
                Mathf.Pow(hitBoxCenters[x].y - bulletPos.y, 2) <=
                Mathf.Pow(EnemyBulletBehaviour.HIT_BOX_RADIUS + secondRadius, 2))
                return true;
        }

        return false;
    }

    void Shoot() {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletBehaviour>().Init(1);

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
