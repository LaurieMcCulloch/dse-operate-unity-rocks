using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameConsole : MonoBehaviour {

    // UI Controls
    public Text textConsole;
    public Text textUserID;   
    public Text textSessionID;
    public Text textUnityVersion;
    public Text textSDKVersion;
    public Text textClientVersion;
     
    // UI Debug console will be used to let player see what the game and SDKs are doing.
    public int numConsoleLines = 12;
    private List<string> console = new List<string>();


    private void Awake()
    {
        // This will allow us to display Unity Debug.Log in the UI
        Application.logMessageReceived += PrintToConsole;
    }

    private void Start()
    {
        SetConsoleVisibility(false);
    }

    private void PrintToConsole(string logString, string stackTrace, LogType type)
    {
        console.Add(string.Format("{0}::{1}\n", System.DateTime.Now.ToString("h:mm:ss tt"), logString));
        if (console.Count > numConsoleLines)
        {
            console.RemoveRange(0, console.Count - numConsoleLines);
        }
        textConsole.text = "";
        console.ForEach(i => textConsole.text += i);
    }

    // Show / Hide Info panel and Console when player clicks Info button.
    public void ToggleConsoleVisibility()
    {
        SetConsoleVisibility(!gameObject.activeSelf);
    }

    private void SetConsoleVisibility(bool isVisible)
    {

        UpdateConsole();        
        gameObject.SetActive(isVisible);
    }

    public void UpdateConsole()
    {
        textUserID.text = "UserID : ";
        textSessionID.text = "SessionID : ";
        textSDKVersion.text = "DDNA SDK Version : ";
        textUnityVersion.text = "Unity Version : " + Application.unityVersion;
        textClientVersion.text = "Client Version : " + Application.version;
    }
}
