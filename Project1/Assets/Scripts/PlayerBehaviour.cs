using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject bgControl;
    [SerializeField] private float lateralSpeed, verticalSpeed;
    private SpriteRenderer sr;
    private float maxX, maxY;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        maxX = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).x);
        maxY = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y);
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();
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
