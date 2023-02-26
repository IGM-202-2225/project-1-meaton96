using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Sprite[] ENEMY_SPRITES = new Sprite[5];
    private int pageNumber;
    [SerializeField] private Image[] images = new Image[3];
    private const int PAGE_COUNT = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            case 0: DisplayEnemyPageOne();
                break;
            case 1: DisplayEnemyPageTwo(); 
                break;
        }
    }

    private void AppendInfoText(params int[] indexes) {
        for (int x = 0; x < indexes.Length; x++) {
            Debug.Log(indexes[x]);
            //not quite working
            infoText.text += "Type " + indexes[x] + "- Attack: " + EnemyBehaviour.DAMAGE_DONE_EACH_ENEMY[indexes[x]] +
                " Bullets: " + EnemyBehaviour.NUM_BULLETS_EACH_ENEMY[indexes[x]];
            images[indexes[x]].sprite = ENEMY_SPRITES[indexes[x]];
            images[indexes[x]].color = EnemyBehaviour.COLOR_EACH_ENEMY[indexes[x]];
            infoText.text += "\n\n";
        }
    }
    private void DisplayEnemyPageOne() {
        titleText.text = "Enemies";
        infoText.text = "";
        AppendInfoText(0, 1, 2);
   
    }
    private void DisplayEnemyPageTwo() {
        titleText.text = "Enemies";
        infoText.text = "";
        AppendInfoText(3, 4);

    }
}
