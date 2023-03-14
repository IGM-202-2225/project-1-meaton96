using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public List<KeyValuePair<int, string>> scores = new();
    private DataTransferBehaviour dtb;
  //  public TextAsset scoreText;
    private const char DILIMINATOR = ',';
    // Start is called before the first frame update
    void Start()
    {
        dtb = GameObject.FindWithTag("Data").GetComponent<DataTransferBehaviour>();

        string path = Application.persistentDataPath + "/high_scores.txt";
        FileStream f = new(path, FileMode.OpenOrCreate);

        f.Close();


        StreamReader reader = new(path, true);

        try {
            string textIn = reader.ReadToEnd();
            string[] lines = textIn.Split('\n');
            for (int x = 0; x < lines.Length; x++) {
                string[] line = lines[x].Split(DILIMINATOR);
                AddScore(int.Parse(line[0]), line[1]);
            }
            reader.Close();
            UpdateHighscoreText();
        }
        catch (FormatException) {
            AddScore(0, "Placeholder");
            UpdateHighscoreText();
        }
        DisplayEndGameMessage();
    }
    private void Awake() {

        

    }
    private void DisplayEndGameMessage() {
        string message = "";
        if (dtb.wonGame) {
            message += "Congrats on finishing the game you finished with the high score of: " + dtb.score;
        }
        else {
            message += "Sorry you died before completing the game. You still scored " + dtb.score + " points";
        }
        endgameText.text = message;
    }

    public void SaveScore() {
        string name = inputField.text;
        int score = dtb.score;

        AddScore(score, name);
        UpdateHighscoreText();
        SaveToFile();
    }
    public void SaveToFile() {
        string path = Application.persistentDataPath + "/high_scores.txt";
        File.WriteAllText(path, string.Empty);
        StreamWriter writer = new(path, true);

        foreach (KeyValuePair<int, string> pair in scores) {
            writer.WriteLine(pair.Key + "" + DILIMINATOR + "" + pair.Value);
        }
        writer.Close();

    }
    public void UpdateHighscoreText() {
        
        string text1 = "", text2 = "";
        int x = 1;
        foreach (KeyValuePair<int, string> pair in scores) {
            text1 += pair.Key + "\n";
            text2 += x + ". " + pair.Value + "\n";
            x++;
        }
        highScoreNames.text = text2;
        highScoreScores.text = text1;
    }
    private void AddScore(int score, string name) {
        scores.Add(new KeyValuePair<int, string>(score, name));
        scores.Sort((kvp1, kvp2) => kvp2.Key.CompareTo(kvp1.Key));
        if (scores.Count > 10) {
            scores.RemoveAt(10);
        }
    }
    public void Exit() {
        Application.Quit();
    }
    public void MainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    
}


