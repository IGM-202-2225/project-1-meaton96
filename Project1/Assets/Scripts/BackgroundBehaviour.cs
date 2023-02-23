using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundBehaviour : MonoBehaviour {
    [SerializeField] Sprite[] bgSpriteList = new Sprite[4];                         
    public float bgMovementSpeed;
    private const float BG_RESET_POINT = -70, BG_DELETE_POINT = -100;
    [SerializeField] private GameObject bgPrefab;
    private GameObject currentBackground, nextBackground;
    private bool nextBgSpawned = false;
    private Vector3 BG_SPAWN_LOCATION = new Vector3(0, 70, 0);
    private Sprite bg;
    // Start is called before the first frame update
    void Start() {
        startNewLevel();
    }
    public void startNewLevel() {
        currentBackground = Instantiate(bgPrefab, BG_SPAWN_LOCATION, Quaternion.identity);
        currentBackground.transform.SetParent(GameObject.FindWithTag("bgCanvas").transform);
        bg = bgSpriteList[Random.Range(0, 4)];
        currentBackground.GetComponent<Image>().sprite = bg;
    }

    // Update is called once per frame
    // Moves the background image down slowly
    // when it is down far enough, load another copy of the background and begin moving that as well
    // swap the pointers when the first background is off the screen and delete the old backround
    void Update() {
        currentBackground.transform.position += bgMovementSpeed * Time.deltaTime * Vector3.down;

        if (nextBackground != null) {
            nextBackground.transform.position += bgMovementSpeed * Time.deltaTime * Vector3.down;
        }

        if (currentBackground.transform.position.y <= BG_RESET_POINT && !nextBgSpawned) {
            nextBackground = Instantiate(bgPrefab, new Vector3(0, 100, 0), Quaternion.identity);
            nextBackground.GetComponent<Image>().sprite = bg;
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
