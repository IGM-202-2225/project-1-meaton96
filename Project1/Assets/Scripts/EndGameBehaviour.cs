using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endgameText;
    [SerializeField] private TextMeshProUGUI highScoreNames, highScoreScores;
    [SerializeField] private TMP_InputField inputField;
    public bool playerWon;
    public SortedDictionary<int, string> scores = new();
    public TextAsset scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake() {
        string textIn = scoreText.text;
        string[] lines = textIn.Split('\n');
        for (int x = 0; x < lines.Length; x++) {
            string[] line = lines[x].Split(',');
            scores.Add(int.Parse(line[1]), (x + 1) + "." + line[0]);
        }
        string text1 = "", text2 = "";
        foreach (KeyValuePair<int, string> pair in scores) {
            text1 += pair.Key + "\n";
            text2 += pair.Value + "\n";
        }
        highScoreNames.text = text2;
        highScoreScores.text = text1;

        
    }
    public void SaveScore() {
        string name = inputField.text;
        int score = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().score;



    }
    private void AddScore(string name, int score) {
        
    }
    public void Exit() {
        Application.Quit();
    }
    public void MainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}


