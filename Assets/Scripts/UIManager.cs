using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static void TriggerGameOver()
    {
        Canvas gameover = GameObject.Find("GameOver").GetComponent<Canvas>();

        if (gameover.enabled == false)
        {
            gameover.enabled = true;
            Time.timeScale = 0;
        }
        

    }
    public static void TriggerWinScreen()
    {
        Canvas winscreen = GameObject.Find("WinScreen").GetComponent<Canvas>();
        winscreen.enabled = true;
        Time.timeScale = 0;
        Debug.Log("YOU WINNN!");
    }
}
