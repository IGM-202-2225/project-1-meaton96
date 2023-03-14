using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransferBehaviour : MonoBehaviour
{
    public int score;
    public bool wonGame;
    public bool easyMode;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    

}
