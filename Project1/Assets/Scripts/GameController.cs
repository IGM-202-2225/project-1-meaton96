using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> enemies = new();
    [SerializeField] private GameObject enemyPreFab;
    // Start is called before the first frame update
    void Start()
    {
        float startingPosX = -8;
        for (int x = 0; x < 6; x++) {
            Vector3 pos = new(startingPosX + x * 2.5f, 0, 0);
            GameObject enemy = Instantiate(enemyPreFab, pos, Quaternion.identity);
            enemy.GetComponent<EnemyBehaviour>().Init(x);
            enemies.Add(enemy);

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
