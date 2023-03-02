using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenuBehaviour : MonoBehaviour {
    [SerializeField] private GameObject pauseMenuCanvas;                        //pointer to pause menu
    [SerializeField] private TextMeshProUGUI titleText;                         //holds title text mesh for changing title of info slide
    [SerializeField] private GameController gameController;                     //game controller pointer
    [SerializeField] private TextMeshProUGUI infoText;                          //holds info text mesh
    [SerializeField] private Sprite[] ENEMY_SPRITES = new Sprite[5];            //holds enemy sprites to draw info images
    [SerializeField] private Sprite BULLET_SPRITE;                              //holds the enemy bullet sprite
    private int pageNumber;                                                     //keeps track of which page is currently displayed
    [SerializeField] private Image[] images = new Image[3];                     //holds 3 images to display on info page
    private const int PAGE_COUNT = 4;                                           //current max amount of pages 

    //back button
    public void Back() {
        pauseMenuCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
    //increases the page number if possible, otherwise go back to beginning
    public void IncrementPageNumber() {
        pageNumber = pageNumber < PAGE_COUNT - 1 ? pageNumber + 1 : 0;
        ChangePage();
    }
    //decreases the page number if possible, otherwise go to the end
    public void DecrementPageNumber() {
        pageNumber = pageNumber >= 0 ? pageNumber - 1 : PAGE_COUNT - 1;
        ChangePage();
    }
    //called to swap the page
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
            case 3: 
                DisplayBulletInfo(3, 4);
                break;
        }
    }
    //add up to 3 enemy info sections and add corresponding images
    private void DisplayEnemyInfo(params int[] indexes) {
        titleText.text = "Enemies";
        infoText.text = "";
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
    //add up to 3 bullet info sections and add corresponding images
    private void DisplayBulletInfo(params int[] indexes) {
        //set title and reset textbox, adjust font size
        titleText.text = "Projectiles";
        infoText.text = "";
        for (int x = 0; x < images.Length; x++) {
            //if less than 3 indexes were passed in, set alpha of extra images to 0 to hide
            if (x >= indexes.Length) {
                images[x].color = new Color(0, 0, 0, 0);
            }
            else {
                //otherwise add to the info text box and change the image sprite and color
                infoText.text += "Damage\n" + indexes[x] * 10 + "-" + (indexes[x] + 1) * 10;
                images[x].sprite = BULLET_SPRITE;
                images[x].color = EnemyBehaviour.BULLET_COLORS[indexes[x]];
                infoText.text += "\n\n";
            }
        }
    }
    
}
