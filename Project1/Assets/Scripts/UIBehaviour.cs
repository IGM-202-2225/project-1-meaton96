using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour {
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject InfoBox;
    private Vector2 livesStartingPoint;
    [SerializeField] private float livesPadding;
    [SerializeField] private GameObject lifePreFab;
    private const float LIFE_SCALE = .7f;
    private PlayerBehaviour playerScript;
    private Stack<GameObject> lives;
    private bool infoDisplayed;
    // Start is called before the first frame update
    void Start() {
        lives = new();
        infoDisplayed = infoText.IsActive();
        livesStartingPoint = new Vector3(0,
            -Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector3.zero).y) + .8f, 0);


        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        GameObject life;
        Vector3 lifeDrawPoint;
        for (int x = 0; x < playerScript.Lives; x++) {
            //create a new life prefab instance, alternates between creating one on the left and right 
            if (x == 0) {
                lifeDrawPoint = new Vector3(0, livesStartingPoint.y, 0);
            }
            else if (x % 2 != 0) {
                lifeDrawPoint = new Vector3(x * -livesPadding, livesStartingPoint.y, 0);
            }
            else
                lifeDrawPoint = new Vector3((x - 1) * livesPadding, livesStartingPoint.y, 0);

            life = Instantiate(lifePreFab, lifeDrawPoint, Quaternion.identity);



            life.transform.parent = transform;
            life.transform.localScale = new Vector3(LIFE_SCALE, LIFE_SCALE, 1);
            lives.Push(life);
        }

    }

    // Update is called once per frame
    void Update() {
        if (playerScript != null) {
            healthBar.transform.localScale = new Vector3(
                playerScript.currentHealth * 1.0f / playerScript.maxHealth, 1, 1);

        }
        scoreText.text = "Score: " + playerScript.score;
        coinText.text = playerScript.coins + "";
        infoText.text = playerScript.getPlayerStats();


        if (playerScript.Lives > 0) {
            if (lives.Count != playerScript.Lives) {
                while (lives.Count > playerScript.Lives) {
                    Destroy(lives.Pop());
                }
                while (lives.Count < playerScript.Lives) {
                    GameObject life = Instantiate(lifePreFab,
                        new Vector3(livesStartingPoint.x + lives.Count - 1 * livesPadding,
                        livesStartingPoint.y, 0), Quaternion.identity);
                    life.transform.parent = transform;
                    lives.Push(life);
                }
            }
        }
    }

    public void ToggleInfo() {
        if (infoDisplayed) {
            infoDisplayed = false;
            infoText.gameObject.SetActive(false);
        }
        else {
            infoDisplayed = true;
            infoText.gameObject.SetActive(true);
        }
    }

}
