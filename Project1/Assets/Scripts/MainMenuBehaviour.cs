using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject rules;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Exit() {
        Application.Quit();
    }
    public void StartGame() {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void DisplayRules() {
        rules.SetActive(true);
    }
}
