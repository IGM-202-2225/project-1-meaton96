using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour {
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Vector2 livesStartingPoint;
    [SerializeField] private float livesPadding;
    [SerializeField] private GameObject lifePreFab;
    private PlayerBehaviour playerScript;
    private Stack<GameObject> lives;
    // Start is called before the first frame update
    void Start() {
        lives = new();
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        for (int x = 0; x < playerScript.Lives; x++) {
            GameObject life = Instantiate(lifePreFab, new Vector3(livesStartingPoint.x + x * livesPadding,
                livesStartingPoint.y, 0), Quaternion.identity);
            life.transform.parent = transform;
            lives.Push(life);
        }

    }

    // Update is called once per frame
    void Update() {
        if (playerScript != null) {
            healthBar.transform.localScale = new Vector3(
                playerScript.currentHealth * 1.0f / playerScript.maxHealth, 1, 1);

        }
        scoreText.text = playerScript.score + "";
        coinText.text = playerScript.coins + "";
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
