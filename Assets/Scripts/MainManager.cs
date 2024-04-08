using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;

    //fields for display the player info
    public Text bestPlayerNameAndScore;
    public Text currentPlayerName;
    public GameObject GameOverText;
    public GameObject ExitText;

    private static int bestScore;
    private static string bestUser;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    public void Awake()
    {
        LoadGameRanks();
    }


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        currentPlayerName.text = DataPersistenceManager.Instance.userName;
        SetBestUser();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        DataPersistenceManager.Instance.score = m_Points;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        CheckBestPlayer();
        ExitText.SetActive(true);
    }
    private void CheckBestPlayer()
    {
        int currentScore = DataPersistenceManager.Instance.score;

        if (currentScore > bestScore)
        {
            bestUser = DataPersistenceManager.Instance.userName;
            bestScore = currentScore;

            bestPlayerNameAndScore.text = $"Best Score - {bestUser}:{bestScore}";

            SaveGameRanks(bestUser, bestScore);
        }
    }
    public void SetBestUser()
    {
        if (bestUser == null && bestScore == 0)
        {
            bestPlayerNameAndScore.text = "Best Score = 0";
        }
        else
        {
            bestPlayerNameAndScore.text = $"Best Score - {bestUser}:{bestScore}";
        }
    }
    public void SaveGameRanks(string bestPlayerName , int bestPlayerScore)
    {
        SaveData data = new SaveData();

        data.bestUserName = bestPlayerName;
        data.highestScore = bestPlayerScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json",json);
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

