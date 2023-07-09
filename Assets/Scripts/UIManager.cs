using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    bool gameend = false;
    static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void TriggerGameOver()
    {
        Canvas gameover = GameObject.Find("GameOver").GetComponent<Canvas>();

        if (gameover.enabled == false)
        {
            gameover.enabled = true;
            instance.gameend = true;
            Time.timeScale = 0;
        }
    }
    public static bool getGameEnd()
    {
        return instance.gameend;
    }
    public static void TriggerWinScreen()
    {
        Canvas winscreen = GameObject.Find("WinScreen").GetComponent<Canvas>();
        winscreen.enabled = true;
        Time.timeScale = 0;
        Debug.Log("YOU WINNN!");
    }
}
