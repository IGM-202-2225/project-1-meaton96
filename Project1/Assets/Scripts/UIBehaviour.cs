using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;
    private PlayerBehaviour playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript != null) {
            healthBar.transform.localScale = new Vector3(
                playerScript.currentHealth * 1.0f / playerScript.maxHealth, 1, 1);
            
        }
        scoreText.text = playerScript.score + "";
        coinText.text = playerScript.coins + "";
    }
}
