using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LoadGameRank : MonoBehaviour
{
    //It is the field where the BestPlayer info is Dislayed
    public Text bestUserName;

    private static int bestScore;
    private static string bestUser;
    //To initialize LoadGameRanks function before application starts
    public void Awake()
    {
        LoadGameRanks();
    }
    //To set the Best Player to the bestUserName Text
    public void SetBestUser()
    {
        if (bestUser == null && bestScore == 0)
        {
            bestUserName.text = "";
        }
        else
        {
            bestUserName.text = $"Best Score - " + bestUser +":"+bestScore;
        }
    }
    //To load the Games highest score after each game 
    public void LoadGameRanks()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestUser = data.bestUserName;
            bestScore = data.highestScore;
        }
    }
    //To save the data of the high score and best player
    [System.Serializable]
class SaveData
    {
        public int highestScore;
        public string bestUserName;
    }
}
