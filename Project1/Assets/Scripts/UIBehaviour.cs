using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    private PlayerBehaviour player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            healthBar.transform.localScale = new Vector3(
                player.currentHealth * 1.0f / player.maxHealth, 1, 1);
        }
            
    }
}
