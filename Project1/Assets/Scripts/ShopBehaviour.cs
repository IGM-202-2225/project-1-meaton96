using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopBehaviour : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI cristalText;
    [SerializeField] private PlayerBehaviour playerScript;
    [SerializeField] private GameObject shopItemPrefab;
    private GameObject[] shopItems;
    public bool canPurchase;
    private int shoppingCartAmount;
    private const float SHOP_ITEM_X = 160, SHOP_ITEM_START_Y = 77, SHOP_ITEM_OFFSET_Y = 100;
    // Start is called before the first frame update
    void Start()
    {
        shopItems= new GameObject[5];
        Vector3 pos;
        for (int x = 0; x < shopItems.Length; x++) {
            if (x < 3) {
                pos = new Vector3(-SHOP_ITEM_X, SHOP_ITEM_START_Y * SHOP_ITEM_OFFSET_Y * x, 0f);
            }
            else {
                pos = new Vector3(SHOP_ITEM_X, SHOP_ITEM_START_Y * SHOP_ITEM_OFFSET_Y * (x - 3), 0f);
            }
            shopItems[x] = Instantiate(shopItemPrefab, pos, Quaternion.identity);   
            shopItems[x].transform.SetParent(transform, false);
            shopItems[x].GetComponent<UpgradeBarBehaviour>().SetCostPerTick(x + 1);
            shopItems[x].GetComponent<UpgradeBarBehaviour>().SetUpgradeId(x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        cristalText.text = playerScript.coins + "";
    }

    public void Exit() {
        gameController.Resume();
    }
    private bool AbleToPurchase(int cost) {
        if (!canPurchase)
            return false;
        return cost <= playerScript.coins;
    }
}
