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
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private GameObject infoBox;
    [SerializeField] private TextMeshProUGUI levelNumText;
    [SerializeField] private TextMeshProUGUI missileText;
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject bossHealthBarBg, bossHealthBar;
    [SerializeField] private GameObject endLevelPauseBg;
    [SerializeField] private TextMeshProUGUI endLevelMessageText;
    private bool bossWave = false;
    private PlayerBehaviour playerScript;
    private bool infoDisplayed;
    private BossBehaviour currentBossScript;
    // Start is called before the first frame update
    void Start() {
        infoDisplayed = infoText.IsActive();
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        enemiesRemainingText.gameObject.transform.position = new Vector3(enemiesRemainingText.gameObject.transform.position.x,
            Camera.main.WorldToScreenPoint(Vector3.zero).y * 2 - 100, 0f);

    }
    public void PauseBetweenRounds() {
        Time.timeScale = 0;

        endLevelMessageText.text = "completed level " + gameController.levelNumber + "!";
        endLevelPauseBg.SetActive(true);

    }
    public void StartNextRound() {
        Time.timeScale = 1;
        endLevelPauseBg.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (playerScript != null) {
            healthBar.transform.localScale = new Vector3(
                playerScript.currentHealth * 1.0f / playerScript.maxHealth, 1, 1);

        }
        scoreText.text = "Score: " + playerScript.score;
        coinText.text = playerScript.coins + "";
        if (infoDisplayed)
            infoText.text = playerScript.GetPlayerStat();

        levelNumText.text = "Level\n" + gameController.levelNumber;
        livesText.text = "Lives\n" + playerScript.Lives;
        enemiesRemainingText.text = "Remaining: " + gameController.enemies.Count;
        missileText.text = playerScript.numMissiles + "";

        if (bossWave) {
            bossHealthBar.transform.localScale = new Vector3(
                currentBossScript.health * 1.0f / currentBossScript.maxHealth, 1, 1);

        }

    }
    public void ToggleBossHealthBar() {
        bossWave = !bossWave;
        bossHealthBarBg.SetActive(bossWave);
        enemiesRemainingText.alpha = bossWave ? 0f : 1f;
        currentBossScript = GameObject.FindWithTag("Boss").GetComponent<BossBehaviour>();
    }

    public void ToggleInfo() {
        if (infoDisplayed) {
            infoDisplayed = false;
            infoBox.SetActive(false);
        }
        else {
            infoDisplayed = true;
            infoBox.SetActive(true);
        }
    }

}
