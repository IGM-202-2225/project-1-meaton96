using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundBehaviour : MonoBehaviour {
    [SerializeField] Sprite[] bgSpriteList = new Sprite[4];                         
    public float bgMovementSpeed;
    private const float BG_RESET_POINT = -35, BG_DELETE_POINT = -50;
    [SerializeField] private GameObject bgPrefab;
    private GameObject currentBackground, nextBackground;
    private bool nextBgSpawned = false;
    private Vector3 BG_SPAWN_LOCATION = new Vector3(0, 35, 0);
    // Start is called before the first frame update
    void Start() {
        startNewLevel();
    }
    public void startNewLevel() {
        currentBackground = Instantiate(bgPrefab, BG_SPAWN_LOCATION, Quaternion.identity);
        currentBackground.transform.SetParent(GameObject.FindWithTag("bgCanvas").transform);
        currentBackground.GetComponent<Image>().sprite = bgSpriteList[Random.Range(0, 4)];
    }

    // Update is called once per frame
    void Update() {
        currentBackground.transform.position += bgMovementSpeed * Time.deltaTime * Vector3.down;

        if (nextBackground != null) {
            nextBackground.transform.position += bgMovementSpeed * Time.deltaTime * Vector3.down;
        }

        Debug.Log(currentBackground.transform.position);
        if (currentBackground.transform.position.y <= BG_RESET_POINT && !nextBgSpawned) {
            nextBackground = Instantiate(bgPrefab, new Vector3(0, 51, 0), Quaternion.identity);
            nextBackground.transform.SetParent(GameObject.FindWithTag("bgCanvas").transform);
            nextBgSpawned = true;
        }
        if (currentBackground.transform.position.y <= BG_DELETE_POINT) {
            Destroy(currentBackground);
            currentBackground = nextBackground;
            nextBackground = null;
            nextBgSpawned = false;
        }

    }
}
