using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBarBehaviour : MonoBehaviour {

    [SerializeField] GameObject tickPreFab;                     //prefab for displaying number of upgrades
    private GameObject[] ticks;                                 //holds the prefab instances
    public int upgradeId;                                       //id for which upgrade bar this is
    private int numTicksPurchased, numTicksCart;                //holds number of upgrades already purchased and amount selected for purchase
    private Vector3 startingPoint;                              //starting point to draw upgrade ticks
    private const int MAX_UPGRADE_AMOUNT = 8;                   //maximum amount of each ugprade type
    private const float OFFSET_X = 24.75f;                      //amount offset for each upgrade tick
    [SerializeField] private TextMeshProUGUI infoText;          //text to dispaly info about this upgrade
    [SerializeField] private GameObject infoTextBox;            //box to write info text on
    [SerializeField] private TextMeshProUGUI typeText;          //header for upgrade type
    [SerializeField] private TextMeshProUGUI costText;          //cost of upgrade
    private int costPerTick;                                    //cost per tick 
    [SerializeField] private ShopBehaviour shopScript;          //pointer to shop parent script
    // Start is called before the first frame update
    void Start() {
        //create 8 upgrade ticks and hide them
        //set their parent to this transform
        ticks = new GameObject[8];
        startingPoint = new Vector3(-98, 3, 0);
        for (int x = 0; x < ticks.Length; x++) {
            ticks[x] = Instantiate(tickPreFab, new Vector3(
                startingPoint.x + x * OFFSET_X,
                startingPoint.y,
                0f),
                Quaternion.identity);
            ticks[x].transform.SetParent(transform, false);
            ticks[x].SetActive(false);
        }
        numTicksPurchased = 0;
        SetInfoText();
    }
    public void SetShopScript(ShopBehaviour sb) {
        shopScript = sb;
    }

    //set cost of each upgrade tick, call when creating the shop item
    public void SetCostPerTick(int cost) {
        costPerTick = cost;
    }
    //get cost of next upgrade
    public int GetCostOfNextTick() {
        return costPerTick * (GetUpgradeLevel() + 1);
    }
    //set the info and type header text based on the ID
    private void SetInfoText() {
        string info = "", type = "";
        switch (upgradeId) {
            case 0:
                info = "Increases player health.";
                type = "Health";
                break;
            case 1:
                info = "Increases player armor. Armor decreases damage taken by enemy bullets";
                type = "Armor";
                break;
            case 2:
                info = "Increases player movement speed";
                type = "Movespeed";
                break;
            case 3:
                info = "Increases the damage player bullets inflict on enemies";
                type = "Damage";
                break;
            case 4:
                info = "Increases player attack speed";
                type = "Attack speed";
                break;
            case 5:
                info = "Increases number of bullets fired by the player";
                type = "Bullets";
                break;
            case 6:
                info = "Adds an additional life";
                type = "Lives";
                break;
        }
        infoText.text = info;
        typeText.text = type;
    }

    // Update is called once per frame
    void Update() {

        //changes the visability and color of each tick
        //sets visable for shopping cart or already purchased upgrades
        //hides all others
        for (int x = 0; x < ticks.Length; x++) {
            if (x < numTicksPurchased) {
                ticks[x].SetActive(true);
            }
            else if (x < numTicksCart + numTicksPurchased) {
                ticks[x].SetActive(true);
                //set the color to green for not yet purchased upgrades
                ticks[x].GetComponent<Image>().color = Color.green;
            }
            else
                ticks[x].SetActive(false);
        }
        //update cost text
        costText.text = GetCostOfNextTick() + "";

    }
    //set the upgrade ID, to be called when creating the shop item
    public void SetUpgradeId(int id) {
        upgradeId = id;
    }
    //increase the number of upgrades if possible
    public void IncreaseTicks() {
        if (numTicksPurchased + numTicksCart < MAX_UPGRADE_AMOUNT &&
            shopScript.playerScript.CanPurchaseItem(GetCostOfNextTick(), shopScript.shoppingCartAmount) &&
            Enabled()) {
            shopScript.AddToShoppingCart(GetCostOfNextTick());
            numTicksCart++;

        }

    }
    //decrease the number of upgrades if possible
    public void DecreaseTicks() {
        if (numTicksCart > 0 && Enabled()) {
            numTicksCart--;
            shopScript.SubtractFromShoppingCart(GetCostOfNextTick());
        }

    }
    //checks if shop items are enabled (between rounds)
    public bool Enabled() {
        return shopScript.canPurchase;
    }
    //resets color for purchased upgrades
    public void ResetTicks() {
        for (int x = 0; x < numTicksPurchased; x++) {
            ticks[x].GetComponent<Image>().color = Color.white;
        }
        numTicksCart = 0;
    }


    //get the total number of upgrade ticks
    public int GetUpgradeLevel() {
        return numTicksPurchased + numTicksCart;
    }
    //show or hide the info box on hover of info button
    public void ShowInfoBox() {
        infoTextBox.SetActive(true);
    }
    public void HideInfoBox() {
        infoTextBox.SetActive(false);
    }
    public void SetPurchasedAmount(int amount) {
        numTicksPurchased = amount;
    }

}
