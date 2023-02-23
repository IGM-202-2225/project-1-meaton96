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
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject InfoBox;
    private const float LIFE_SCALE = .7f;
    private PlayerBehaviour playerScript;
    private bool infoDisplayed;
    // Start is called before the first frame update
    void Start() {
        infoDisplayed = infoText.IsActive();
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
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
        livesText.text = "Lives\n" + playerScript.Lives;

    }

    public void ToggleInfo() {
        if (infoDisplayed) {
            infoDisplayed = false;
            InfoBox.SetActive(false);
        }
        else {
            infoDisplayed = true;
            InfoBox.SetActive(true);
        }
    }

}
