using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    INIT,
    ATTRACT,
    PLAY,
    NEXT_LEVEL,
    PAUSED,
    RESUMED,
    DIED,
    GAME_OVER
}

[System.Serializable]
public class Level
{
    public int asteroids;

}

[System.Serializable]
public class Game
{
    [Header("Game Configuration")]
    public int playerLivesAtStart = 3;
    public List<Level> levels;

    [Header("Game State")]
    public int playerLives; 
    public int playerLevel;
    public int score ;
    public int hiscore ;
    public GameState state;

    [Header("UI")]
    private Text txtLives;
    private Text txtScore;
    private Text txtHiscore;

    public Game()
    {
        levels = new List<Level>();
        
        state = GameState.INIT;

    }

    public void LoadLevels()
    {
        var jsonLevels = Resources.Load<TextAsset>("Levels");
        this.levels = JsonUtility.FromJson<Game>(jsonLevels.text).levels;
    }

    public void Start()
    {
        playerLevel = 0;
        score = 0;
        hiscore = GetHiscore();
        playerLives = playerLivesAtStart;
        
        txtLives = (Text)GameObject.Find("txtLives").GetComponent<Text>();
        txtScore = (Text)GameObject.Find("txtScore").GetComponent<Text>();
        txtHiscore = (Text)GameObject.Find("txtHiscore").GetComponent<Text>();

        HandleUI();
    }
    public void StartLevel(List<GameObject>asteroids, GameObject asteroidPrefab )
    {
        for (int loop = 0; loop < levels[playerLevel].asteroids; loop++)
        {
            // Spawn asteroids
            GameObject newAsteroid = GameObject.Instantiate(asteroidPrefab);

            newAsteroid.GetComponent<Asteroid>().SetSize(0);
            asteroids.Add(newAsteroid);

        }
    }
    public void NextLevel()
    {
        playerLevel++;
        if (playerLevel > levels.Count - 1)
        {
            playerLevel = 1;
        }

        HandleUI();

  
    }
    public void PlayerDied()
    {
        playerLives--;
    }
    public void IncrementScore(int points)
    {
        score += points;
        HandleUI();
    }
    public int GetHiscore()
    {
        int s = 1000;
        if (PlayerPrefs.HasKey("Hiscore"))
        {
            s = PlayerPrefs.GetInt("Hiscore");
        }
        return s;
    }
    public void SetHiscore(int s)
    {
        hiscore = s;
        PlayerPrefs.SetInt("Hiscore", hiscore);
        HandleUI();
    }
    public void HandleUI()
    {
        txtLives.text = "Lives " + (playerLives);
        txtScore.text = score.ToString();
        txtHiscore.text = hiscore.ToString();        

    }

}
