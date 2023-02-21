using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBehaviour : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite =
            GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
