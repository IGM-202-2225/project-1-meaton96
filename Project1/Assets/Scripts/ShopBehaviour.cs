using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopBehaviour : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI cristalText;
    [SerializeField] public PlayerBehaviour playerScript;
    [SerializeField] private GameObject shopItemPrefab;
    private GameObject[] shopItems;
    public bool canPurchase;
    private int shoppingCartAmount;
    private const float SHOP_ITEM_X = 160, SHOP_ITEM_START_Y = 95, SHOP_ITEM_OFFSET_Y = 120;
    // Start is called before the first frame update
    void Start() {
        
    }
    public void Init() {
        shopItems = new GameObject[6];
        Vector3 pos;
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

            ubb.SetCostPerTick(x + 1);
            ubb.GetComponent<UpgradeBarBehaviour>().SetUpgradeId(x);
            ubb.SetShopScript(this);
        }
    }
    public void OnShopShow() {
        for (int x = 0; x < shopItems.Length; x++) {
            shopItems[x].GetComponent<UpgradeBarBehaviour>().SetPurchasedAmount(playerScript.upgradeLevels[x]);
            shopItems[x].GetComponent<UpgradeBarBehaviour>().ResetTicks();
            shoppingCartAmount = 0;
        }
    }
    // Update is called once per frame
    void Update() {

        cristalText.text = playerScript.coins - shoppingCartAmount + " (" + playerScript.coins + ")";
    }
    public void AddToShoppingCart(int amt) {
        shoppingCartAmount += amt;
    }
    public void SubtractFromShoppingCart(int amt) {
        shoppingCartAmount -= amt;
    }

    public void Exit() {
        gameController.Resume();
    }
    private bool AbleToPurchase(int cost) {
        if (!canPurchase)
            return false;
        return cost <= playerScript.coins;
    }

    public void PurchaseAndExit() {
        for (int x = 0; x < shopItems.Length; x++) {
            UpgradeBarBehaviour shopItemScript = shopItems[x].GetComponent<UpgradeBarBehaviour>();
            playerScript.SetUpgradeLevel(shopItemScript.upgradeId, shopItemScript.GetUpgradeLevel());
        }
        playerScript.coins -= shoppingCartAmount;
        Exit();
    }
}
