using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenuBehaviour : MonoBehaviour {
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Sprite[] ENEMY_SPRITES = new Sprite[5];
    [SerializeField] private Sprite BULLET_SPRITE;
    private int pageNumber;
    [SerializeField] private Image[] images = new Image[3];
    private const int PAGE_COUNT = 4;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void Back() {
        pauseMenuCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
    public void IncrementPageNumber() {
        pageNumber = pageNumber < PAGE_COUNT - 1 ? pageNumber + 1 : 0;
        ChangePage();
    }
    public void DecrementPageNumber() {
        pageNumber = pageNumber >= 0 ? pageNumber - 1 : PAGE_COUNT - 1;
        ChangePage();
    }
    public void ChangePage() {
        switch (pageNumber) {
            case 0:
                DisplayEnemyInfo(0, 1, 2);
                break;
            case 1:
                DisplayEnemyInfo(3, 4);
                break;
            case 2:
                DisplayBulletInfo(0, 1, 2);
                break;
            case 3: DisplayBulletInfo(3, 4);
                break;
        }
    }

    private void DisplayEnemyInfo(params int[] indexes) {
        titleText.text = "Enemies";
        infoText.text = "";
        infoText.fontSize = 18;
        for (int x = 0; x < images.Length; x++) {
            if (x >= indexes.Length) {
                images[x].color = new Color(0, 0, 0, 0);
            }
            else {
                infoText.text += "Type " + indexes[x] + "- Attack: " + EnemyBehaviour.DAMAGE_DONE_EACH_ENEMY[indexes[x]] +
                    " Bullets: " + EnemyBehaviour.NUM_BULLETS_EACH_ENEMY[indexes[x]];
                images[x].sprite = ENEMY_SPRITES[indexes[x]];
                images[x].color = EnemyBehaviour.COLOR_EACH_ENEMY[indexes[x]];
                infoText.text += "\n\n";
            }
        }
    }
    private void DisplayBulletInfo(params int[] indexes) {
        titleText.text = "Projectiles";
        infoText.text = "";
        infoText.fontSize = 20;
        for (int x = 0; x < images.Length; x++) {
            if (x >= indexes.Length) {
                images[x].color = new Color(0, 0, 0, 0);
            }
            else {
                infoText.text += "Damage\n" + indexes[x] * 10 + "-" + (indexes[x] + 1) * 10;
                images[x].sprite = BULLET_SPRITE;
                images[x].color = EnemyBehaviour.BULLET_COLORS[indexes[x]];
                infoText.text += "\n\n";
            }
        }
    }
    
}
