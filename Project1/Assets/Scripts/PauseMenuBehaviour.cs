using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehaviour : MonoBehaviour {

    [SerializeField] private GameObject infoMenuCanvas;
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject shopCanvas;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void Exit() {
        Application.Quit();
    }
    public void MainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void Resume() {
        gameController.Resume();
    }
    public void Shop() {
        shopCanvas.SetActive(true);
        shopCanvas.GetComponent<ShopBehaviour>().OnShopShow();
        gameObject.SetActive(false);

    }
    public void Info() {
        gameObject.SetActive(false);
        infoMenuCanvas.SetActive(true);
        infoMenuCanvas.GetComponent<InfoMenuBehaviour>().ChangePage();
    }
}
