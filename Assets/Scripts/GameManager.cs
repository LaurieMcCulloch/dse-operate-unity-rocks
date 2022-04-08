using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Core.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static UGSAnalytics Analytics;

    public Game game;
    private GameObject panelAttract;
    private GameObject panelGameOver;
    private GameObject panelPaused;

    private Ship playerShip;
    public GameObject asteroidPrefab;
    public List<GameObject> asteroids;

    bool isPaused = false;

    private async void Awake()
    {
        // Create Static Inastance on UGSManager
        Instance = this;
        Analytics = GetComponent<UGSAnalytics>();

        // Set UGS Environment
        var options = new InitializationOptions();
        options.SetEnvironmentName("development");// Configuration.UGS_ENVIRONMENT);
        options.SetAnalyticsUserId("Laurie");

        // Initialize UGS
        await UnityServices.InitializeAsync(options);
    }

    
    // Start is called before the first frame update
    void Start()
    {
      
        panelAttract = GameObject.Find("panelAttract");
        panelAttract.SetActive(false);

        panelPaused = GameObject.Find("panelPaused");
        panelPaused.SetActive(false);

        panelGameOver = GameObject.Find("panelGameOver");
        panelGameOver.SetActive(false);

        playerShip = GameObject.FindObjectOfType<Ship>();


        game = new Game();
        game.LoadLevels();

        if (game.levels.Count == 0)
        {
            Debug.Log("Unable to continue - Failed to load levels");
            return;
        }

        UpdateState(GameState.ATTRACT); 
        
    }
    private void OnApplicationQuit()
    {

    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
        Debug.Log("On Application Has Focus " + hasFocus);
        UpdateState(hasFocus ?  GameState.RESUMED : GameState.PAUSED );
    }

    void OnApplicationPause(bool pauseStatus)
    {


        isPaused = pauseStatus;
        Debug.Log("On Application Pause " + pauseStatus);
        
        UpdateState(isPaused ? GameState.PAUSED : GameState.RESUMED);
    }
    
    public void UpdateState(GameState s)
    {
        switch(s)
        {
            case GameState.ATTRACT:
                if(panelAttract != null)
                {
                    panelAttract.SetActive(true);
                }
                break;

            case GameState.PLAY:
                game.Start();
                foreach(GameObject asteroid in asteroids)
                {
                    Destroy(asteroid);
                }
                asteroids.Clear();
                playerShip.Reset();
                game.StartLevel(asteroids, asteroidPrefab);
                break;

            case GameState.NEXT_LEVEL:
                game.NextLevel();
                game.StartLevel(asteroids, asteroidPrefab);                
                break;

            case GameState.PAUSED:
                if(panelPaused!=null)
                {
                    panelPaused.SetActive(true);
                    Analytics.RecordEvent("gamePaused", null);
                }
                Debug.Log("<<PAUSED>>");
                break;

            case GameState.RESUMED:
                if (panelPaused != null)
                {
                    panelPaused.SetActive(false);
                    Analytics.RecordEvent("gameResumed", null);
                }
                Debug.Log(">>RESUMED<<");
                break; 

            case GameState.DIED:
                if (game.playerLives > 0)
                {
                    // todo put delay and short period of invulnerability in here
                    playerShip.Reset();
                }
                else
                {
                    UpdateState(GameState.GAME_OVER);
                }
                break;

            case GameState.GAME_OVER:
                // TODO Put Game Over Message or High Score here
                if (game.score > game.hiscore)
                {
                    game.SetHiscore(game.score);
                }
                UpdateState(GameState.ATTRACT);
                break;

            default:                
                break;           
        }
        game.state = s; 
    }

    public void StartGame()
    {

        if (panelAttract != null)
        {
            panelAttract.SetActive(false);
            UpdateState(GameState.PLAY);
        }
    }
}
