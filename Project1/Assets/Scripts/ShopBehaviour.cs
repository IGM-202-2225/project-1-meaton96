using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopBehaviour : MonoBehaviour {
    [SerializeField] private GameController gameController;         //pointer to game controller script
    [SerializeField] private TextMeshProUGUI cristalText;           //text to display how much money the player has in the shop
    [SerializeField] public PlayerBehaviour playerScript;           //player pointer
    [SerializeField] private GameObject shopItemPrefab;             //prefab for making an item in the shop
    private GameObject[] shopItems;                                 //holds all the current shop items
    public bool canPurchase;                                        //bool if the its between rounds so the player can buy or not
    public int shoppingCartAmount;                                 //holds total cost of all upgrades selected for purchase
    private const float SHOP_ITEM_X = 160, SHOP_ITEM_START_Y = 95, SHOP_ITEM_OFFSET_Y = 120;
    
    //initialize shop items
    public void Init() {
        shopItems = new GameObject[6];
        Vector3 pos;
        //distribute new shop items across the screen
        for (int x = 0; x < shopItems.Length; x++) {
            if (x < 3) {
                pos = new Vector3(-SHOP_ITEM_X, SHOP_ITEM_START_Y - SHOP_ITEM_OFFSET_Y * x, 0f);
            }
            else {
                pos = new Vector3(SHOP_ITEM_X, SHOP_ITEM_START_Y - SHOP_ITEM_OFFSET_Y * (x - 3), 0f);
            }
            shopItems[x] = Instantiate(shopItemPrefab, pos, Quaternion.identity);
            shopItems[x].transform.SetParent(transform, false);

            UpgradeBarBehaviour ubb = shopItems[x].GetComponent<UpgradeBarBehaviour>();
            //set cost, id, and pass it the shop behaviour script  for each shop item
            ubb.SetCostPerTick(x + 1);
            ubb.GetComponent<UpgradeBarBehaviour>().SetUpgradeId(x);
            ubb.SetShopScript(this);
        }
    }
    //called when the shop is pulled up
    //OnEnable and OnVisable didn't work so manually call this when shop
    public void OnShopShow() {
        for (int x = 0; x < shopItems.Length; x++) {
            //set purchased amount and reset ticks for each shop item
            shopItems[x].GetComponent<UpgradeBarBehaviour>().SetPurchasedAmount(playerScript.upgradeLevels[x]);
            shopItems[x].GetComponent<UpgradeBarBehaviour>().ResetTicks();
            shoppingCartAmount = 0; //also reset shopping cart amount
        }
    }
    // Update is called once per frame
    void Update() {

        cristalText.text = playerScript.coins - shoppingCartAmount + " (" + playerScript.coins + ")";
    }
    //add amount to shopping cart
    public void AddToShoppingCart(int amt) {
        shoppingCartAmount += amt;
    }
    //subtract amount from shopping cart
    public void SubtractFromShoppingCart(int amt) {
        shoppingCartAmount -= amt;
    }

    //resume the game when exiting the shop
    public void Exit() {
        gameController.Resume();
    }
    //purchases all shopping cart items 
    //and assigns the upgrade level to the player
    //removes shopping cart cost from player money
    //calls exit to resume the game
    public void PurchaseAndExit() {
        for (int x = 0; x < shopItems.Length; x++) {
            UpgradeBarBehaviour shopItemScript = shopItems[x].GetComponent<UpgradeBarBehaviour>();
            playerScript.SetUpgradeLevel(shopItemScript.upgradeId, shopItemScript.GetUpgradeLevel());
        }
        playerScript.coins -= shoppingCartAmount;
        Exit();
    }
}
