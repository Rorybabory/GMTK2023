using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static bool gameend = false;
    public static void TriggerGameOver()
    {
        Canvas gameover = GameObject.Find("GameOver").GetComponent<Canvas>();

        if (gameover.enabled == false)
        {
            gameover.enabled = true;
            gameend = true;
            Time.timeScale = 0;
        }
    }
    public static bool getGameEnd()
    {
        return gameend;
    }
    public static void TriggerWinScreen()
    {
        Canvas winscreen = GameObject.Find("WinScreen").GetComponent<Canvas>();
        winscreen.enabled = true;
        Time.timeScale = 0;
        Debug.Log("YOU WINNN!");
    }
}
